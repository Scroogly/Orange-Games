/********************************************
 * File: AudioSettingsUI.cs
 * Author: Orange Games Team
 * Description: Binds UI sliders/toggles to mixer volumes
 * and mute, including simple persistence.
 ********************************************/
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;   // 0..1
    [SerializeField] private Slider _sfxSlider;   // 0..1
    [SerializeField] private Toggle _muteToggle;

    const string PREF_BGM = "pref_bgm";
    const string PREF_SFX = "pref_sfx";
    const string PREF_MUTE = "pref_mute";

    void Start()
    {
        float bgm = PlayerPrefs.GetFloat(PREF_BGM, 0.7f);
        float sfx = PlayerPrefs.GetFloat(PREF_SFX, 0.7f);
        bool mute = PlayerPrefs.GetInt(PREF_MUTE, 0) == 1;

        _bgmSlider.SetValueWithoutNotify(bgm);
        _sfxSlider.SetValueWithoutNotify(sfx);
        _muteToggle.SetIsOnWithoutNotify(mute);

        OnBGMChanged(bgm);
        OnSFXChanged(sfx);
        OnMuteToggled(mute);
    }

    /// Called by UI slider
    public void OnBGMChanged(float value01)
    {
        AudioManager.Instance.SetBGMVolume01(value01);
        PlayerPrefs.SetFloat(PREF_BGM, value01);
    }

    /// Called by UI slider
    public void OnSFXChanged(float value01)
    {
        AudioManager.Instance.SetSFXVolume01(value01);
        PlayerPrefs.SetFloat(PREF_SFX, value01);
    }

    /// Called by UI toggle
    public void OnMuteToggled(bool muted)
    {
        AudioManager.Instance.SetMuted(muted);
        PlayerPrefs.SetInt(PREF_MUTE, muted ? 1 : 0);
    }

    public void OnPausePressed()  => AudioManager.Instance.PauseAll();
    public void OnResumePressed() => AudioManager.Instance.ResumeAll();
}
