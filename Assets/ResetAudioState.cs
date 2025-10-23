using UnityEngine;
public class ResetAudioState : MonoBehaviour
{
    void Awake()
    {
        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }
}
