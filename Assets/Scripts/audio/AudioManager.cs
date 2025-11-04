using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I;

    [Header("Mixer routing")]
    public AudioMixerGroup musicOut;
    public AudioMixerGroup sfxOut;

    AudioSource musicSrc;
    AudioSource sfxSrc;

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        musicSrc = gameObject.AddComponent<AudioSource>();
        sfxSrc   = gameObject.AddComponent<AudioSource>();

        musicSrc.loop = true;
        musicSrc.playOnAwake = false;
        sfxSrc.playOnAwake   = false;

        if (musicOut) musicSrc.outputAudioMixerGroup = musicOut;
        if (sfxOut)   sfxSrc.outputAudioMixerGroup   = sfxOut;
    }

    // --- Music ---
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (!clip) return;
        musicSrc.clip = clip;
        musicSrc.loop = loop;
        musicSrc.Play();
    }

    public void StopMusic()
    {
        if (musicSrc && musicSrc.isPlaying) musicSrc.Stop();
    }

    public bool MusicIsPlaying => musicSrc && musicSrc.isPlaying;

    // --- SFX ---
    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (!clip) return;
        sfxSrc.pitch = pitch;
        sfxSrc.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}
