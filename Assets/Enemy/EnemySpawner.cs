using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave 1";
        [TextArea(2, 4)]
        public string waveDescription = "Описание волны...";
        public Sprite waveIcon;
        public GameObject[] addictiveObjects;
        public List<GameObject> enemyPrefabs;
        public int enemyCount = 10;
        public float spawnDelay = 2f;
    }

    [System.Serializable]
    public class WaveEvent : UnityEvent<string, string, Sprite, GameObject[]> { }

    public List<Wave> waves;
    public float spawnRadius = 20f;
    public float deadZoneRadius = 5f;
    public Transform player;
    public WaveEvent onWaveStart;
    public UnityEvent onAllWavesComplete;

    [Header("Бесконечные волны")]
    public bool enableInfiniteWaves = true;
    public List<GameObject> infiniteWaveEnemyPrefabs;
    public int infiniteWaveBaseEnemyCount = 15;
    public int infiniteWaveEnemyIncreasePerWave = 5;
    public float infiniteWaveBaseSpawnDelay = 1.5f;
    public float infiniteWaveMinSpawnDelay = 0.3f;

    [Header("Масштабирование сложности")]
    public float enemyStatsMultiplierPerWave = 0.1f;
    [Tooltip("Максимальный множитель статов")]
    public float maxStatsMultiplier = 3f;

    private int currentWaveIndex = 0;
    private int currentInfiniteWave = 1;
    private bool isSpawning = false;
    private int enemiesAlive = 0;
    private bool infiniteMode = false;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        StartCoroutine(SpawnWaveWithDelay(5));
    }

    IEnumerator SpawnWaveWithDelay(int delay)
    {
        yield return new WaitForSeconds(delay);
        StartNextWave();
    }

    void StartNextWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];
            onWaveStart?.Invoke(currentWave.waveName, currentWave.waveDescription, currentWave.waveIcon, currentWave.addictiveObjects);
            StartCoroutine(SpawnWave(currentWave));
        }
        else if (enableInfiniteWaves && !infiniteMode)
        {
            StartInfiniteWave();
        }
        else
        {
            onAllWavesComplete?.Invoke();
        }
    }

    void StartInfiniteWave()
    {
        infiniteMode = true;
        StartCoroutine(SpawnInfiniteWave());
    }

    IEnumerator SpawnInfiniteWave()
    {
        while (enableInfiniteWaves)
        {
            int enemyCount = infiniteWaveBaseEnemyCount + (currentInfiniteWave - 1) * infiniteWaveEnemyIncreasePerWave;
            float spawnDelay = Mathf.Max(infiniteWaveMinSpawnDelay, infiniteWaveBaseSpawnDelay);

            float statsMultiplier = 1f + (currentInfiniteWave - 1) * enemyStatsMultiplierPerWave;
            statsMultiplier = Mathf.Min(statsMultiplier, maxStatsMultiplier);

            string waveName = $"Бесконечная волна {currentInfiniteWave}";
            string waveDescription = $"Волна {currentInfiniteWave} | Врагов: {enemyCount} | Сила: x{statsMultiplier:F1}";

            onWaveStart?.Invoke(waveName, waveDescription, GetWaveIcon(), null);

            isSpawning = true;

            for (int i = 0; i < enemyCount; i++)
            {
                if (infiniteWaveEnemyPrefabs.Count > 0)
                {
                    GameObject randomEnemy = infiniteWaveEnemyPrefabs[Random.Range(0, infiniteWaveEnemyPrefabs.Count)];
                    SpawnEnemy(randomEnemy, statsMultiplier);
                }

                yield return new WaitForSeconds(spawnDelay);
            }

            isSpawning = false;

            yield return new WaitUntil(() => enemiesAlive <= 0);
            yield return new WaitForSeconds(2f);

            currentInfiniteWave++;
        }
    }

    Sprite GetWaveIcon()
    {
        if (waves.Count > 0)
        {
            return waves[waves.Count - 1].waveIcon;
        }
        return null;
    }

    IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;

        for (int i = 0; i < wave.enemyCount; i++)
        {
            if (wave.enemyPrefabs.Count > 0)
            {
                GameObject randomEnemy = wave.enemyPrefabs[Random.Range(0, wave.enemyPrefabs.Count)];
                SpawnEnemy(randomEnemy, 1f);
            }

            yield return new WaitForSeconds(wave.spawnDelay);
        }

        isSpawning = false;

        yield return new WaitUntil(() => enemiesAlive <= 0);
        yield return new WaitForSeconds(2f);

        currentWaveIndex++;
        StartNextWave();
    }

    void SpawnEnemy(GameObject enemyPrefab, float statsMultiplier)
    {
        Vector3 spawnPos = GetRandomSpawnPosition();

        if (spawnPos != Vector3.zero && enemyPrefab != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            enemiesAlive++;

            BaseEnemy baseEnemy = enemy.GetComponent<BaseEnemy>();
            if (baseEnemy != null)
            {
                if (statsMultiplier > 1f)
                {
                    baseEnemy.ScaleStats(statsMultiplier);
                }

                if (player != null)
                {
                    baseEnemy.MainTarget = player;
                }
            }

            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.onDeath.AddListener(() => OnEnemyDeath());
            }
        }
    }

    void OnEnemyDeath()
    {
        enemiesAlive--;
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (player == null)
        {
            return transform.position;
        }

        for (int i = 0; i < 10; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(deadZoneRadius, spawnRadius);

            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;

            Vector3 spawnPos = player.position + new Vector3(x, 0, z);

            RaycastHit hit;
            if (Physics.Raycast(spawnPos + Vector3.up * 10f, Vector3.down, out hit, 20f))
            {
                spawnPos.y = hit.point.y;
            }

            if (Physics.OverlapSphere(spawnPos, 1f).Length == 0)
            {
                return spawnPos;
            }
        }

        float angle2 = Random.Range(0f, 360f);
        float distance2 = Random.Range(deadZoneRadius, spawnRadius);
        float x2 = Mathf.Cos(angle2 * Mathf.Deg2Rad) * distance2;
        float z2 = Mathf.Sin(angle2 * Mathf.Deg2Rad) * distance2;

        Vector3 finalPos = player.position + new Vector3(x2, 0, z2);

        RaycastHit hit2;
        if (Physics.Raycast(finalPos + Vector3.up * 10f, Vector3.down, out hit2, 20f))
        {
            finalPos.y = hit2.point.y;
        }

        return finalPos;
    }
}