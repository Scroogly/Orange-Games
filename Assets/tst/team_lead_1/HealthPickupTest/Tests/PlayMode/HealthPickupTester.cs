using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class StressTestSpawner
{
    private GenericSpawner spawner;
    private GameObject prefab;

    [UnitySetUp]
    public IEnumerator LoadScene()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;

    }
    [SetUp]
    public void Setup()
    {
        spawner = ScriptableObject.CreateInstance<GenericSpawner>();
        prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }
    [UnityTest]

    public IEnumerator StressSpawn_Prefabs_Continuous()
    {
        int spawnCount = 0;

        while (true)
        {
            GameObject obj = spawner.Spawn(prefab, Random.insideUnitSphere * 5f, Quaternion.identity);
            Assert.IsNotNull(obj);

            spawnCount++;

            if (spawnCount % 1000 == 0)
            {
                Debug.Log($"Spawned {spawnCount} prefabs!");
            }

            yield return null; 
        }
        
    }
}
