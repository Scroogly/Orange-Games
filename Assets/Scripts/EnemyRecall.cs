using UnityEngine;

public class EnemyRespawn : MonoBehaviour
{
    [Header("Where the enemy returns if it falls off")]
    public Transform respawnPoint;

    [Header("How far below the map counts as 'fallen'?")]
    public float fallThresholdY = -10f;

    EnemyController controller;

    void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    void Update()
    {
        if (transform.position.y < fallThresholdY)
        {
            RespawnEnemy();
        }
    }

    void RespawnEnemy()
    {
       
        transform.position = respawnPoint.position;

        controller.Start();  
    }
}
