using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace StressSuite.V6
{
    // Runs a 12s visible stress test that spawns enemies in waves, freezes Y, and soft-fails at the end.
    public class EnemyStressPlay_v6
    {
        const string SCENE_NAME = "SampleScene";   // change if needed

        [UnityTest, Timeout(30000)]
        public IEnumerator Run_StressTest()
        {
            SceneManager.LoadScene(SCENE_NAME, LoadSceneMode.Single);
            while (SceneManager.GetActiveScene().name != SCENE_NAME) yield return null;

            var prefab = Resources.Load<GameObject>("Enemy");           // Assets/Resources/Enemy.prefab
            Assert.IsNotNull(prefab, "Missing Assets/Resources/Enemy.prefab");

            const float DURATION = 12f, WAVE_INTERVAL = 2f, Y_TOL = 0.10f, X_TOL = 0.06f;
            const int STEP = 150, MAX = 2000;

            var list = new List<State>(1024);
            float endAt = Time.realtimeSinceStartup + DURATION;
            float nextWave = Time.realtimeSinceStartup;
            string firstError = null;

            while (Time.realtimeSinceStartup < endAt)
            {
                if (Time.realtimeSinceStartup >= nextWave && list.Count < MAX)
                {
                    Spawn(list, prefab, STEP);
                    nextWave += WAVE_INTERVAL;
                }

                if (firstError == null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var s = list[i];
                        if (!s.Go) continue;
                        var p = s.Go.transform.position;

                        if (Mathf.Abs(p.y - s.StartY) > Y_TOL) { firstError = $"Y drift {p.y:F2}"; break; }

                        if (p.x < s.MinX) s.MinX = p.x;
                        if (p.x > s.MaxX) s.MaxX = p.x;

                        if (s.MinX < s.Left - X_TOL || s.MaxX > s.Right + X_TOL)
                        { firstError = "Out of bounds"; break; }
                    }
                }

                yield return null;
            }

            if (firstError != null) Assert.Fail(firstError);
        }

        static void Spawn(List<State> list, GameObject prefab, int add)
        {
            for (int i = 0; i < add; i++)
            {
                var go = Object.Instantiate(
                    prefab,
                    new Vector3(Random.Range(-60f, 60f), Random.Range(-3f, 6f), 0f),
                    Quaternion.identity);

                var rb = go.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    rb.gravityScale = 0f;
                    rb.constraints = RigidbodyConstraints2D.FreezePositionY |
                                     RigidbodyConstraints2D.FreezeRotation;
                }

                var ec = go.GetComponent<EnemyController>() ?? go.AddComponent<EnemyController>();
                float a = Random.Range(-10f, -1f), b = Random.Range(1f, 10f);
                switch ((list.Count + i) % 4)
                {
                    case 0: ec.leftOffset = a; ec.rightOffset = b; break;               // normal
                    case 1: ec.leftOffset = b; ec.rightOffset = -a; break;              // inverted
                    case 2: float r = Random.Range(0.10f, 0.30f); ec.leftOffset = -r; ec.rightOffset = r; break; // tiny
                    default: ec.leftOffset = 0f; ec.rightOffset = 0f; break;            // zero width
                }
                ec.speed = Random.Range(0.05f, 40f);

                float startX = go.transform.position.x;
                list.Add(new State
                {
                    Go = go,
                    StartY = go.transform.position.y,
                    Left  = startX + Mathf.Min(ec.leftOffset, ec.rightOffset),
                    Right = startX + Mathf.Max(ec.leftOffset, ec.rightOffset),
                    MinX = float.MaxValue,
                    MaxX = float.MinValue
                });
            }
        }

        // class (not struct) to allow in-loop updates without foreach issues
        private class State
        {
            public GameObject Go;
            public float StartY, Left, Right, MinX, MaxX;
        }
    }
}
