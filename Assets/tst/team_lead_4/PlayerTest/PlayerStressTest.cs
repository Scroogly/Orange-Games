/********************************************
 * File: PlayerStressTest.cs
 * Author: Safal Shrestha
 * Description: Stress test for spawning large
 *              numbers of player instances.
 ********************************************/

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class StressTest
{
    /// Loads the scene and spawns thousands of players for stress testing.
    [UnityTest]
    public IEnumerator PlayerStressTest()
    {
        SceneManager.LoadScene("SampleScene");

        // Allow scene time to load.
        yield return new WaitForSeconds(1f);

        GameObject player = GameObject.Find("Player");
        Assert.IsNotNull(player, "Player not found in SampleScene");

        int cloneCount = 10000;

        for (int i = 0; i < cloneCount; i++)
        {
            GameObject clone = Object.Instantiate(player);
            clone.name = $"PlayerClone_{i}";
            clone.transform.position = new Vector3(
                Random.Range(-30, 30),
                Random.Range(0, 10),
                0
            );
        }

        yield return new WaitForSeconds(2f);

        int totalPlayers = GameObject.FindGameObjectsWithTag("Player").Length;

        Debug.Log($"Total player objects created: {totalPlayers}");

        Assert.GreaterOrEqual(totalPlayers, cloneCount, "Not all clones were created.");
    }
}
