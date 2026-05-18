using UnityEngine;
using UnityEngine.Audio;   // Important

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioSource musicSource;

    [Header("Distortion Settings")]
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot creepySnapshot;

    private void Awake()
    {
        Instance = this;
    }

    public void StartCreepyDistortion()
    {
        creepySnapshot?.TransitionTo(0.8f);   // Smooth transition to distorted state
    }

    public void ReturnNormalMusic()
    {
        normalSnapshot?.TransitionTo(1.2f);   // Slightly slower return
    }
}
