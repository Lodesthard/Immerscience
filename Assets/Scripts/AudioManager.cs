using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioSource musicSource;

    [Header("Mixer Snapshots")]
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot creepySnapshot;

    private void Awake()
    {
        Instance = this;
    }

    public void StartMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
            Debug.Log("🎵 Music Started");
        }
    }

    public void UpdateDistortion(float meltAmount)
    {
        if (normalSnapshot == null || creepySnapshot == null) return;

        if (meltAmount > 0.15f)
            creepySnapshot.TransitionTo(0.8f);
        else
            normalSnapshot.TransitionTo(1.2f);
    }
}
