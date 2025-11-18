using NUnit.Framework;
using UnityEngine;

public class HealthBoundaryTester
{
    private PlayerHealth _playerHealth;
    private GameObject _playerObject;

    [SetUp]
    public void Setup()
    {
        _playerObject = new GameObject("PlayerHealthTestObject");
        _playerHealth = _playerObject.AddComponent<PlayerHealth>();

        _playerHealth.SetMaxHealthPreserveCurrent(100);
    }

    [Test]
    public void Heal_DoesNotExceedMaxHealth()
    {
        _playerHealth.ApplyDamage(50);
        Assert.AreEqual(50, _playerHealth.CurrentHealth);

        _playerHealth.Heal(100);
        Assert.AreEqual(100, _playerHealth.CurrentHealth);

        _playerHealth.Heal(10);
        Assert.AreEqual(100, _playerHealth.CurrentHealth);
    }

    [Test]
    public void TakeDamage_DoesNotGoBelowZero()
    {
        _playerHealth.ApplyDamage(150);
        Assert.AreEqual(0, _playerHealth.CurrentHealth);

        _playerHealth.ApplyDamage(10);
        Assert.AreEqual(0, _playerHealth.CurrentHealth);
    }
}