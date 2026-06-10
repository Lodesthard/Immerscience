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

    private Coroutine _fadeRoutine;

    // Arrête la musique de la creepy zone avec un fondu. Appelé quand le joueur
    // quitte la zone (ex. téléportation vers la salle 3).
    public void StopMusic(float fadeTime = 1.5f)
    {
        // Remet le mix en mode normal quoi qu'il arrive.
        if (normalSnapshot != null) normalSnapshot.TransitionTo(fadeTime);

        if (musicSource == null || !musicSource.isPlaying) return;

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeOutAndStop(fadeTime));
    }

    private IEnumerator FadeOutAndStop(float fadeTime)
    {
        float startVol = musicSource.volume;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
            yield return null;
        }
        musicSource.Stop();
        musicSource.volume = startVol; // restaure pour un futur StartMusic
        _fadeRoutine = null;
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
