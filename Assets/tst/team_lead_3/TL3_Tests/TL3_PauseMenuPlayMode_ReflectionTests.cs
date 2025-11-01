using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TL3_PauseMenuPlayMode_ReflectionTests
{
    private GameObject host;
    private Component pauseMenu;       // held as Component (no strong type)
    private Type pauseMenuType;        // resolved via reflection

    // -------- helpers (no Assembly-CSharp reference needed) --------
    private static Type FindTypeInAllAssemblies(string simpleName)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            // skip dynamic / test runner internals that can throw
            if (asm.IsDynamic) continue;
            try
            {
                var t = asm.GetTypes().FirstOrDefault(tp => tp.Name == simpleName);
                if (t != null) return t;
            }
            catch { /* ignore reflection-only/unsafe assemblies */ }
        }
        return null;
    }

    private static Component AddComponentByType(GameObject go, Type t)
    {
        return (Component)go.AddComponent(t);
    }

    private static void CallVoid(object target, string methodName)
    {
        var t = target.GetType();
        var m = t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.IsNotNull(m, $"Method '{methodName}' not found on {t.Name}.");
        m.Invoke(target, null);
    }

    private static int CountNamed(string name)
    {
        int c = 0;
        foreach (var go in UnityEngine.Object.FindObjectsOfType<GameObject>())
            if (go.name == name) c++;
        return c;
    }

    // ---------------- test lifecycle ----------------
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Find PauseMenu type at runtime (no compile-time reference)
        pauseMenuType = FindTypeInAllAssemblies("PauseMenu");
        Assert.IsNotNull(pauseMenuType, "Couldn't locate type 'PauseMenu' in loaded assemblies.");

        host = new GameObject("TL3_PauseHost");
        pauseMenu = AddComponentByType(host, pauseMenuType);

        // Let PauseMenu.Start() run (builds UI)
        yield return null;

        // Ensure unpaused before each test
        CallVoid(pauseMenu, "ResumeGame");
        yield return null;
        Assert.AreEqual(1f, Time.timeScale, "Precondition: timeScale must be 1 at start.");
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        var pc = GameObject.Find("PauseCanvas"); if (pc) UnityEngine.Object.DestroyImmediate(pc);
        var es = GameObject.Find("EventSystem"); if (es) UnityEngine.Object.DestroyImmediate(es);
        if (host) UnityEngine.Object.DestroyImmediate(host);

        Time.timeScale = 1f;
        AudioListener.pause = false;
        yield return null;
    }

    // ---------------- Boundary 1 ----------------
    // Repeated Pause does not duplicate UI; timeScale stays 0.
    [UnityTest]
    public IEnumerator Boundary_IdempotentPause_NoDupes_TimescaleZero()
    {
        CallVoid(pauseMenu, "PauseGame");
        yield return null;

        int before = CountNamed("PauseCanvas");

        // call PauseGame again -> should be a no-op wrt duplication
        CallVoid(pauseMenu, "PauseGame");
        yield return null;

        Assert.AreEqual(0f, Time.timeScale, "While paused, timeScale must be 0.");
        Assert.AreEqual(before, CountNamed("PauseCanvas"),
            "PauseCanvas should not duplicate on repeated Pause calls.");

        CallVoid(pauseMenu, "ResumeGame");
        yield return null;
    }

    // ---------------- Boundary 2 ----------------
    // Pausing during motion freezes transform; resume continues cleanly.
    [UnityTest]
    public IEnumerator Boundary_PauseDuringMotion_FreezesAndResumes()
    {
        var mover = new GameObject("TL3_Mover");
        var rb = mover.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(6f, 0f);

        // let it move a few frames
        yield return null; yield return null; yield return null;
        var before = mover.transform.position;

        CallVoid(pauseMenu, "PauseGame");
        yield return new WaitForSecondsRealtime(0.4f); // unscaled -> no motion expected
        var during = mover.transform.position;
        Assert.AreEqual(before, during, "Transform must not change while paused.");

        CallVoid(pauseMenu, "ResumeGame");
        yield return new WaitForSeconds(0.2f); // scaled -> should move
        var after = mover.transform.position;
        Assert.Greater(after.x, during.x, "Object should continue moving after resume.");

        UnityEngine.Object.Destroy(mover);
    }

    // ---------------- Stress ----------------
    // Rapid toggle 50x: no leaks/dupes; end state unpaused and stable.
    [UnityTest]
    public IEnumerator Stress_RapidToggle50_NoLeaks_EndUnpaused()
    {
        for (int i = 0; i < 50; i++)
        {
            CallVoid(pauseMenu, "PauseGame");
            yield return new WaitForSecondsRealtime(0.01f);
            CallVoid(pauseMenu, "ResumeGame");
            yield return null;
        }

        Assert.AreEqual(1f, Time.timeScale, "Should end unpaused (timeScale=1).");
        Assert.LessOrEqual(CountNamed("EventSystem"), 1, "No duplicate EventSystems after stress.");
        Assert.LessOrEqual(CountNamed("PauseCanvas"), 1, "No duplicate PauseCanvas after stress.");

        // idempotent resume
        CallVoid(pauseMenu, "ResumeGame");
        yield return null;
        Assert.AreEqual(1f, Time.timeScale);
    }
}
