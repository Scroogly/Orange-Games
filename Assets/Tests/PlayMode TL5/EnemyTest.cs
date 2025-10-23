using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyControllerPlayModeTests
{
    private GameObject enemyGO;
    private EnemyController controller;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        enemyGO = new GameObject("Enemy");
        enemyGO.transform.position = Vector2.zero;   // startX = 0
        controller = enemyGO.AddComponent<EnemyController>();

        // Make the patrol small & quick so tests run fast/stable
        controller.leftOffset  = -2f;
        controller.rightOffset =  2f;
        controller.speed       =  6f;   // fast enough to hit bounds quickly
        // leave turnTolerance as default

        // Let Start() run and initialize leftX/rightX
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(enemyGO);
        // Wait a frame so Unity can actually destroy it
        yield return null;
    }

    /// <summary>
    /// Enemy should reach the right bound, flip, reach the left bound, flip back.
    /// </summary>
    [UnityTest]
    public IEnumerator Reaches_Both_Bounds_And_Turns_Back()
    {
        const float tol = 0.08f;     // generous tolerance to account for frame timing
        const float runSeconds = 3f; // enough for multiple traversals

        float expectedLeft  = enemyGO.transform.position.x + controller.leftOffset;   // -2
        float expectedRight = enemyGO.transform.position.x + controller.rightOffset;  // +2

        float elapsed = 0f;
        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;

        bool turnedAtRight = false;
        bool turnedAtLeft  = false;

        float prevX = enemyGO.transform.position.x;

        while (elapsed < runSeconds)
        {
            yield return null; // advance one frame
            elapsed += Time.deltaTime;

            float x = enemyGO.transform.position.x;
            minX = Mathf.Min(minX, x);
            maxX = Mathf.Max(maxX, x);

            float dx = x - prevX; // sign gives us the current direction

            // If we're close to the right bound and moving left now, we flipped at right
            if (!turnedAtRight && Mathf.Abs(x - expectedRight) <= tol && dx < 0f)
                turnedAtRight = true;

            // If we're close to the left bound and moving right now, we flipped at left
            if (!turnedAtLeft && Mathf.Abs(x - expectedLeft) <= tol && dx > 0f)
                turnedAtLeft = true;

            prevX = x;
        }

        // 1) We actually reached both bounds (within tolerance)
        Assert.That(maxX, Is.InRange(expectedRight - tol, expectedRight + tol),
            $"Did not reach the RIGHT bound. maxX={maxX:F3}, expected≈{expectedRight:F3}");
        Assert.That(minX, Is.InRange(expectedLeft - tol, expectedLeft + tol),
            $"Did not reach the LEFT bound. minX={minX:F3}, expected≈{expectedLeft:F3}");

        // 2) We flipped direction at both bounds
        Assert.IsTrue(turnedAtRight, "Did not flip direction at the RIGHT bound.");
        Assert.IsTrue(turnedAtLeft,  "Did not flip direction at the LEFT bound.");
    }

    /// <summary>
    /// Enemy should never wander outside its configured patrol range.
    /// </summary>
    [UnityTest]
    public IEnumerator Stays_Within_Configured_Range()
    {
        const float tol = 0.02f;
        float expectedLeft  = enemyGO.transform.position.x + controller.leftOffset;   // -2
        float expectedRight = enemyGO.transform.position.x + controller.rightOffset;  // +2

        // Run for some time and constantly assert the position is in range (with small tolerance)
        float elapsed = 0f;
        const float runSeconds = 2.0f;

        while (elapsed < runSeconds)
        {
            yield return null;
            elapsed += Time.deltaTime;

            float x = enemyGO.transform.position.x;
            Assert.That(x, Is.GreaterThanOrEqualTo(expectedLeft - tol)
                         .And.LessThanOrEqualTo(expectedRight + tol),
                $"Enemy left patrol range at x={x:F3}. Expected [{expectedLeft:F3}, {expectedRight:F3}]");
        }
    }

    /// <summary>
    /// Swapped offsets (e.g., leftOffset greater than rightOffset) should still work.
    /// </summary>
    [UnityTest]
    public IEnumerator Handles_Swapped_Offsets()
    {
        // Reconfigure on the same object (simple)
        controller.leftOffset  =  3f;   // (intentionally wrong order)
        controller.rightOffset = -3f;

        // Restart logic by re-running Start() (simulate re-init)
        controller.SendMessage("Start");
        yield return null;

        float runSeconds = 1.5f;
        float elapsed = 0f;

        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;

        while (elapsed < runSeconds)
        {
            yield return null;
            elapsed += Time.deltaTime;

            float x = enemyGO.transform.position.x;
            minX = Mathf.Min(minX, x);
            maxX = Mathf.Max(maxX, x);
        }

        // Since controller normalizes min/max internally using Mathf.Min/Max,
        // the patrol should remain symmetrical around startX (0) at [-3, +3]
        Assert.That(minX, Is.LessThanOrEqualTo(-2.9f), "Left traversal not reached with swapped offsets.");
        Assert.That(maxX, Is.GreaterThanOrEqualTo( 2.9f), "Right traversal not reached with swapped offsets.");
    }
}
