using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject Health;
    public GameObject player;                
    public List<float> spawnX;
    public List<Vector3> spawnPositions;

    private int indexSpawnpoint = 0;
    private bool[] hasSpawned;

    public GenericSpawner genericSpawner;

    private void Start()
    {
        hasSpawned = new bool[spawnX.Count];
    }

    private void Update()
    {
        for(int i = 0; i < spawnX.Count; i++)
        {
            if (!hasSpawned[i] && player.transform.position.x > spawnX[i])
            {
                SpawnHealthPickup();
                hasSpawned[i] = true;
                break;
            }
        }
    }

    void SpawnHealthPickup()
    {
        if(spawnPositions.Count == 0 || genericSpawner == null) { return; }
        if (indexSpawnpoint >= spawnPositions.Count) { return; }

        genericSpawner.Spawn(Health, spawnPositions[indexSpawnpoint], Quaternion.identity);
        indexSpawnpoint += 1;

    }
}

