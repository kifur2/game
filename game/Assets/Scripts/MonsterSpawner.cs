using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveContent
    {
        [SerializeField] private int[] monstersAmount;
        public int[] MonstersAmount => monstersAmount;
    }

    public bool infiniteWaves;
    [SerializeField] private GameObject[] monsterSpawnPrefabs;
    [SerializeField] private List<WaveContent> waves;
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
        if (waves != null)
        {
            if (_currentWave >= waves.Count)
            {
                if (infiniteWaves)
                {
                    waves.Add(waves[_currentWave - 1]);
                    for (int i = 0; i < waves[_currentWave].MonstersAmount.Length; i++)
                    {
                        waves[_currentWave].MonstersAmount[i] += waves[_currentWave].MonstersAmount.Length - i;
                        if (Random.Range(0, 1) < 0.3)
                        {
                            waves[_currentWave].MonstersAmount[i] -= 1;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Wave index out of range.");
                    return;
                }
            }


            for (int i = 0; i < waves[_currentWave].MonstersAmount.Length; i++)
            {
                for (int j = 0; j < waves[_currentWave].MonstersAmount[i]; j++)
                {
                    SpawnEnemy(i);
                }
            }
        }
        else
        {
            Debug.LogError("Waves not defined.");
        }
    }

    private void SpawnEnemy(int i)
    {
        GameObject m = Instantiate(monsterSpawnPrefabs[i], FindSpawnLocation(),
            Quaternion.identity, enemiesParentObject);
        m.GetComponent<EnemyBehaviour>().playerTransform = player;
        m.GetComponent<Target>().parentObject = pickupsParentObject;
        m.GetComponent<Target>().spawner = this;
        spawnedMonsters.Add(m);
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