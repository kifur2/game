using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private EndlessTerrain endlessTerrain;

    public List<GameObject> spawnedMonsters;
    private int _currentWave;
    [SerializeField] private float spawnRangeMax = 15;
    [SerializeField] private float spawnRangeMin = 4;

    private bool _isSpawningWave = false;


    void Start()
    {
        StartCoroutine(SpawnWaveWithDelay(3f));
    }

    IEnumerator SpawnWaveWithDelay(float delay)
    {
        _isSpawningWave = true;
        yield return new WaitForSeconds(delay);
        SpawnWave();
        _isSpawningWave = false;
    }

    void Update()
    {
        if (!_isSpawningWave && spawnedMonsters.Count == 0)
        {
            _currentWave++;
            StartCoroutine(SpawnWaveWithDelay(3f));
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
        Vector3 position = FindSpawnLocation();
        if (position == Vector3.zero) Debug.LogError("Position is zero");
        GameObject m = Instantiate(monsterSpawnPrefabs[i], position, Quaternion.identity, enemiesParentObject);
        m.GetComponent<EnemyBehaviour>().playerTransform = player;
        m.GetComponent<Target>().parentObject = pickupsParentObject;
        m.GetComponent<Target>().spawner = this;
        spawnedMonsters.Add(m);
        InitializeNavMeshAgent(m);
    }

    void InitializeNavMeshAgent(GameObject monster)
    {
        var navMeshAgent = monster.GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
            StartCoroutine(EnableNavMeshAgent(navMeshAgent));
        }
    }

    IEnumerator EnableNavMeshAgent(NavMeshAgent agent)
    {
        yield return new WaitForEndOfFrame();
        agent.enabled = true;
    }

    Vector3 FindSpawnLocation()
    {
        Vector3 spawnPosition;
        NavMeshHit hit;
        const float heightOffset = 2.0f;

        for (int i = 0; i < 20; i++)
        {
            float distance = Random.Range(spawnRangeMin, spawnRangeMax);
            float angle = Random.Range(0, 2 * Mathf.PI);

            float newX = player.position.x + distance * Mathf.Cos(angle);
            float newZ = player.position.z + distance * Mathf.Sin(angle);
            Vector2 flatPosition = new Vector2(newX, newZ);
            float height;
            try
            {
                height = endlessTerrain.GetHeightAtPosition(flatPosition) +
                         heightOffset;
            }
            catch (KeyNotFoundException)
            {
                Debug.LogWarning("Chunk not found in dictionary for position: " + flatPosition);
                continue;
            }

            spawnPosition = new Vector3(newX, height, newZ);
            if (NavMesh.SamplePosition(spawnPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.Log("Suitable spawn position not found on NavMesh.");
        return new Vector3(player.position.x, player.position.y, player.position.z);
    }
}