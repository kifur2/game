using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveContent
    {
        [SerializeField] private GameObject[] monsterSpawn;
        public GameObject[] MonsterSpawn => monsterSpawn;
    }

    [SerializeField] private WaveContent[] waves;
    [SerializeField] private Transform enemiesParentObject;
    [SerializeField] private Transform pickupsParentObject;
    [SerializeField] private Transform player;

    public List<GameObject> spawnedMonsters;
    private int _currentWave;
    [SerializeField] private float spawnRangeMax = 15;
    [SerializeField] private float spawnRangeMin = 4;

    void Start()
    {
        SpawnWave();
    }

    void Update()
    {
        if (spawnedMonsters.Count == 0)
        {
            _currentWave++;
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        if (waves != null && waves.Length > _currentWave)
        {
            foreach (var monster in waves[_currentWave].MonsterSpawn)
            {
                GameObject m = Instantiate(monster, FindSpawnLocation(), Quaternion.identity, enemiesParentObject);
                m.GetComponent<EnemyBehaviour>().playerTransform = player;
                m.GetComponent<Target>().parentObject = pickupsParentObject;
                m.GetComponent<Target>().spawner = this;
                spawnedMonsters.Add(m);
            }
        }
        else
        {
            Debug.LogError("Wave index out of range or waves not defined.");
        }
    }

    Vector3 FindSpawnLocation()
    {
        Vector3 spawnPosition;

        for (int i = 0; i < 10; i++)
        {
            float distance = Random.Range(spawnRangeMin, spawnRangeMax);
            float angle = Random.Range(0, 2 * Mathf.PI);

            float newX = player.position.x + distance * Mathf.Cos(angle);
            float newZ = player.position.z + distance * Mathf.Sin(angle);
            spawnPosition = new Vector3(
                newX,
                player.position.y,
                newZ
            );
            if (Physics.Raycast(spawnPosition, Vector3.down, 5))
                return spawnPosition;
        }

        Debug.Log("Suitable spawn position not found.");

        return transform.position;
    }
}