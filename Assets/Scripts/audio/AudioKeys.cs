using UnityEngine;

public class AudioKeys : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip musicClip;
    public AudioClip sfxClip;

    [Header("Keys")]
    public KeyCode playMusicKey = KeyCode.M;
    public KeyCode stopMusicKey = KeyCode.N;
    public KeyCode sfxKey       = KeyCode.J;

    void Update()
    {
        if (Input.GetKeyDown(playMusicKey))
        {
            if (AudioManager.I.MusicIsPlaying)
            {
                Debug.Log("AudioKeys: toggle stop");
                AudioManager.I.StopMusic();
            }
            else
            {
                Debug.Log("AudioKeys: play music");
                AudioManager.I.PlayMusic(musicClip);
            }
        }

        if (Input.GetKeyDown(stopMusicKey))
        {
            Debug.Log("AudioKeys: stop key pressed");
            AudioManager.I.StopMusic();
        }

        if (Input.GetKeyDown(sfxKey))
        {
            Debug.Log("AudioKeys: play sfx");
            AudioManager.I.PlaySFX(sfxClip);
        }
    }
}
