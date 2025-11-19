// EnemyPlayModeTests.cs
// 5 PlayMode tests that run over multiple frames in real time.

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public class EnemyPlayModeTests
{
    // Helper: create enemy at runtime (EnemyController + optional EnemyShooter).
    GameObject CreateEnemy(bool withShooter = false)
    {
        var go = new GameObject("RuntimeEnemy");
        var sr = go.AddComponent<SpriteRenderer>();
        var controller = go.AddComponent<EnemyController>();
        controller.sprite = sr;

        if (withShooter)
        {
            go.AddComponent<EnemyShooter>();
        }

        return go;
    }

    // Helper: create bullet prefab (EnemyBullet).
    GameObject CreateBulletPrefab()
    {
        var bullet = new GameObject("EnemyBulletPrefab");
        bullet.AddComponent<SpriteRenderer>();
        bullet.AddComponent<Rigidbody2D>();
        bullet.AddComponent<EnemyBullet>();
        return bullet;
    }

    // 1) Test: enemy actually patrols back and forth between its bounds over time.
    [UnityTest]
    public IEnumerator Enemy_PatrolsBetweenBounds_OverTime()
    {
        var enemy = CreateEnemy();
        var controller = enemy.GetComponent<EnemyController>();

        controller.leftOffset = -1f;
        controller.rightOffset = 1f;
        controller.speed = 2f;

        // Let Awake/Start run.
        yield return null;

        float startX = enemy.transform.position.x;
        float leftBound  = startX - 1f;
        float rightBound = startX + 1f;

        bool reachedLeft = false;
        bool reachedRight = false;

        // Simulate 3 seconds of game time.
        float endTime = Time.time + 3f;
        while (Time.time < endTime)
        {
            yield return null;

            float x = enemy.transform.position.x;
            if (x <= leftBound + 0.05f) reachedLeft = true;
            if (x >= rightBound - 0.05f) reachedRight = true;
        }

        Assert.IsTrue(reachedLeft && reachedRight,
            "Enemy should reach both patrol bounds over time.");

        Object.Destroy(enemy);
    }

    // 2) Test: enemy stays upright even when physics (gravity) is applied.
    [UnityTest]
    public IEnumerator Enemy_RotationStaysUpright_WithPhysics()
    {
        var enemy = CreateEnemy();
        var rb = enemy.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.freezeRotation = false;

        // Let Unity run Awake/Start/LateUpdate.
        yield return null;

        // Simulate falling + updates for 1 second.
        float endTime = Time.time + 1f;
        while (Time.time < endTime)
        {
            yield return null;
        }

        // LateUpdate in EnemyController should have kept rotation at identity.
        Assert.AreEqual(Quaternion.identity, enemy.transform.rotation,
            "Enemy rotation should stay upright even with physics.");

        Object.Destroy(enemy);
    }

    // 3) Test: shooter fires bullets over time according to fireRate (roughly).
    [UnityTest]
    public IEnumerator Shooter_FiresBulletsOverTime_UsingFireRate()
    {
        // Clear any existing bullets.
        foreach (var b in Object.FindObjectsOfType<EnemyBullet>())
            Object.Destroy(b.gameObject);

        var enemy = CreateEnemy(withShooter: true);
        var shooter = enemy.GetComponent<EnemyShooter>();

        shooter.bulletPrefab = CreateBulletPrefab();

        var firePoint = new GameObject("FirePoint").transform;
        firePoint.parent = enemy.transform;
        firePoint.localPosition = Vector3.zero;
        shooter.firePoint = firePoint;

        shooter.fireRate = 0.5f; // about 2 shots per second

        // Let Awake/Start run.
        yield return null;

        // Run for ~1.6 seconds.
        float endTime = Time.time + 1.6f;
        while (Time.time < endTime)
        {
            yield return null;
        }

        var bullets = Object.FindObjectsOfType<EnemyBullet>();

        // Expect around 3 bullets; allow 2–4 as tolerance.
        Assert.GreaterOrEqual(bullets.Length, 2,
            "Shooter should fire multiple bullets over time.");
        Assert.LessOrEqual(bullets.Length, 4,
            "Shooter should respect fireRate and not spam too many bullets.");

        Object.Destroy(enemy);
        foreach (var b in bullets) Object.Destroy(b.gameObject);
    }

    // 4) Test: a bullet moves forward every frame, not just once.
    [UnityTest]
    public IEnumerator Bullet_MovesForward_ContinuouslyOverFrames()
    {
        var bulletPrefab = CreateBulletPrefab();
        var bulletGO = Object.Instantiate(bulletPrefab);
        var bullet = bulletGO.GetComponent<EnemyBullet>();

        bulletGO.transform.position = Vector3.zero;
        bullet.speed = 5f;
        bullet.SetDirection(1f);

        // Let Start run.
        yield return null;

        float previousX = bulletGO.transform.position.x;

        // Check movement over several frames.
        for (int i = 0; i < 5; i++)
        {
            yield return null;
            float currentX = bulletGO.transform.position.x;
            Assert.Greater(currentX, previousX,
                "Bullet should move farther along +X each frame.");
            previousX = currentX;
        }

        Object.Destroy(bulletGO);
        Object.Destroy(bulletPrefab);
    }

    // 5) Test: a bullet destroys itself after its lifetime expires.
    [UnityTest]
    public IEnumerator Bullet_IsDestroyed_AfterLifetimeExpires()
    {
        var bulletPrefab = CreateBulletPrefab();
        var bulletGO = Object.Instantiate(bulletPrefab);
        var bullet = bulletGO.GetComponent<EnemyBullet>();

        bullet.lifeTime = 0.2f;

        // Let Start run.
        yield return null;

        // Wait longer than lifetime.
        yield return new WaitForSeconds(0.3f);

        // Bullet should be gone.
        var stillThere = Object.FindObjectOfType<EnemyBullet>();
        Assert.IsNull(stillThere, "Bullet should be destroyed after its lifetime.");

        Object.Destroy(bulletPrefab); // prefab only, instance should be gone already
    }
}
