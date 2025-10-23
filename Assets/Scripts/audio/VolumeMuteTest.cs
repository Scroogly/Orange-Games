using System.Collections;
using UnityEngine;

public class VolumeMuteTest : MonoBehaviour
{
    IEnumerator Start()
    {
        // Find the audio source (your background music)
        var src = FindObjectOfType<AudioSource>();
        if (src != null && !src.isPlaying) src.Play();

        // Test 1: Volume = 0
        AudioListener.volume = 0f;
        yield return new WaitForSeconds(1f);

        // Restore normal volume
        AudioListener.volume = 1f;

        // Test 2: Rapid mute/unmute
        for (int i = 0; i < 10; i++)
        {
            AudioListener.pause = !AudioListener.pause;
            yield return new WaitForSeconds(0.05f);
        }

        AudioListener.pause = false;
        Debug.Log("BoundaryTest_Volume done");
    }
}
