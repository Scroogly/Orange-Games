using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerTests
{
    [Test]
    public void PlayerStartsWithGivenHealth()
    {
        Player player = new Player(100);
        Assert.AreEqual(100, player.health);
    }

    [Test]
    public void PlayerTakesDamageCorrectly()
    {
        Player player = new Player(100);
        player.TakeDamage(30);
        Assert.AreEqual(70, player.health);
    }
}

