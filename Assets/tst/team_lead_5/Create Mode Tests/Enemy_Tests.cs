// EnemyTests.cs
// 25 simple tests for EnemyController, EnemyShooter, and EnemyBullet.

using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

public class EnemyTests
{
    // ---------- Helpers ------------------------------------------------

    // Creates an enemy with EnemyController (and optional EnemyShooter).
    GameObject CreateEnemy(bool withShooter = false)
    {
        var go = new GameObject("TestEnemy");

        var sr = go.AddComponent<SpriteRenderer>();
        var controller = go.AddComponent<EnemyController>();
        controller.sprite = sr;

        if (withShooter)
        {
            go.AddComponent<EnemyShooter>();
        }

        controller.Awake();  // EnemyController methods must be public
        controller.Start();
        return go;
    }

    // Creates a simple bullet prefab with EnemyBullet attached.
    GameObject CreateBulletPrefab()
    {
        var bullet = new GameObject("EnemyBulletPrefab");
        bullet.AddComponent<SpriteRenderer>();
        bullet.AddComponent<Rigidbody2D>();
        bullet.AddComponent<EnemyBullet>();
        return bullet;
    }

    // ---------- EnemyController tests (movement / patrol) --------------

    [Test]
    public void Enemy_HasSpriteRendererAfterAwake()
    {
        // Test: Awake assigns a SpriteRenderer reference.
        var go = new GameObject("EnemyNoSprite");
        var sr = go.AddComponent<SpriteRenderer>();
        var controller = go.AddComponent<EnemyController>();

        controller.Awake();
        Assert.IsNotNull(controller.sprite);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Enemy_DefaultSpeedPositive()
    {
        // Test: default speed should be > 0.
        var enemy = CreateEnemy();
        var controller = enemy.GetComponent<EnemyController>();

        Assert.Greater(controller.speed, 0f);
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Enemy_DefaultPatrolRangeNonZero()
    {
        // Test: default left/right offsets should not be equal.
        var enemy = CreateEnemy();
        var controller = enemy.GetComponent<EnemyController>();

        Assert.AreNotEqual(controller.leftOffset, controller.rightOffset);
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Enemy_KeepsSameY_AfterSingleUpdate()
    {
        // Test: Y position stays the same after one Update.
        var enemy = CreateEnemy();
        var controller = enemy.GetComponent<EnemyController>();

        float startY = enemy.transform.position.y;
        controller.Update();

        Assert.AreEqual(startY, enemy.transform.position.y, 0.0001f);
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Enemy_YStaysConstant_ManyUpdates()
    {
        // Test: Y position stays the same over many Updates.
        var enemy = CreateEnemy();
        var controller = enemy.GetComponent<EnemyController>();

        float startY = enemy.transform.position.y;
        for (int i = 0; i < 100; i++)
        {
            controller.Update();
        }

        Assert.AreEqual(startY, enemy.transform.position.y, 0.0001f);
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Enemy_MovesHorizontally_WhenSpeedPositive()
    {
        // Test: enemy moves on X when speed > 0.
        var enemy = CreateEnemy();
        var controller = enemy.GetComponent<EnemyController>();

        controller.speed = 2f;
        float startX = enemy.transform.position.x;
        controller.Update();

        Assert.AreNotEqual(startX, enemy.transform.position.x);
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Enemy_DoesNotMove_WhenSpeedZero()
    {
        // Test: enemy does not move when speed = 0.
        var enemy = CreateEnemy();
        var controller = enemy.GetComponent<EnemyController>();

        controller.speed = 0f;
        Vector3 startPos = enemy.transform.position;
        controller.Update();

        Assert.AreEqual(startPos, enemy.transform.position);
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Enemy_FlipsScale_WhenReachingRightBound()
    {
        // Test: enemy flips scale.x when hitting right bound.
        var enemy = CreateEnemy();
        var controller = enemy.GetComponent<EnemyController>();

        controller.leftOffset = -3f;
        controller.rightOffset = 3f;
        controller.Start();

        enemy.transform.localScale = Vector3.one; // facing right
        enemy.transform.position = new Vector2(
            enemy.transform.position.x + controller.rightOffset,
            enemy.transform.position.y);

        controller.Update();

        Assert.Less(enemy.transform.localScale.x, 0f);
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Enemy_DefaultMovesRight_FirstUpdate()
    {
        // Test: by default the first motion is to the right.
        var enemy = CreateEnemy();
        var controller = enemy.GetComponent<EnemyController>();

        float startX = enemy.transform.position.x;
        controller.Update();

        Assert.GreaterOrEqual(enemy.transform.position.x, startX);
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Enemy_LateUpdate_ResetsRotation()
    {
        // Test: LateUpdate sets rotation back to identity.
        var enemy = CreateEnemy();
        var controller = enemy.GetComponent<EnemyController>();

        enemy.transform.rotation = Quaternion.Euler(0, 0, 45f);
        controller.LateUpdate();

        Assert.AreEqual(Quaternion.identity, enemy.transform.rotation);
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Enemy_HandlesSwappedOffsets_StillFlips()
    {
        // Test: enemy still flips when leftOffset > rightOffset.
        var enemy = CreateEnemy();
        var controller = enemy.GetComponent<EnemyController>();

        controller.leftOffset = 4f;
        controller.rightOffset = -2f;
        controller.Start();

        enemy.transform.localScale = Vector3.one;
        enemy.transform.position = new Vector2(
            enemy.transform.position.x + controller.leftOffset,
            enemy.transform.position.y);

        controller.Update();

        Assert.Less(enemy.transform.localScale.x, 0f);
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Enemy_StartDoesNotChangeInitialX()
    {
        // Test: Start does not move the enemy horizontally.
        var enemy = CreateEnemy();
        var controller = enemy.GetComponent<EnemyController>();

        float startX = enemy.transform.position.x;
        controller.Start();

        Assert.AreEqual(startX, enemy.transform.position.x, 0.0001f);
        Object.DestroyImmediate(enemy);
    }

    // ---------- EnemyShooter tests -------------------------------------

    [Test]
    public void Shooter_SafelyHandlesMissingBulletPrefab()
    {
        // Test: Update does nothing when bulletPrefab is null.
        var enemy = CreateEnemy(withShooter: true);
        var shooter = enemy.GetComponent<EnemyShooter>();

        shooter.bulletPrefab = null;
        shooter.firePoint = enemy.transform;

        Assert.DoesNotThrow(() => shooter.Update());
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Shooter_SafelyHandlesMissingFirePoint()
    {
        // Test: Update does nothing when firePoint is null.
        var enemy = CreateEnemy(withShooter: true);
        var shooter = enemy.GetComponent<EnemyShooter>();

        shooter.bulletPrefab = CreateBulletPrefab();
        shooter.firePoint = null;

        Assert.DoesNotThrow(() => shooter.Update());
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Shooter_SafelyHandlesMissingBoth()
    {
        // Test: Update does nothing when both fields are null.
        var enemy = CreateEnemy(withShooter: true);
        var shooter = enemy.GetComponent<EnemyShooter>();

        shooter.bulletPrefab = null;
        shooter.firePoint = null;

        Assert.DoesNotThrow(() => shooter.Update());
        Object.DestroyImmediate(enemy);
    }

    [Test]
    public void Shooter_SpawnsBullet_AtFirePoint()
    {
        // Test: Shoot spawns bullet at the firePoint position.
        // Clear any leftover bullets.
        foreach (var b in Object.FindObjectsOfType<EnemyBullet>())
            Object.DestroyImmediate(b.gameObject);

        var enemy = CreateEnemy(withShooter: true);
        var shooter = enemy.GetComponent<EnemyShooter>();

        shooter.bulletPrefab = CreateBulletPrefab();
        var firePoint = new GameObject("FirePoint").transform;
        firePoint.parent = enemy.transform;
        firePoint.localPosition = new Vector3(0.5f, 0.2f, 0f);
        shooter.firePoint = firePoint;

        shooter.Shoot();

        var bullet = Object.FindObjectOfType<EnemyBullet>();
        Assert.IsNotNull(bullet);
        Assert.AreEqual(firePoint.position, bullet.transform.position);

        Object.DestroyImmediate(enemy);
        if (bullet != null) Object.DestroyImmediate(bullet.gameObject);
    }

    [Test]
    public void Shooter_BulletFacesRight_WhenEnemyFacesRight()
    {
        // Test: bullet sprite faces right when enemy faces right.
        foreach (var b in Object.FindObjectsOfType<EnemyBullet>())
            Object.DestroyImmediate(b.gameObject);

        var enemy = CreateEnemy(withShooter: true);
        var shooter = enemy.GetComponent<EnemyShooter>();

        shooter.bulletPrefab = CreateBulletPrefab();
        var firePoint = new GameObject("FirePoint").transform;
        firePoint.parent = enemy.transform;
        firePoint.localPosition = Vector3.zero;
        shooter.firePoint = firePoint;

        enemy.transform.localScale = Vector3.one; // face right
        shooter.Shoot();

        var bullet = Object.FindObjectOfType<EnemyBullet>();
        Assert.IsNotNull(bullet);
        Assert.GreaterOrEqual(bullet.transform.localScale.x, 0f);

        Object.DestroyImmediate(enemy);
        if (bullet != null) Object.DestroyImmediate(bullet.gameObject);
    }

    [Test]
    public void Shooter_BulletFacesLeft_WhenEnemyFacesLeft()
    {
        // Test: bullet sprite faces left when enemy faces left.
        foreach (var b in Object.FindObjectsOfType<EnemyBullet>())
            Object.DestroyImmediate(b.gameObject);

        var enemy = CreateEnemy(withShooter: true);
        var shooter = enemy.GetComponent<EnemyShooter>();

        shooter.bulletPrefab = CreateBulletPrefab();
        var firePoint = new GameObject("FirePoint").transform;
        firePoint.parent = enemy.transform;
        firePoint.localPosition = Vector3.zero;
        shooter.firePoint = firePoint;

        enemy.transform.localScale = new Vector3(-1f, 1f, 1f); // face left
        shooter.Shoot();

        var bullet = Object.FindObjectOfType<EnemyBullet>();
        Assert.IsNotNull(bullet);
        Assert.LessOrEqual(bullet.transform.localScale.x, 0f);

        Object.DestroyImmediate(enemy);
        if (bullet != null) Object.DestroyImmediate(bullet.gameObject);
    }

    [Test]
    public void Shooter_ShootCalledMultipleTimes_SpawnsSameCountBullets()
    {
        // Test: calling Shoot n times spawns n bullets.
        foreach (var b in Object.FindObjectsOfType<EnemyBullet>())
            Object.DestroyImmediate(b.gameObject);

        var enemy = CreateEnemy(withShooter: true);
        var shooter = enemy.GetComponent<EnemyShooter>();

        shooter.bulletPrefab = CreateBulletPrefab();
        var firePoint = new GameObject("FirePoint").transform;
        firePoint.parent = enemy.transform;
        firePoint.localPosition = Vector3.zero;
        shooter.firePoint = firePoint;

        int shots = 3;
        for (int i = 0; i < shots; i++)
            shooter.Shoot();

        var bullets = Object.FindObjectsOfType<EnemyBullet>();
        Assert.AreEqual(shots, bullets.Length);

        Object.DestroyImmediate(enemy);
        foreach (var b in bullets) Object.DestroyImmediate(b.gameObject);
    }

    [Test]
    public void FirePoint_MovesWithEnemy()
    {
        // Test: firePoint stays at same offset when enemy moves.
        var enemy = CreateEnemy(withShooter: true);
        var shooter = enemy.GetComponent<EnemyShooter>();

        var firePoint = new GameObject("FirePoint").transform;
        firePoint.parent = enemy.transform;
        firePoint.localPosition = new Vector3(0.5f, 0f, 0f);
        shooter.firePoint = firePoint;

        Vector3 startOffset = firePoint.position - enemy.transform.position;
        enemy.transform.position += new Vector3(2f, 0f, 0f);

        Vector3 newOffset = firePoint.position - enemy.transform.position;
        Assert.AreEqual(startOffset, newOffset);

        Object.DestroyImmediate(enemy);
    }

    // ---------- EnemyBullet tests --------------------------------------

    [Test]
    public void Bullet_MovesRight_WhenDirectionPositive()
    {
        // Test: bullet moves to the right when direction is positive.
        var bulletGO = CreateBulletPrefab();
        var bullet = bulletGO.GetComponent<EnemyBullet>();

        bulletGO.transform.position = Vector3.zero;
        bullet.speed = 5f;
        bullet.SetDirection(1f);
        bullet.Update();

        Assert.Greater(bulletGO.transform.position.x, 0f);
        Object.DestroyImmediate(bulletGO);
    }

    [Test]
    public void Bullet_MovesLeft_WhenDirectionNegative()
    {
        // Test: bullet moves to the left when direction is negative.
        var bulletGO = CreateBulletPrefab();
        var bullet = bulletGO.GetComponent<EnemyBullet>();

        bulletGO.transform.position = Vector3.zero;
        bullet.speed = 5f;
        bullet.SetDirection(-1f);
        bullet.Update();

        Assert.Less(bulletGO.transform.position.x, 0f);
        Object.DestroyImmediate(bulletGO);
    }

    [Test]
    public void Bullet_DefaultLifetimePositive()
    {
        // Test: bullet lifetime is greater than zero.
        var bulletPrefab = CreateBulletPrefab();
        var bullet = bulletPrefab.GetComponent<EnemyBullet>();

        Assert.Greater(bullet.lifeTime, 0f);
        Object.DestroyImmediate(bulletPrefab);
    }

    [Test]
    public void Bullet_UsesSpeedForMovement()
    {
        // Test: higher speed moves the bullet farther in one Update.
        var bulletGO1 = CreateBulletPrefab();
        var b1 = bulletGO1.GetComponent<EnemyBullet>();
        bulletGO1.transform.position = Vector3.zero;
        b1.speed = 2f;
        b1.SetDirection(1f);
        b1.Update();
        float dist1 = bulletGO1.transform.position.x;

        var bulletGO2 = CreateBulletPrefab();
        var b2 = bulletGO2.GetComponent<EnemyBullet>();
        bulletGO2.transform.position = Vector3.zero;
        b2.speed = 6f;
        b2.SetDirection(1f);
        b2.Update();
        float dist2 = bulletGO2.transform.position.x;

        Assert.Greater(dist2, dist1);
        Object.DestroyImmediate(bulletGO1);
        Object.DestroyImmediate(bulletGO2);
    }

    [Test]
    public void BulletPrefab_HasSpriteRenderer()
    {
        // Test: bullet prefab has a SpriteRenderer so it can be visible.
        var bulletPrefab = CreateBulletPrefab();
        var sr = bulletPrefab.GetComponent<SpriteRenderer>();

        Assert.IsNotNull(sr);
        Object.DestroyImmediate(bulletPrefab);
    }
}
