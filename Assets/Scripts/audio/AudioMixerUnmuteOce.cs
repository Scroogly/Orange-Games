using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerUnmuteOnce : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private string bgmParam = "BGMVolume";
    [SerializeField] private string sfxParam = "SFXVolume";

    // run once at scene start, then remove itself
    void Start()
    {
        if (masterMixer == null) { Debug.LogError("Assign MasterMixer!"); return; }

        // set exposed params to 0 dB
        masterMixer.SetFloat(bgmParam, 0f);
        masterMixer.SetFloat(sfxParam, 0f);

        // also unpause/unmute globally (just in case)
        AudioListener.pause = false;
        AudioListener.volume = 1f;

        Debug.Log("AudioMixerUnmuteOnce: set BGM/SFX to 0 dB.");
        Destroy(this); // remove after doing its job
    }
}
