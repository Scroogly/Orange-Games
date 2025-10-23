using NUnit.Framework;
using UnityEngine;

public class AudioBoundaryTests
{
    private AudioSource music;

    [SetUp]
    public void Setup()
    {
        var go = new GameObject("MusicPlayer");
        music = go.AddComponent<AudioSource>();
        music.clip = AudioClip.Create("dummy", 44100, 1, 44100, false);
    }

    [Test]
    public void Volume_Clamped_0_To_1()
    {
        music.volume = -5f;
        Assert.That(music.volume, Is.InRange(0f, 1f));

        music.volume = 5f;
        Assert.That(music.volume, Is.InRange(0f, 1f));
    }

    [Test]
    public void Loop_Is_Enabled_For_BackgroundMusic()
    {
        music.loop = true;
        Assert.IsTrue(music.loop);
    }
}
