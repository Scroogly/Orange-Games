/********************************************
 * File: PlayerTest.cs
 * Author: Safal Shrestha
 * Description: Unit tests for player health,
 *              movement, combat, contact
 *              damage, and power-ups.
 ********************************************/

using System.Reflection;
using NUnit.Framework;
using UnityEngine;

/// Tests for PlayerHealth, movement, combat, and power-ups.
public class PlayerComponentTests
{
    // HEALTH TESTS------------------------------

    /// PlayerHealth.TakeDamage should reduce health.
    [Test]
    public void PlayerHealth_TakeDamage_ReducesHealth()
    {
        var go = new GameObject("PlayerHealthTest");
        var health = go.AddComponent<PlayerHealth>();

        health.SetMaxHealthPreserveCurrent(100);
        health.ApplyDamage(35);

        Assert.AreEqual(65, health.CurrentHealth);

        Object.DestroyImmediate(go);
    }

    /// Taking negative damage should do nothing.
    [Test]
    public void PlayerHealth_NegativeDamage_NoEffect()
    {
        var go = new GameObject("HealthNegativeDamage");
        var health = go.AddComponent<PlayerHealth>();

        health.SetMaxHealthPreserveCurrent(100);
        health.ApplyDamage(-20);

        Assert.AreEqual(100, health.CurrentHealth);

        Object.DestroyImmediate(go);
    }

    /// Healing should not exceed max health.
    [Test]
    public void PlayerHealth_Heal_DoesNotExceedMax()
    {
        var go = new GameObject("PlayerHealTest");
        var health = go.AddComponent<PlayerHealth>();

        health.SetMaxHealthPreserveCurrent(50);
        health.ApplyDamage(30);     // HP = 20
        health.Heal(100);           // Should clamp at 50

        Assert.AreEqual(50, health.CurrentHealth);

        Object.DestroyImmediate(go);
    }

    /// Setting max health should clamp current health down if needed.
    [Test]
    public void PlayerHealth_SetMaxHealth_ClampsCurrent()
    {
        var go = new GameObject("HealthClampTest");
        var health = go.AddComponent<PlayerHealth>();

        health.SetMaxHealthPreserveCurrent(100);
        health.ApplyDamage(10);     // HP = 90

        health.SetMaxHealthPreserveCurrent(50); // HP should clamp to 50

        Assert.AreEqual(50, health.CurrentHealth);

        Object.DestroyImmediate(go);
    }

    // MOVEMENT TESTS-----------------------------

    /// Speed multipliers should change the effective movement speed.
    [Test]
    public void PlayerMovementController_SpeedMultiplierChangesSpeed()
    {
        var go = new GameObject("PlayerMoveTest");
        go.AddComponent<Rigidbody2D>();

        var movement = go.AddComponent<PlayerMovementController>();

        typeof(PlayerMovementController)
            .GetField("_moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(movement, 5f);

        movement.SetSpeedMultiplier(2f);

        Assert.AreEqual(10f, movement.GetEffectiveMoveSpeed(), 0.01f);

        Object.DestroyImmediate(go);
    }

    /// Speed multiplier cannot be negative.
    [Test]
    public void PlayerMovementController_SpeedMultiplier_ClampsAtZero()
    {
        var go = new GameObject("SpeedClamp");
        go.AddComponent<Rigidbody2D>();

        var movement = go.AddComponent<PlayerMovementController>();

        movement.SetSpeedMultiplier(-5f);

        Assert.AreEqual(0f, movement.CurrentSpeedMultiplier);

        Object.DestroyImmediate(go);
    }

    // POWER-UP TESTS -----------------------------

    /// Power-up should use dynamic binding stats (50 health, 3x speed).
    [Test]
    public void PlayerPowerUp_DynamicBinding_UsesSubclassStats()
    {
        var go = new GameObject("PowerUpDynamicTest");
        go.AddComponent<Rigidbody2D>();

        var health = go.AddComponent<PlayerHealth>();
        var movement = go.AddComponent<PlayerMovementController>();
        var power = go.AddComponent<PlayerPowerUp>();

        health.SetMaxHealthPreserveCurrent(100);

        typeof(PlayerMovementController)
            .GetField("_moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(movement, 5f);

        // Manually set PowerStatsEnhanced (dynamic binding)
        var powerStats = new PowerStatsEnhanced();
        typeof(PlayerPowerUp)
            .GetField("_powerStats", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(power, powerStats);

        power.ActivatePower();

        // Should use subclass values: +50 health, x3 speed
        Assert.AreEqual(150, health.MaxHealth, "Health should be 100 + 50 = 150");
        Assert.AreEqual(15f, movement.GetEffectiveMoveSpeed(), 0.01f, "Speed should be 5 * 3 = 15");

        power.DeactivatePower();

        Assert.AreEqual(100, health.MaxHealth, "Health should return to 100");
        Assert.AreEqual(5f, movement.GetEffectiveMoveSpeed(), 0.01f, "Speed should return to 5");

        Object.DestroyImmediate(go);
    }

    /// Power-up stats object returns correct dynamic binding values.
    [Test]
    public void PowerStatsEnhanced_ReturnsCorrectValues()
    {
        var stats = new PowerStatsEnhanced();

        Assert.AreEqual(50, stats.GetBonusHealth(), "Dynamic binding should return 50");
        Assert.AreEqual(3f, stats.GetSpeedMultiplier(), 0.01f, "Dynamic binding should return 3");
        Assert.AreEqual(10f, stats.GetDuration(), 0.01f, "Dynamic binding should return 10");
    }

    /// Base stats object returns different values (for static binding).
    [Test]
    public void PowerStatsBase_ReturnsCorrectValues()
    {
        var stats = new PowerStatsBase();

        Assert.AreEqual(100, stats.GetBonusHealth(), "Base should return 100");
        Assert.AreEqual(5f, stats.GetSpeedMultiplier(), 0.01f, "Base should return 5");
        Assert.AreEqual(60f, stats.GetDuration(), 0.01f, "Base should return 60");
    }

    /// Power-up should not activate twice.
    [Test]
    public void PlayerPowerUp_ActivateWhileActive_NoDoubleBoost()
    {
        var go = new GameObject("PowerUpDoubleTest");
        go.AddComponent<Rigidbody2D>();

        var health = go.AddComponent<PlayerHealth>();
        var movement = go.AddComponent<PlayerMovementController>();
        var power = go.AddComponent<PlayerPowerUp>();

        health.SetMaxHealthPreserveCurrent(100);

        var powerStats = new PowerStatsEnhanced();
        typeof(PlayerPowerUp)
            .GetField("_powerStats", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(power, powerStats);

        power.ActivatePower();
        power.ActivatePower(); // Should not stack

        Assert.AreEqual(150, health.MaxHealth, "Health should only boost once to 150");

        Object.DestroyImmediate(go);
    }

    // COMBAT TESTS-----------------------------

    /// Attacking should damage enemies in range.
    [Test]
    public void PlayerCombatController_Attack_DamagesEnemy()
    {
        // Create player
        var player = new GameObject("PlayerCombatTest");
        var combat = player.AddComponent<PlayerCombatController>();

        // Set attack parameters
        typeof(PlayerCombatController)
            .GetField("_attackRange", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(combat, 5f);

        typeof(PlayerCombatController)
            .GetField("_attackDamage", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(combat, 15);

        // Ensure ALL layers are included
        LayerMask allLayers = new LayerMask { value = ~0 };
        typeof(PlayerCombatController)
            .GetField("_enemyLayer", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(combat, allLayers);

        // Create enemy with collider
        var enemy = new GameObject("EnemyTestObj");
        var collider = enemy.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;

        enemy.AddComponent<EnemyController>();
        enemy.AddComponent<EnemyHealthSimple>();
        enemy.transform.position = player.transform.position;

        // Run private method PerformAttack()
        typeof(PlayerCombatController)
            .GetMethod("PerformAttack", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(combat, null);

        // Get enemy health
        var enemyHealth = enemy.GetComponent<EnemyHealthSimple>();

        // Assert 50 -> 35
        Assert.AreEqual(35, enemyHealth.CurrentHealth, "Enemy should take 15 damage.");

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy);
    }

    /// Attacking outside range should not damage enemy.
    [Test]
    public void PlayerCombatController_Attack_OutOfRange_NoDamage()
    {
        var player = new GameObject("PlayerCombatRangeTest");
        var combat = player.AddComponent<PlayerCombatController>();

        typeof(PlayerCombatController)
            .GetField("_attackRange", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(combat, 1f);

        typeof(PlayerCombatController)
            .GetField("_attackDamage", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(combat, 20);

        LayerMask allLayers = new LayerMask { value = ~0 };
        typeof(PlayerCombatController)
            .GetField("_enemyLayer", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(combat, allLayers);

        var enemy = new GameObject("Enemy");
        enemy.AddComponent<CircleCollider2D>();
        enemy.AddComponent<EnemyController>();

        var health = enemy.AddComponent<EnemyHealthSimple>();
        enemy.transform.position = new Vector3(10f, 10f, 0); // Far away

        typeof(PlayerCombatController)
            .GetMethod("PerformAttack", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(combat, null);

        Assert.AreEqual(50, health.CurrentHealth, "Enemy should not take damage out of range");

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy);
    }

    // CONTACT DAMAGE TESTS ---------------------------

    /// PlayerContactDamage should damage player when touching enemy.
    [Test]
    public void PlayerContactDamage_TakesDamageFromEnemy()
    {
        var player = new GameObject("PlayerContactTest");
        var health = player.AddComponent<PlayerHealth>();
        var contact = player.AddComponent<PlayerContactDamage>();

        health.SetMaxHealthPreserveCurrent(100);

        typeof(PlayerContactDamage)
            .GetField("contactDamage", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(contact, 20);

        var enemy = new GameObject("EnemyCollider");
        enemy.AddComponent<BoxCollider2D>();
        enemy.AddComponent<EnemyController>();

        var collider = enemy.GetComponent<Collider2D>();

        typeof(PlayerContactDamage)
            .GetMethod("TryApplyDamage", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(contact, new object[] { collider });

        Assert.AreEqual(80, health.CurrentHealth);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy);
    }

    /// Contact damage should not trigger when touching a non-enemy.
    [Test]
    public void PlayerContactDamage_NoDamageFromNonEnemy()
    {
        var player = new GameObject("PlayerContactNonEnemy");
        var health = player.AddComponent<PlayerHealth>();
        var contact = player.AddComponent<PlayerContactDamage>();

        health.SetMaxHealthPreserveCurrent(100);

        var box = new GameObject("Box");
        box.AddComponent<BoxCollider2D>(); // No EnemyController

        var collider = box.GetComponent<Collider2D>();

        typeof(PlayerContactDamage)
            .GetMethod("TryApplyDamage", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(contact, new object[] { collider });

        Assert.AreEqual(100, health.CurrentHealth);

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(box);
    }

    // WIN CONDITION TESTS ---------------------------

    /// Player should win after defeating all enemies.
    [Test]
    public void PlayerController_DefeatAllEnemies_TriggersWin()
    {
        // Clean up any leftover enemies from previous tests
        foreach (var enemy in Object.FindObjectsOfType<EnemyController>())
        {
            Object.DestroyImmediate(enemy.gameObject);
        }

        var player = new GameObject("PlayerWinTest");
        player.AddComponent<Rigidbody2D>();
        player.AddComponent<PlayerHealth>();
        player.AddComponent<PlayerMovementController>();
        player.AddComponent<PlayerCombatController>();
        var controller = player.AddComponent<PlayerController>();

        // Create enemies AFTER controller exists but BEFORE Awake
        var enemy1 = new GameObject("Enemy1");
        enemy1.AddComponent<EnemyController>();

        var enemy2 = new GameObject("Enemy2");
        enemy2.AddComponent<EnemyController>();

        // Trigger Awake to count enemies
        typeof(PlayerController)
            .GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(controller, null);

        var combat = player.GetComponent<PlayerCombatController>();
        var movement = player.GetComponent<PlayerMovementController>();

        // Simulate defeating both enemies
        typeof(PlayerController)
            .GetMethod("HandleEnemyDefeated", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(controller, new object[] { enemy1.GetComponent<EnemyController>() });

        typeof(PlayerController)
            .GetMethod("HandleEnemyDefeated", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(controller, new object[] { enemy2.GetComponent<EnemyController>() });

        // Check if movement and combat are disabled (win state)
        Assert.IsFalse(movement.enabled, "Movement should be disabled on win");
        Assert.IsFalse(combat.enabled, "Combat should be disabled on win");

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy1);
        Object.DestroyImmediate(enemy2);
    }

    /// Player should not win if any enemies remain.
    [Test]
    public void PlayerController_EnemiesRemaining_NoWin()
    {
        // Clean up any leftover enemies from previous tests
        foreach (var enemy in Object.FindObjectsOfType<EnemyController>())
        {
            Object.DestroyImmediate(enemy.gameObject);
        }

        var player = new GameObject("PlayerNoWinTest");
        player.AddComponent<Rigidbody2D>();
        player.AddComponent<PlayerHealth>();
        player.AddComponent<PlayerMovementController>();
        player.AddComponent<PlayerCombatController>();
        var controller = player.AddComponent<PlayerController>();

        // Create 3 enemies
        var enemy1 = new GameObject("Enemy1");
        enemy1.AddComponent<EnemyController>();

        var enemy2 = new GameObject("Enemy2");
        enemy2.AddComponent<EnemyController>();

        var enemy3 = new GameObject("Enemy3");
        enemy3.AddComponent<EnemyController>();

        // Trigger Awake to count enemies
        typeof(PlayerController)
            .GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(controller, null);

        var combat = player.GetComponent<PlayerCombatController>();
        var movement = player.GetComponent<PlayerMovementController>();

        // Only defeat 2 out of 3 enemies
        typeof(PlayerController)
            .GetMethod("HandleEnemyDefeated", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(controller, new object[] { enemy1.GetComponent<EnemyController>() });

        typeof(PlayerController)
            .GetMethod("HandleEnemyDefeated", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(controller, new object[] { enemy2.GetComponent<EnemyController>() });

        // Movement and combat should still be enabled
        Assert.IsTrue(movement.enabled, "Movement should remain enabled");
        Assert.IsTrue(combat.enabled, "Combat should remain enabled");

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy1);
        Object.DestroyImmediate(enemy2);
        Object.DestroyImmediate(enemy3);
    }

    // POWER-UP EDGE CASES ---------------------------

    /// Power-up duration should be customizable.
    [Test]
    public void PlayerPowerUp_CustomDuration_Works()
    {
        var go = new GameObject("PowerUpDurationTest");
        go.AddComponent<Rigidbody2D>();
        var health = go.AddComponent<PlayerHealth>();
        var movement = go.AddComponent<PlayerMovementController>();
        var power = go.AddComponent<PlayerPowerUp>();

        health.SetMaxHealthPreserveCurrent(100);

        var powerStats = new PowerStatsEnhanced();
        typeof(PlayerPowerUp)
            .GetField("_powerStats", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(power, powerStats);

        // Activate with custom duration of 5 seconds (instead of default 10)
        power.ActivatePower(5f);

        Assert.AreEqual(150, health.MaxHealth, "Power should be active");

        Object.DestroyImmediate(go);
    }

    /// Deactivating an inactive power should have no effect.
    [Test]
    public void PlayerPowerUp_DeactivateWhenInactive_NoEffect()
    {
        var go = new GameObject("PowerUpDeactivateTest");
        go.AddComponent<Rigidbody2D>();
        var health = go.AddComponent<PlayerHealth>();
        var movement = go.AddComponent<PlayerMovementController>();
        var power = go.AddComponent<PlayerPowerUp>();

        health.SetMaxHealthPreserveCurrent(100);

        var powerStats = new PowerStatsEnhanced();
        typeof(PlayerPowerUp)
            .GetField("_powerStats", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(power, powerStats);

        // Try deactivating without activating first
        power.DeactivatePower();

        Assert.AreEqual(100, health.MaxHealth, "Health should remain unchanged");

        Object.DestroyImmediate(go);
    }

    // HEALTH EDGE CASES ---------------------------

    /// Taking lethal damage should set health to zero.
    [Test]
    public void PlayerHealth_LethalDamage_SetsHealthToZero()
    {
        var go = new GameObject("LethalDamageTest");
        var health = go.AddComponent<PlayerHealth>();

        health.SetMaxHealthPreserveCurrent(50);
        health.ApplyDamage(100); // Overkill damage

        Assert.AreEqual(0, health.CurrentHealth, "Health should be exactly 0");
        Assert.IsFalse(health.IsAlive, "Player should not be alive");

        Object.DestroyImmediate(go);
    }

    /// Healing while at full health should have no effect.
    [Test]
    public void PlayerHealth_HealAtFullHealth_NoChange()
    {
        var go = new GameObject("HealFullTest");
        var health = go.AddComponent<PlayerHealth>();

        health.SetMaxHealthPreserveCurrent(100);
        int healed = health.Heal(20);

        Assert.AreEqual(0, healed, "Should return 0 when already at full health");
        Assert.AreEqual(100, health.CurrentHealth);

        Object.DestroyImmediate(go);
    }

    /// Healing a dead player should have no effect.
    [Test]
    public void PlayerHealth_HealWhenDead_NoEffect()
    {
        var go = new GameObject("HealDeadTest");
        var health = go.AddComponent<PlayerHealth>();

        health.SetMaxHealthPreserveCurrent(50);
        health.ApplyDamage(50); // Kill player

        int healed = health.Heal(30);

        Assert.AreEqual(0, healed, "Cannot heal when dead");
        Assert.AreEqual(0, health.CurrentHealth);

        Object.DestroyImmediate(go);
    }

    /// Negative healing should have no effect.
    [Test]
    public void PlayerHealth_NegativeHeal_NoEffect()
    {
        var go = new GameObject("NegativeHealTest");
        var health = go.AddComponent<PlayerHealth>();

        health.SetMaxHealthPreserveCurrent(100);
        health.ApplyDamage(30); // HP = 70

        int healed = health.Heal(-20);

        Assert.AreEqual(0, healed, "Negative heal should return 0");
        Assert.AreEqual(70, health.CurrentHealth, "Health should not change");

        Object.DestroyImmediate(go);
    }

    // MOVEMENT EDGE CASES ---------------------------

    /// Resetting speed multiplier should return to 1.0.
    [Test]
    public void PlayerMovementController_ResetMultiplier_ReturnsToOne()
    {
        var go = new GameObject("ResetMultiplierTest");
        go.AddComponent<Rigidbody2D>();
        var movement = go.AddComponent<PlayerMovementController>();

        movement.SetSpeedMultiplier(5f);
        movement.ResetSpeedMultiplier();

        Assert.AreEqual(1f, movement.CurrentSpeedMultiplier, 0.01f);

        Object.DestroyImmediate(go);
    }

    /// Speed multiplier of zero should make player immobile.
    [Test]
    public void PlayerMovementController_ZeroMultiplier_NoMovement()
    {
        var go = new GameObject("ZeroSpeedTest");
        go.AddComponent<Rigidbody2D>();
        var movement = go.AddComponent<PlayerMovementController>();

        typeof(PlayerMovementController)
            .GetField("_moveSpeed", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(movement, 10f);

        movement.SetSpeedMultiplier(0f);

        Assert.AreEqual(0f, movement.GetEffectiveMoveSpeed(), 0.01f);

        Object.DestroyImmediate(go);
    }

    // COMBAT EDGE CASES ---------------------------

    /// Attacking with zero damage should not affect enemy.
    [Test]
    public void PlayerCombatController_ZeroDamage_NoEffect()
    {
        var player = new GameObject("PlayerZeroDamageTest");
        var combat = player.AddComponent<PlayerCombatController>();

        typeof(PlayerCombatController)
            .GetField("_attackRange", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(combat, 5f);

        typeof(PlayerCombatController)
            .GetField("_attackDamage", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(combat, 0); // Zero damage

        LayerMask allLayers = new LayerMask { value = ~0 };
        typeof(PlayerCombatController)
            .GetField("_enemyLayer", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(combat, allLayers);

        var enemy = new GameObject("Enemy");
        enemy.AddComponent<CircleCollider2D>();
        enemy.AddComponent<EnemyController>();
        var health = enemy.AddComponent<EnemyHealthSimple>();
        enemy.transform.position = player.transform.position;

        typeof(PlayerCombatController)
            .GetMethod("PerformAttack", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(combat, null);

        Assert.AreEqual(50, health.CurrentHealth, "Health should remain at 50");

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy);
    }

    /// Multiple enemies in range should all take damage.
    [Test]
    public void PlayerCombatController_MultipleEnemies_AllTakeDamage()
    {
        var player = new GameObject("PlayerMultiHitTest");
        var combat = player.AddComponent<PlayerCombatController>();

        typeof(PlayerCombatController)
            .GetField("_attackRange", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(combat, 10f);

        typeof(PlayerCombatController)
            .GetField("_attackDamage", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(combat, 10);

        LayerMask allLayers = new LayerMask { value = ~0 };
        typeof(PlayerCombatController)
            .GetField("_enemyLayer", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(combat, allLayers);

        // Create 2 enemies at same position
        var enemy1 = new GameObject("Enemy1");
        enemy1.AddComponent<CircleCollider2D>();
        var health1 = enemy1.AddComponent<EnemyHealthSimple>();
        enemy1.AddComponent<EnemyController>();
        enemy1.transform.position = player.transform.position;

        var enemy2 = new GameObject("Enemy2");
        enemy2.AddComponent<CircleCollider2D>();
        var health2 = enemy2.AddComponent<EnemyHealthSimple>();
        enemy2.AddComponent<EnemyController>();
        enemy2.transform.position = player.transform.position;

        typeof(PlayerCombatController)
            .GetMethod("PerformAttack", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(combat, null);

        Assert.AreEqual(40, health1.CurrentHealth, "Enemy 1 should take damage");
        Assert.AreEqual(40, health2.CurrentHealth, "Enemy 2 should take damage");

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy1);
        Object.DestroyImmediate(enemy2);
    }

    // CONTACT DAMAGE EDGE CASES ---------------------------

    /// Contact damage should respect cooldown period.
    [Test]
    public void PlayerContactDamage_Cooldown_PreventsDamage()
    {
        var player = new GameObject("CooldownTest");
        var health = player.AddComponent<PlayerHealth>();
        var contact = player.AddComponent<PlayerContactDamage>();

        health.SetMaxHealthPreserveCurrent(100);

        typeof(PlayerContactDamage)
            .GetField("contactDamage", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(contact, 10);

        typeof(PlayerContactDamage)
            .GetField("damageCooldown", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(contact, 5f);

        var enemy = new GameObject("Enemy");
        enemy.AddComponent<BoxCollider2D>();
        enemy.AddComponent<EnemyController>();
        var collider = enemy.GetComponent<Collider2D>();

        // First contact - should deal damage
        typeof(PlayerContactDamage)
            .GetMethod("TryApplyDamage", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(contact, new object[] { collider });

        Assert.AreEqual(90, health.CurrentHealth, "First contact should damage");

        // Immediate second contact - should be blocked by cooldown
        typeof(PlayerContactDamage)
            .GetMethod("TryApplyDamage", BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(contact, new object[] { collider });

        Assert.AreEqual(90, health.CurrentHealth, "Second contact should be blocked");

        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy);
    }

    // BINDING DEMONSTRATION TESTS ---------------------------

    /// Static binding should use base class values through base reference.
    [Test]
    public void StaticBinding_UsesBaseClassValues()
    {
        // Declare as base type
        PowerStatsBase statsRef = new PowerStatsBase();

        // Should call base class methods
        Assert.AreEqual(100, statsRef.GetBonusHealth());
        Assert.AreEqual(5f, statsRef.GetSpeedMultiplier(), 0.01f);
        Assert.AreEqual(60f, statsRef.GetDuration(), 0.01f);
    }

    /// Dynamic binding should use subclass values through base reference.
    [Test]
    public void DynamicBinding_UsesSubclassValues()
    {
        // Declare as base type, assign subclass (polymorphism)
        PowerStatsBase statsRef = new PowerStatsEnhanced();

        // Should call subclass methods due to virtual/override
        Assert.AreEqual(50, statsRef.GetBonusHealth());
        Assert.AreEqual(3f, statsRef.GetSpeedMultiplier(), 0.01f);
        Assert.AreEqual(10f, statsRef.GetDuration(), 0.01f);
    }
}