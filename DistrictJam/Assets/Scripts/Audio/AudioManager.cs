using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Core Audio Manager Class. -- Singleton with pooled AudioSources, priority evictions,
/// mixer based ducking, 2D distance attenuation.
/// To use, attach it to a persistent GameObject in the first scene. (Empty  AudioManager GameObject is fine)
/// </summary>

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("Pool Settings")]
    [SerializeField] private int sfxPoolSize = 16;
    [SerializeField] private int musicPoolSize = 2; // For crossfading music

    [Header("Mixer Group References")]
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup ambientGroup;
    [SerializeField] private AudioMixerGroup uiGroup;

    // Pools
    private List<PooledSource> _sfxPool = new();
    private AudioSource[] _musicSources;
    private int _activeMusicIndex = 0;

    // Ducking
    private Coroutine _duckCoroutine;
    private const string MUSIC_VOL_PARAM = "MusicVolume";
    private const string SFX_VOL_PARAM   = "SFXVolume";

    // State
    private bool _isMusicPlaying = false;

    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildPools();
    }

    #endregion

    // Construction
    #region Pool Construction

    private void BuildPools()
    {
        // SFX Pool
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var go = new GameObject($"SFX_Pool_{i}");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = sfxGroup;
            src.playOnAwake = false;
            _sfxPool.Add(new PooledSource(src));
        }

        // Music Sources
        _musicSources = new AudioSource[musicPoolSize];
        for (int i = 0; i < musicPoolSize; i++)
        {
            var go = new GameObject($"Music_{i}");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = musicGroup;
            src.playOnAwake = false;
            src.loop = true;
            _musicSources[i] = src;
        }
    }

    #endregion

    // Public SFX API
    #region Public SFX API

    /// <summary> Plays one-shot SFX at world position </summary>
    public void PlaySFX(AudioClip clip, Vector3 worldPos, SoundPriority priority = SoundPriority.Normal,
                        float volume = 1f, float pitch = 1f, float maxDistance = 20f)
    {
        if (clip == null) return;

        var source = GetAvailableSource(priority);
        if (source == null) return;

        // 2D spatial attenuation — camera is "above", scale volume by distance
        float dist = Vector3.Distance(worldPos, Camera.main.transform.position);
        float attenuation = Mathf.Clamp01(1f - dist / maxDistance);

        source.src.clip = clip;
        source.src.volume = volume * attenuation;
        source.src.pitch = pitch;
        source.src.loop = false;
        source.priority = priority;
        source.src.Play();

        StartCoroutine(ReturnToPoolWhenDone(source));
    }

    ///<summary> Play a one-shot SFX with no positional attenuation (UI, bugle, jump etc) </summary>
    public void PlaySFXGlobal(AudioClip clip, SoundPriority priority = SoundPriority.Normal,
                           float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        var source = GetAvailableSource(priority);
        if (source == null) return;

        source.src.clip = clip;
        source.src.volume = volume;
        source.src.pitch = pitch;
        source.src.loop = false;
        source.priority = priority;
        source.src.Play();

        StartCoroutine(ReturnToPoolWhenDone(source));
    }

    ///<summary> Play looping ambient sound. Returns to PooledSource so caller can stop it </summary>
    public PooledSource PlayLooping(AudioClip clip, SoundPriority priority = SoundPriority.Low,
    float volume = 1f)
    {
        if (clip == null) return null;
 
        var source = GetAvailableSource(priority);
        if (source == null) return null;
 
        source.src.outputAudioMixerGroup = ambientGroup;
        source.src.clip   = clip;
        source.src.volume = volume;
        source.src.loop   = true;
        source.priority   = priority;
        source.src.Play();
        return source;
    }

    public void StopLooping(PooledSource source)
    {
        if (source == null) return;
        source.src.Stop();
        source.src.outputAudioMixerGroup = sfxGroup; // Reset group
        source.priority = SoundPriority.None; // Reset priority
    }
    #endregion

    // Public Music API
    #region Public Music API
    /// <summary> Crossfade to new music track </summary>
    public void PlayMusic(AudioClip clip, float fadeDuration = 1.5f, float targetVolume = 1f)
    {
        if (clip == null) return;
        StartCoroutine(CrossfadeMusic(clip, fadeDuration, targetVolume));
    }

    public void StopMusic(float fadeDuration = 1f)
    {
        StartCoroutine(FadeOutMusic(fadeDuration));
    }

    /// <summary> Dusk Music temporarily. Uses for bugle jump or big sfx moments </summary>
    public void DuckMusic(float duckVolume = 0.3f, float holdTime = 0.5f, float fadeTime = 0.4f)
    {
        if (_duckCoroutine != null) StopCoroutine(_duckCoroutine);
        _duckCoroutine = StartCoroutine(DuckRoutine(duckVolume, holdTime, fadeTime));
    }
    #endregion

    // Global Volume Controls
    #region Global Volume Controls

    ///<summary> Set master volume </summary>
    public void SetMasterVolume(float value) =>
    masterMixer.SetFloat("MasterVolume", LinearToDecibel(value));
 
    public void SetMusicVolume(float value) =>
        masterMixer.SetFloat(MUSIC_VOL_PARAM, LinearToDecibel(value));

    public void SetSFXVolume(float value) =>
        masterMixer.SetFloat(SFX_VOL_PARAM, LinearToDecibel(value));

    #endregion

    // Pool Logic
    #region Pool Logic

    private PooledSource GetAvailableSource(SoundPriority requestedPriority)
    {
        // First pass: find an idle source
        foreach (var ps in _sfxPool)
            if (!ps.src.isPlaying) return ps;

        // Second pass: evict lowest priority source
        PooledSource lowestPriority = null;
        foreach (var ps in _sfxPool)
        {
            if (lowestPriority == null || ps.priority < lowestPriority.priority)
                lowestPriority = ps;
        }

        if (lowestPriority != null && lowestPriority.priority <= requestedPriority)
        {
            lowestPriority.src.Stop();
            return lowestPriority;
        }

        // Pool is full of higher priority sounds -> drop this request
        Debug.LogWarning("[AudioManager] Pool full, sound dropped.");
        return null;
    }

    private IEnumerator ReturnToPoolWhenDone(PooledSource ps)
    {
        yield return new WaitWhile(() => ps.src.isPlaying);
        ps.priority = SoundPriority.None;
    }

    #endregion

    // Coroutines
    #region Coroutines

    private IEnumerator CrossfadeMusic(AudioClip newClip, float duration, float targetVolume)
    {
        int nextIndex = (_activeMusicIndex + 1) % 2;
        var outSrc = _musicSources[_activeMusicIndex];
        var inSrc = _musicSources[nextIndex];

        inSrc.clip = newClip;
        inSrc.volume = 0f;
        inSrc.Play();

        float startVolume = outSrc.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            inSrc.volume = Mathf.Lerp(0f, targetVolume, t);
            outSrc.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        outSrc.Stop();
        _activeMusicIndex = nextIndex;
        _isMusicPlaying = true;
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        var src = _musicSources[_activeMusicIndex];
        float startVolume = src.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            src.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        src.Stop();
        _isMusicPlaying = false;
    }

    private IEnumerator DuckRoutine(float duckVolume, float holdTime, float fadeTime)
    {
        // Duck down
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            masterMixer.SetFloat(MUSIC_VOL_PARAM, Mathf.Lerp(0f, LinearToDecibel(duckVolume), elapsed / fadeTime));
            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        // Restore
        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            masterMixer.SetFloat(MUSIC_VOL_PARAM, Mathf.Lerp(LinearToDecibel(duckVolume), 0f, elapsed / fadeTime));
            yield return null;
        }

        masterMixer.SetFloat(MUSIC_VOL_PARAM, 0f); // 0dB = full volume
    }

    #endregion

    // Utilities
    #region Utilities
    private float LinearToDecibel(float linear)
    {
        return linear > 0.0001f ? Mathf.Log10(linear) * 20f : -80f;
    }

    #endregion
}

// Supporting Types
#region Supporting Types

public enum SoundPriority
{
    None = 0,
    Low = 1,   // ambient, distant fireflies
    Normal = 2,   // general SFX, jar throw
    High = 3,   // sap chase, fire ignition
    Critical = 4    // bugle, death, zone clear
}

public class PooledSource
{
    public AudioSource src;
    public SoundPriority priority;

    public PooledSource(AudioSource source)
    {
        src = source;
        priority = SoundPriority.None;
    }
}

#endregion