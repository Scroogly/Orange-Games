/********************************************
 * File: PlayerTest.cs
 * Author: Orange Ninja Team
 * Description: Unit tests for Player data logic.
 ********************************************/

using NUnit.Framework;

/// Contains unit tests for the Player class.
public class PlayerTests
{
    [Test]
    public void PlayerStartsWithGivenHealth()
    {
        Player player = new Player(100);
        Assert.AreEqual(100, player.GetHealth());
    }

    [Test]
    public void PlayerTakesDamageCorrectly()
    {
        Player player = new Player(100);
        player.TakeDamage(30);
        Assert.AreEqual(70, player.GetHealth());
    }

    [Test]
    public void PlayerHealthDoesNotGoBelowZero()
    {
        Player player = new Player(50);
        player.TakeDamage(100);
        Assert.AreEqual(0, player.GetHealth());
    }

    [Test]
    public void PlayerIsAliveReturnsTrueWhenHealthAboveZero()
    {
        Player player = new Player(20);
        Assert.IsTrue(player.IsAlive());
    }

    [Test]
    public void PlayerIsAliveReturnsFalseWhenHealthIsZero()
    {
        Player player = new Player(10);
        player.TakeDamage(10);
        Assert.IsFalse(player.IsAlive());
    }
}
