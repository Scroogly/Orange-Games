/********************************************
 * File: SoundLibrary.cs
 * Author: Orange Games Team
 * Description: ScriptableObject lookups for SFX/BGM by id,
 * including a fallback SFX clip.
 ********************************************/
using UnityEngine;

[System.Serializable]
public class SfxEntry
{
    public string id;
    public AudioClip clip;
}

[System.Serializable]
public class BgmEntry
{
    public string id;
    public AudioClip clip;
}

[CreateAssetMenu(menuName = "Audio/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Header("SFX")]
    [SerializeField] private SfxEntry[] _sfx;
    [SerializeField] private AudioClip _fallbackSfx;

    [Header("BGM")]
    [SerializeField] private BgmEntry[] _bgm;

    /// Returns mapped SFX clip by id or null.
    public AudioClip GetSFX(string id)
    {
        foreach (var e in _sfx)
        {
            if (e != null && e.id == id) return e.clip;
        }
        return null;
    }

    /// Returns mapped BGM clip by id or null.
    public AudioClip GetBGM(string id)
    {
        foreach (var e in _bgm)
        {
            if (e != null && e.id == id) return e.clip;
        }
        return null;
    }

    /// Fallback SFX if a mapping is missing (A01).
    public AudioClip GetFallbackSFX() => _fallbackSfx;
}
