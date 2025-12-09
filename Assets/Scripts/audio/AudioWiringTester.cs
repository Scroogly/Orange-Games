using UnityEngine;
using UnityEngine.Audio;

public class AudioWiringTester : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;     // assign your MasterMixer
    [SerializeField] private string bgmId = "menu";
    [SerializeField] private string sfxId = "jump";

    void Start()
    {
        Debug.Log("=== AudioWiringTester START ===");

        // Listener sanity , do we have an audio listener in the scene is it paused, what is the volume
        var listener = FindObjectOfType<AudioListener>();
        Debug.Log($"AudioListener found? {listener != null}, pause={AudioListener.pause}, vol={AudioListener.volume}");
        AudioListener.pause = false; AudioListener.volume = 1f;

        // Mixer sanity debug log: Mixed parameteers exist :BGM VOLUME SFX VOLUME
        if (mixer == null) { Debug.LogError("Mixer NOT assigned on tester."); return; }
        float v;
        bool gotBgm = mixer.GetFloat("BGMVolume", out v);
        bool gotSfx = mixer.GetFloat("SFXVolume", out _);
        Debug.Log($"Mixer params present? BGM={gotBgm}, SFX={gotSfx}, BGMVolume={v} dB");
        // THEN SETS THEM TO 0 DB(FULL VOLUME):
        mixer.SetFloat("BGMVolume", 0f);
        mixer.SetFloat("SFXVolume", 0f);

        // AudioManager PREFAB IS MISSING - THIS STOPSpresence
        if (AudioManager.Instance == null) { Debug.LogError("AudioManager.Instance is NULL (not in scene?)."); return; }

        // Try to play BGM WITH NO FADE
        Debug.Log($"Playing BGM id='{bgmId}'");
        AudioManager.Instance.PlayBGM(bgmId, 0f);

        // Try to play SFX JUMP OSUND ONE TIME
        Debug.Log($"Playing SFX id='{sfxId}'");
        AudioManager.Instance.PlaySFX(sfxId);

        // Direct raw one-shot (bypass mixer) to prove device output if needed
        // (assign a clip in inspector to hear this)
        // var src = gameObject.AddComponent<AudioSource>();
        // src.playOnAwake = true; src.outputAudioMixerGroup = null; src.clip = someClip;
    }
}
