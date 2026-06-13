using UnityEngine;

/// <summary>
/// Game-specific audio controller.
/// Attach to a persistent GameObject alongside AudioManager.
/// All game systems call this — never call AudioManager directly from gameplay code.
/// This keeps game logic decoupled from audio implementation.
/// </summary>
public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }

    [SerializeField] private SoundLibrary library;

    private PooledSource _sapLoopSource;
    private PooledSource _fireflyAmbientSource;
    private PooledSource _cobwebBurnSource;

    private bool _sapChaseActive = false;

    // Unity Lifecycle
    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayExploreMusic();
    }

    #endregion

    // Player Stuff
    #region Player

    public void OnJarThrow(Vector3 pos) =>
        AudioManager.Instance.PlaySFX(library.jarThrow, pos, SoundPriority.Normal);

    public void OnJarLand(Vector3 pos) =>
        AudioManager.Instance.PlaySFX(library.jarLand, pos, SoundPriority.Normal);

    public void OnJarGrapple(Vector3 pos) =>
        AudioManager.Instance.PlaySFX(library.jarGrapple, pos, SoundPriority.Normal);

    /// <summary>Bugle double jump will duck music briefly, plays at full volume globally.</summary>
    public void OnBuglerJump()
    {
        AudioManager.Instance.PlaySFXGlobal(library.buglerJump, SoundPriority.Critical, volume: 0.9f);
        AudioManager.Instance.DuckMusic(duckVolume: 0.4f, holdTime: 0.3f, fadeTime: 0.25f);
    }

    public void OnSquirrelLand(Vector3 pos) =>
        AudioManager.Instance.PlaySFX(library.squirrelLand, pos, SoundPriority.Normal, volume: 0.7f);

    public void OnSquirrelFootstep(Vector3 pos) =>
        AudioManager.Instance.PlaySFX(library.squirrelFootstep, pos, SoundPriority.Low, volume: 0.5f);

    public void OnSquirrelDeath(Vector3 pos)
    {
        StopAllLooping();
        AudioManager.Instance.PlaySFX(library.squirrelDeath, pos, SoundPriority.Critical);
        AudioManager.Instance.StopMusic(fadeDuration: 0.5f);
    }

    #endregion

    // Fireflies
    #region Fireflies

    public void OnFireflyCatch(Vector3 pos) =>
        AudioManager.Instance.PlaySFX(library.fireflyCatch, pos, SoundPriority.High, volume: 0.8f);

    public void StartFireflyAmbient()
    {
        if (_fireflyAmbientSource != null) return; // already playing
        _fireflyAmbientSource = AudioManager.Instance.PlayLooping(library.fireflyAmbient,
                                                                   SoundPriority.Low, volume: 0.4f);
    }

    public void StopFireflyAmbient()
    {
        AudioManager.Instance.StopLooping(_fireflyAmbientSource);
        _fireflyAmbientSource = null;
    }

    #endregion

    // Sap
    #region Sap

    /// <summary>Call when sap starts rising in this zone.</summary>
    public void StartSapChase()
    {
        if (_sapChaseActive) return;
        _sapChaseActive = true;
        _sapLoopSource = AudioManager.Instance.PlayLooping(library.sapRise, SoundPriority.High, volume: 0.6f);
        PlayChaseMusic();
    }

    /// <summary>Call when sap is dangerously close to the player.</summary>
    public void OnSapNearPlayer(Vector3 sapPos)
    {
        // Swap loop clip to intense version
        if (_sapLoopSource != null)
        {
            AudioManager.Instance.StopLooping(_sapLoopSource);
            _sapLoopSource = AudioManager.Instance.PlayLooping(library.sapClose, SoundPriority.High, volume: 0.85f);
        }
    }

    public void OnSapTouchPlayer(Vector3 pos)
    {
        AudioManager.Instance.PlaySFX(library.sapTouch, pos, SoundPriority.Critical);
        StopSapChase();
    }

    public void StopSapChase()
    {
        if (!_sapChaseActive) return;
        _sapChaseActive = false;
        AudioManager.Instance.StopLooping(_sapLoopSource);
        _sapLoopSource = null;
        PlayExploreMusic();
    }

    #endregion

    // Cobwebs and Fire
    #region Cobweb / Fire

    public void OnCobwebIgnite(Vector3 pos)
    {
        AudioManager.Instance.PlaySFX(library.cobwebIgnite, pos, SoundPriority.High);
        _cobwebBurnSource = AudioManager.Instance.PlayLooping(library.cobwebBurn, SoundPriority.Normal, volume: 0.6f);
    }

    public void OnCobwebCleared(Vector3 pos)
    {
        AudioManager.Instance.StopLooping(_cobwebBurnSource);
        _cobwebBurnSource = null;
        AudioManager.Instance.PlaySFX(library.cobwebClear, pos, SoundPriority.Critical, volume: 1f);
    }

    #endregion

    // Zone
    #region Zone

    public void OnZoneEnter(Vector3 pos) =>
        AudioManager.Instance.PlaySFX(library.zoneEnter, pos, SoundPriority.Normal);

    public void OnZoneClear(Vector3 pos)
    {
        StopAllLooping();
        AudioManager.Instance.PlaySFX(library.zoneClear, pos, SoundPriority.Critical);
        PlayExploreMusic();
    }

    public void OnVictory()
    {
        StopAllLooping();
        AudioManager.Instance.PlayMusic(library.musicVictory, fadeDuration: 2f);
    }

    #endregion

    // Music HElpers
    #region Music Helpers

    private void PlayExploreMusic() =>
        AudioManager.Instance.PlayMusic(library.musicExplore, fadeDuration: 2f);

    private void PlayChaseMusic() =>
        AudioManager.Instance.PlayMusic(library.musicChase, fadeDuration: 0.8f);

    #endregion

    // Cleanup
    #region Cleanup

    private void StopAllLooping()
    {
        AudioManager.Instance.StopLooping(_sapLoopSource);
        AudioManager.Instance.StopLooping(_fireflyAmbientSource);
        AudioManager.Instance.StopLooping(_cobwebBurnSource);
        _sapLoopSource = null;
        _fireflyAmbientSource = null;
        _cobwebBurnSource = null;
        _sapChaseActive = false;
    }

    #endregion
}