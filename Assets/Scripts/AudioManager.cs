using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot creepySnapshot;

    private bool isPlaying = false;

    private void Awake()
    {
        Instance = this;
    }

    // Call this when player enters étage 2
    public void StartMusic()
    {
        if (musicSource != null && !isPlaying)
        {
            musicSource.Play();
            isPlaying = true;
            normalSnapshot.TransitionTo(0.1f);
        }
    }

    public void StartCreepyDistortion()
    {
        creepySnapshot?.TransitionTo(0.8f);
    }

    public void ReturnNormalMusic()
    {
        normalSnapshot?.TransitionTo(1.2f);
    }
}
