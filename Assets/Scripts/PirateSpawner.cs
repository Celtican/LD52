using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PirateSpawner : MonoBehaviour
{
    public GameObject piratePrefab;
    public float startTimeBetweenSpawns = 15f;
    public float timeDecreasePerSpawn = 1f;
    public float minTimeBetweenSpawns = 5;
    public float spawnChance = 0.5f;
    public float numEnemiesToSpawn = 1;
    public float increaseEnemiesToSpawnPerWarp = 1f;
    public LayerMask asteroidLayerMask;

    private float curTimeBetweenSpawns;
    private float timeUntilSpawn = 1000000;
    private new Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        timeUntilSpawn -= Time.deltaTime;
        if (timeUntilSpawn <= 0)
        {
            curTimeBetweenSpawns = Mathf.Max(curTimeBetweenSpawns - timeDecreasePerSpawn, minTimeBetweenSpawns);
            timeUntilSpawn = curTimeBetweenSpawns;
            if (Random.Range(0f, 1f) < spawnChance)
            {
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        if (PlayerController.instance == null) return;
        int numEnemies = (int)numEnemiesToSpawn;
        if (Random.Range(0f, 1f) < numEnemiesToSpawn % 1) numEnemies += 1;

        for (int i = 0; i < numEnemies; i++)
        {
            for (int j = 0; j < 10; j++) // attempt to spawn 10 times
            {
                float range = camera.orthographicSize * camera.aspect + 2;

                float angle = Random.Range(0f, Mathf.PI * 2f);

                Vector2 direction = new Vector2(Mathf.Cos(angle) * range, Mathf.Sin(angle) * range);
                Vector3 playerPosition = PlayerController.instance.transform.position;

                ContactFilter2D filter = new()
                {
                    layerMask = asteroidLayerMask
                };
                Debug.DrawRay(playerPosition, direction, Color.red, 1);
                // if there is an asteroid this direction, don't spawn
                RaycastHit2D[] hits = new RaycastHit2D[1];
                int numHits = Physics2D.Raycast(new Vector2(playerPosition.x, playerPosition.y) + direction.normalized*2, direction, filter, hits, range);
                if (numHits > 0) continue;

                Instantiate(piratePrefab, playerPosition + new Vector3(direction.x, direction.y), Quaternion.identity);

                break; // don't try to spawn any more
            }
        }
    }

    public void Reset()
    {
        curTimeBetweenSpawns = startTimeBetweenSpawns;
        timeUntilSpawn = curTimeBetweenSpawns;
        numEnemiesToSpawn += increaseEnemiesToSpawnPerWarp;
    }
}