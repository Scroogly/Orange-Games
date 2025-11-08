/********************************************
 * File: AudioManager.cs
 * Author: Orange Games Team (Gurneet)
 * Description: Central audio manager for BGM/SFX,
 * volume, mute, pause/resume, and fade switching.
 ********************************************/
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer & Parameters")]
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private string _musicParam = "MusicVol";
    [SerializeField] private string _sfxParam = "SFXVol";

    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip _mainTheme;
    [SerializeField] private AudioClip _bossTheme;

    const string PP_MUSIC = "pp_music_vol";
    const string PP_SFX   = "pp_sfx_vol";
    bool _muted;

    void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadVolumes();
    }

    void Start() {
        if (_mainTheme != null) PlayMainTheme();
    }

    // ---- BGM ----
    public void PlayMainTheme() => FadeToTrack(_mainTheme, 0.8f);
    public void PlayBossTheme() => FadeToTrack(_bossTheme, 0.8f);

    void FadeToTrack(AudioClip clip, float fade) {
        if (clip == null) return;
        StartCoroutine(FadeRoutine(clip, fade));
    }

    IEnumerator FadeRoutine(AudioClip next, float t) {
        float start = _musicSource.volume;
        for (float a = 0; a < t; a += Time.unscaledDeltaTime) {
            _musicSource.volume = Mathf.Lerp(start, 0f, a / t);
            yield return null;
        }
        _musicSource.clip = next;
        _musicSource.loop = true;
        _musicSource.Play();
        for (float a = 0; a < t; a += Time.unscaledDeltaTime) {
            _musicSource.volume = Mathf.Lerp(0f, 1f, a / t);
            yield return null;
        }
        _musicSource.volume = 1f;
    }

    // ---- SFX ----
    public void PlaySFX(AudioClip clip, float vol = 1f) {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip, Mathf.Clamp01(vol));
    }

    // ---- Volume (sliders 0..1) with log mapping for mixer ----
    public void SetMusicVolume01(float v) {
        SetLogVolume(_musicParam, v);
        PlayerPrefs.SetFloat(PP_MUSIC, v);
    }
    public void SetSFXVolume01(float v) {
        SetLogVolume(_sfxParam, v);
        PlayerPrefs.SetFloat(PP_SFX, v);
    }

    void SetLogVolume(string param, float v01) {
        // Convert 0..1 slider to decibels (-80..0). Avoid -Inf.
        float dB = (v01 <= 0.0001f) ? -80f : Mathf.Lerp(-30f, 0f, Mathf.Sqrt(v01));
        _mixer.SetFloat(param, dB);
    }

    void LoadVolumes() {
        float mv = PlayerPrefs.GetFloat(PP_MUSIC, 0.8f);
        float sv = PlayerPrefs.GetFloat(PP_SFX, 0.8f);
        SetMusicVolume01(mv);
        SetSFXVolume01(sv);
    }

    // ---- Mute / Pause ----
    public void ToggleMute() {
        _muted = !_muted;
        AudioListener.pause = _muted;        // pauses all sources reliably
    }
}
