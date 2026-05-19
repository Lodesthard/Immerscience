using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot creepySnapshot;

    private void Awake()
    {
        Instance = this;
    }

    public void StartMusic()
    {
        if (musicSource != null) musicSource.Play();
    }

    public void StartCreepyDistortion()
    {
        creepySnapshot?.TransitionTo(1.2f);
    }

    public void ReturnNormalMusic()
    {
        normalSnapshot?.TransitionTo(1.5f);
    }
}
