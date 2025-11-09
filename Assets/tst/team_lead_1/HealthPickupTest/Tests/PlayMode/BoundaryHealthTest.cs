using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class HealthBoundaryTester
{

    private PlayerHealth playerHealth;

    [OneTimeSetUp]

    public void Setup()
    {
        playerHealth = new PlayerHealth();
    }

    [Test]

    public void Heal_DoesNotExceedMaxHealth()
    {
        playerHealth.SetMaxHealth(100);
        playerHealth.Heal(50);
        Assert.AreEqual(100, playerHealth.CurrentHealth);

        playerHealth.Heal(10);
        Assert.AreEqual(100, playerHealth.CurrentHealth);
    }

    [Test]
    public void TakeDamage_DoesNotGoBelowZero()
    {
        playerHealth.SetMaxHealth(100);
        playerHealth.TakeDamage(150);
        Assert.AreEqual(0, playerHealth.CurrentHealth);

        playerHealth.TakeDamage(10);
        Assert.AreEqual(0, playerHealth.CurrentHealth);
    }
  
}
