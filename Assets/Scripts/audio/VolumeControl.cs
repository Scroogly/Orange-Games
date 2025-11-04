using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [Header("Mixer & Sliders")]
    public AudioMixer mixer;                 // MainMixer asset
    public Slider masterSlider, musicSlider, sfxSlider;

    // Convert 0..1 slider to decibels for mixer
    float ToDb(float v) => v <= 0.0001f ? -80f : Mathf.Log10(v) * 20f;

    void Start()
    {
        float m  = PlayerPrefs.GetFloat("vol.master", 0.8f);
        float mu = PlayerPrefs.GetFloat("vol.music" , 0.8f);
        float sx = PlayerPrefs.GetFloat("vol.sfx"   , 0.8f);

        if (masterSlider) { masterSlider.value = m;  SetMaster(m); }
        if (musicSlider)  { musicSlider.value  = mu; SetMusic(mu); }
        if (sfxSlider)    { sfxSlider.value    = sx; SetSFX(sx); }
    }

    public void SetMaster(float v)
    {
        if (mixer) mixer.SetFloat("MasterVol", ToDb(v));
        PlayerPrefs.SetFloat("vol.master", v);
    }

    public void SetMusic(float v)
    {
        if (mixer) mixer.SetFloat("MusicVol", ToDb(v));
        PlayerPrefs.SetFloat("vol.music", v);
    }

    public void SetSFX(float v)
    {
        if (mixer) mixer.SetFloat("SFXVol", ToDb(v));
        PlayerPrefs.SetFloat("vol.sfx", v);
    }
}
