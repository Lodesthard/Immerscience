using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot creepySnapshot;

    // Frame-level guard to prevent simultaneous engine execution
    private bool _isPlayRequestedThisFrame = false;

    private void Awake()
    {
        Instance = this;
    }

    public void StartMusic()
    {
        if (musicSource == null) return;

        // 1. Standard check if it's already running from a previous frame
        if (musicSource.isPlaying)
        {
            return; 
        }

        // 2. Hardware race-condition check for simultaneous same-frame calls
        if (_isPlayRequestedThisFrame)
        {
            return;
        }

        // Lock down instantly, play, and schedule the lock release for next frame
        _isPlayRequestedThisFrame = true;
        musicSource.Play();
        Debug.Log("🎵 Music started fresh from the beginning.");

        StartCoroutine(ResetFrameLock());
    }

    private IEnumerator ResetFrameLock()
    {
        yield return null; // Waits exactly 1 frame
        _isPlayRequestedThisFrame = false;
    }

    public void StartCreepyDistortion()
    {
        Debug.Log("🎵 → CREEPY DISTORTION");
        creepySnapshot?.TransitionTo(0.5f);   // faster + stronger
    }

    public void ReturnNormalMusic()
    {
        Debug.Log("🎵 → NORMAL");
        normalSnapshot?.TransitionTo(1.0f);
    }
}
