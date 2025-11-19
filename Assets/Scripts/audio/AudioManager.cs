/********************************************
 * File: AudioManager.cs
 * Author: Orange Games Team
 * Description: Centralized audio system for SFX/BGM,
 * crossfades, pause state, and mixer routing.
 ********************************************/
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

/// Singleton manager for game audio: SFX, BGM, volumes, pause/resume.
/// STATIC BINDING EXAMPLE: THIS CLASS DOES NTO USE VIRTUAL METHODS, SO ALL FUNCTION CALLS ARE DECIDED AT A COMPILE TIME
public class AudioManager : MonoBehaviour
{
    [Header("Mixer & Groups")]
    [SerializeField] private AudioMixer _masterMixer;
    [SerializeField] private AudioMixerGroup _bgmGroup;
    [SerializeField] private AudioMixerGroup _sfxGroup;

    [Header("Library")]
    [SerializeField] private SoundLibrary _library;

    [Header("Players")]
    [SerializeField] private AudioSource _bgmSourceA;
    [SerializeField] private AudioSource _bgmSourceB;
    [SerializeField] private AudioSource _sfxSourcePrefab;

    [Header("Exposed Params")]
    [SerializeField] private string _bgmVolParam = "BGMVolume";
    [SerializeField] private string _sfxVolParam = "SFXVolume";

    public static AudioManager Instance { get; private set; }

    private AudioSource _activeBGM;
    private AudioSource _inactiveBGM;
    private float _savedBGMTime = 0f;
    private bool _isMuted = false;

    void Awake()
    {
        // Singleton pattern
        //UNITY CALLS AWAKE () USING DYNAMIC BINDING UNITY CHOOSES AT A RUN TIME BUT INSIDE THIS CLASS, ALL METHODS LIKE PLAYSFX()&PLAYBGM ARE STATICALLY BOUND BECAUSE THEY ARE NOT VIRTUAL
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Basic validation (coding standard: validate & log)
        Debug.Assert(_masterMixer != null, "MasterMixer not assigned!");
        Debug.Assert(_bgmGroup != null && _sfxGroup != null, "Mixer groups not assigned!");
        Debug.Assert(_library != null, "SoundLibrary not assigned!");

        // Configure BGM sources
        if (_bgmSourceA == null) _bgmSourceA = CreateBGMSource("BGM_A");
        if (_bgmSourceB == null) _bgmSourceB = CreateBGMSource("BGM_B");

        _activeBGM = _bgmSourceA;
        _inactiveBGM = _bgmSourceB;
    }

    /// Plays a short SFX by id from the library. Includes fallback if missing.
    /// STATIC BINDING: COMPILER ALREADY KNOWS AT COMPILE TIME EHICH VERSION OF SFX TO CALL
    public void PlaySFX(string sfxId, float pitch = 1f, float volume = 1f)
    {
        var clip = _library.GetSFX(sfxId);
        if (clip == null)
        {
            // A01: Missing mapping → fallback + warning
            var fallback = _library.GetFallbackSFX();
            if (fallback != null)
            {
                SpawnOneShot(fallback, pitch, volume);
                Debug.LogWarning($"SFX '{sfxId}' missing. Played fallback.");
            }
            else
            {
                Debug.LogWarning($"SFX '{sfxId}' missing and no fallback set.");
            }
            return;
        }
        SpawnOneShot(clip, pitch, volume);
    }

    /// Starts or switches looping BGM by id (crossfade optional).
    /// STATIC BINDING NO OVERIDING POSSIBLE - ALWAYS CALLS THIS EXACT METHOD.
    public void PlayBGM(string bgmId, float fadeSeconds = 0.75f)
    {
        var clip = _library.GetBGM(bgmId);
        if (clip == null)
        {
            // A02 exception: continue without BGM, log
            Debug.LogWarning($"BGM '{bgmId}' missing. Continuing without BGM.");
            return;
        }

        // Prepare target source
        _inactiveBGM.clip = clip;
        _inactiveBGM.loop = true;
        _inactiveBGM.outputAudioMixerGroup = _bgmGroup;
        _inactiveBGM.Play();

        if (fadeSeconds > 0f && _activeBGM.isPlaying)
        {
            StartCoroutine(CrossfadeBGM(_activeBGM, _inactiveBGM, fadeSeconds));
        }
        else
        {
            _activeBGM.Stop();
            SwapActive();
        }
    }

    /// Boss switch: fade out current and fade in boss track (crossfade).
    public void SwitchToBossBGM(string bossBgmId, float fadeSeconds = 1.0f)
    {
        var clip = _library.GetBGM(bossBgmId);
        if (clip == null)
        {
            // A05 exception: fallback continue
            Debug.LogWarning($"Boss BGM '{bossBgmId}' missing. Continuing current BGM.");
            return;
        }
        PlayBGM(bossBgmId, fadeSeconds);
    }

    /// Adjusts mixer volumes in dB (slider [0..1] mapped to -80..0 dB).
    public void SetBGMVolume01(float value01) => SetLinear01ToDB(_bgmVolParam, value01);
    public void SetSFXVolume01(float value01) => SetLinear01ToDB(_sfxVolParam, value01);

    /// Saves & suspends audio for pause. Resume restores state.
    public void PauseAll()
    {
        _savedBGMTime = _activeBGM.isPlaying ? _activeBGM.time : _savedBGMTime;
        AudioListener.pause = true; // lightweight global pause/duck
    }

    public void ResumeAll()
    {
        AudioListener.pause = false;
        if (_activeBGM.clip != null)
        {
            _activeBGM.time = _savedBGMTime;
            if (!_activeBGM.isPlaying) _activeBGM.Play();
        }
    }

    public void SetMuted(bool muted)
    {
        _isMuted = muted;
        AudioListener.volume = _isMuted ? 0f : 1f;
    }

    // ----------------- Internals -----------------

    private AudioSource CreateBGMSource(string nameTag)
    {
        var go = new GameObject(nameTag);
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = _bgmGroup;
        src.playOnAwake = false;
        src.loop = true;
        src.spatialBlend = 0f;
        return src;
    }

    private void SpawnOneShot(AudioClip clip, float pitch, float volume)
    {
        Debug.Assert(_sfxSourcePrefab != null, "Assign an SFX source prefab (with AudioSource).");
        var src = Instantiate(_sfxSourcePrefab, transform);
        src.outputAudioMixerGroup = _sfxGroup;
        src.pitch = pitch;
        src.volume = volume;
        src.spatialBlend = 0f;
        src.PlayOneShot(clip);
        Destroy(src.gameObject, clip.length + 0.05f);
    }

    private IEnumerator CrossfadeBGM(AudioSource from, AudioSource to, float seconds)
    {
        float t = 0f;
        to.volume = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / seconds);
            from.volume = 1f - k;
            to.volume = k;
            yield return null;
        }
        from.Stop();
        from.volume = 1f;
        to.volume = 1f;
        SwapActive();
    }

    private void SwapActive()
    {
        var temp = _activeBGM;
        _activeBGM = _inactiveBGM;
        _inactiveBGM = temp;
    }

    private void SetLinear01ToDB(string param, float v01)
    {
        // Map 0..1 to -80..0 dB; clamp for safety
        float v = Mathf.Clamp01(v01);
        float dB = Mathf.Lerp(-80f, 0f, v);
        _masterMixer.SetFloat(param, dB);
    }
}
