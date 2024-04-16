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
    [SerializeField] private Transform diamondsParentObject;
    [SerializeField] private Transform player;

    public List<GameObject> spawnedMonsters;
    private int currentWave = 0;
    [SerializeField]
    private float spawnRangeX = 10;
    [SerializeField]
    private float spawnRangeY = 10;

    void Start()
    {
        SpawnWave();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedMonsters.Count == 0)
        {
            currentWave++;
            SpawnWave();
        }
    }
    void SpawnWave()
    {
        if (waves != null && waves.Length > currentWave)
        {
            foreach (var monster in waves[currentWave].MonsterSpawn)
            {
                GameObject m = Instantiate(monster, FindSpawnLocation(), Quaternion.identity, enemiesParentObject);
                m.GetComponent<ZombieBehaviour>().player = player;
                m.GetComponent<Target>().parentObjects[0] = diamondsParentObject;
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
            spawnPosition = new Vector3(
                Random.Range(-spawnRangeX, spawnRangeX) + transform.position.x,
                transform.position.y,
                Random.Range(-spawnRangeY, spawnRangeY) + transform.position.z
            );
            if (Physics.Raycast(spawnPosition, Vector3.down, 5))
                return spawnPosition;
        }

        Debug.Log("Suitable spawn position not found.");
        
        return transform.position; 
        
    }

}
