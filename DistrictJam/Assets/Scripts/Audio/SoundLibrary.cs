using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// ScriptableObject that holds all sound definitions for the game.
/// Create via Assets > Create > Audio > Sound Library.
/// Drag clips in the Inspector
/// </summary>
/// 

// UPDATE LATER WITH ACTUAL SOUNDS!!! THESE ARE TEMP
[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Header("Player")]
    public AudioClip jarThrow;
    public AudioClip jarLand;
    public AudioClip jarGrapple;
    public AudioClip buglerJump;
    public AudioClip squirrelLand;
    public AudioClip squirrelFootstep;
    public AudioClip squirrelDeath;

    [Header("Fireflies")]
    public AudioClip fireflyCatch;
    public AudioClip fireflyAmbient;

    [Header("Sap")]
    public AudioClip sapRise;            
    public AudioClip sapClose;           
    public AudioClip sapTouch;          

    [Header("Fire / Cobweb")]
    public AudioClip cobwebIgnite;
    public AudioClip cobwebBurn;         
    public AudioClip cobwebClear;        

    [Header("Zone")]
    public AudioClip zoneEnter;
    public AudioClip zoneClear;

    [Header("Music")]
    public AudioClip musicExplore;      
    public AudioClip musicChase;         
    public AudioClip musicVictory;      
}