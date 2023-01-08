using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [Tooltip("The object that is spawned at every interval.")]
    [SerializeField] private GameObject objectToSpawn;

    [Tooltip("How much time in seconds between each spawn.")] [Range(0f, 60f)]
    [SerializeField] private float timeBetweenSpawns = 5;
    
    [Tooltip("A random number between 0 and this is added to the time between each spawn.")] [Range(0f, 60f)]
    [SerializeField] private float extraRandomTime;
    
    [Tooltip("How far away from the spawner will the entity spawn")]
    [SerializeField] private float spawnRadius = 1f;

    public float minRadius = 0f;

    [Tooltip("If this spawner has this many entities, it will not spawn any more. Set to 0 to always spawn.")]
    [SerializeField] private int maxEntities;

    [Tooltip("The chance to spawn the entity.")] [Range(0f, 1f)] [SerializeField]
    private float spawnChance = 0.8f;
    
    [Tooltip("How many entities will attempt to spawn on start.")] [SerializeField]
    private int startingEntities = 5;

    public bool onlySpawnOnStart;

    private float timeUntilNextSpawn;

    private void OnEnable()
    {
        ResetTimer();
        for (int i = 0; i < startingEntities; i++)
        {
            if (Random.Range(0f, 1f) <= spawnChance) SpawnEntity();
        }
    }

    private void Update()
    {
        if (onlySpawnOnStart) return;
        
        timeUntilNextSpawn -= Time.deltaTime;

        if (!(timeUntilNextSpawn <= 0)) return;
        
        ResetTimer();

        if ((maxEntities != 0 && transform.childCount >= maxEntities) || Random.Range(0f, 1f) > spawnChance) return;
        
        SpawnEntity();
    }

    private void SpawnEntity()
    {
        Instantiate(objectToSpawn, transform.position + GetRandomPointInCircle(minRadius, spawnRadius), Quaternion.identity, transform);
    }

    private Vector3 GetRandomPointInCircle(float min, float radius)
    {
        float angle = Random.Range(0, Mathf.PI+Mathf.PI);
        float r = Random.Range(min, radius);
        
        return new Vector3(Mathf.Cos(angle)*r, Mathf.Sin(angle)*r, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.DrawWireSphere(transform.position, minRadius);
    }

    private void ResetTimer()
    {
        timeUntilNextSpawn = timeBetweenSpawns + Random.Range(0, extraRandomTime);
    }
}
