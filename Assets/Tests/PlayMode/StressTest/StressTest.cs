using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class StressTest
{
    [UnityTest]
    public IEnumerator PlayerStressTest()
    {
        // Load the scene by name 
        SceneManager.LoadScene("SampleScene");
        yield return new WaitForSeconds(1f); // allow scene to load

        GameObject player = GameObject.Find("Player");
        Assert.IsNotNull(player, "Player not found — make sure 'Player' object exists in SampleScene");

        int cloneCount = 250;
        for (int i = 0; i < cloneCount; i++)
        {
            GameObject clone = Object.Instantiate(player);
            clone.name = $"PlayerClone_{i}";
            clone.transform.position = new Vector3(Random.Range(-30, 30), Random.Range(0, 10), 0);
        }

        yield return new WaitForSeconds(2f); // let clones settle

        int totalPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
        Debug.Log($"Spawned {totalPlayers} player objects in stress test");

        Assert.GreaterOrEqual(totalPlayers, cloneCount, "Not all player clones were created");
    }

    [UnityTest]
    public IEnumerator PhysicsOverloadStressTest()
    {
        Debug.Log("Starting heavy physics stress test...");

        for (int i = 0; i < 200; i++)
        {
            GameObject cube = new GameObject($"Cube_{i}");
            cube.AddComponent<BoxCollider2D>();
            Rigidbody2D rb = cube.AddComponent<Rigidbody2D>();
            rb.AddForce(Random.insideUnitCircle * 2000f);
        }

        yield return new WaitForSeconds(5);
        Debug.Log("Physics stress test complete — no crash or freeze detected.");
        Assert.Pass();
    }
}
