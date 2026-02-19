using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinerManager : MonoBehaviour
{
    [SerializeField] private GameObject minerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxMiners = 10;
    [SerializeField] private int startMiners = 3;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 3f;
    [SerializeField] private float spawnHeightOffset = 0.5f;
    [SerializeField] private int maxSpawnAttempts = 10;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private List<GameObject> activeMiners = new List<GameObject>();
    [SerializeField] private int currentMinerCount = 0;

    private MapGenerator mapGenerator;

    void Start()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("Player")?.transform;
        mapGenerator = FindFirstObjectByType<MapGenerator>();

        if (groundLayer == 0)
            groundLayer = LayerMask.GetMask("Ground");

        for (int i = 0; i < startMiners; i++)
        {
            SpawnNewMiner();
        }
    }

    public bool SpawnNewMiner()
    {
        if (currentMinerCount >= maxMiners || minerPrefab == null)
            return false;

        Vector3 spawnPosition = GetValidSpawnPosition();

        GameObject newMiner = Instantiate(minerPrefab, spawnPosition, Quaternion.identity);

        // Проверяем и корректируем позицию на NavMesh
        NavMeshAgent agent = newMiner.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, 2f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }

        activeMiners.Add(newMiner);
        currentMinerCount++;

        return true;
    }

    private Vector3 GetValidSpawnPosition()
    {
        if (spawnPoint == null)
            return transform.position;

        // Пытаемся найти валидную позицию для спавна
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            // Random.insideUnitCircle возвращает Vector2 с x и y, используем x для X, y для Z
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;

            // Спавним высоко для последующего рейкаста вниз
            Vector3 testPosition = new Vector3(
                spawnPoint.position.x + randomCircle.x,
                50f, // Высоко над землей
                spawnPoint.position.z + randomCircle.y
            );

            // Получаем высоту земли
            float groundHeight = GetGroundHeight(testPosition);

            if (groundHeight > -1000f) // Валидная высота найдена
            {
                Vector3 finalPosition = new Vector3(
                    testPosition.x,
                    groundHeight + spawnHeightOffset,
                    testPosition.z
                );

                // Проверяем, что позиция на NavMesh
                NavMeshHit navHit;
                if (NavMesh.SamplePosition(finalPosition, out navHit, 1f, NavMesh.AllAreas))
                {
                    return navHit.position;
                }
            }
        }

        // Fallback - спавним прямо у башни с коррекцией высоты
        Vector3 fallbackPos = spawnPoint.position;
        float fallbackHeight = GetGroundHeight(new Vector3(fallbackPos.x, 50f, fallbackPos.z));

        if (fallbackHeight > -1000f)
        {
            fallbackPos.y = fallbackHeight + spawnHeightOffset;

            NavMeshHit fallbackHit;
            if (NavMesh.SamplePosition(fallbackPos, out fallbackHit, 2f, NavMesh.AllAreas))
            {
                return fallbackHit.position;
            }
        }

        return spawnPoint.position;
    }

    private float GetGroundHeight(Vector3 position)
    {
        // Сначала пробуем через MapGenerator
        if (mapGenerator != null)
        {
            var tileData = mapGenerator.GetTileAtWorldPosition(position);
            if (tileData != null)
            {
                return tileData.height;
            }
        }

        // Затем через Raycast
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 100f, groundLayer))
        {
            return hit.point.y;
        }

        return -1000f; // Невалидная высота
    }

    public bool RemoveOneMiner()
    {
        if (currentMinerCount <= 0 || activeMiners.Count == 0)
            return false;

        GameObject minerToRemove = activeMiners[activeMiners.Count - 1];
        activeMiners.RemoveAt(activeMiners.Count - 1);
        currentMinerCount--;

        if (minerToRemove != null)
            Destroy(minerToRemove);

        return true;
    }

    public bool RemoveSpecificMiner(GameObject miner)
    {
        if (miner == null || !activeMiners.Contains(miner))
            return false;

        activeMiners.Remove(miner);
        currentMinerCount--;
        Destroy(miner);

        return true;
    }

    public void RemoveAllMiners()
    {
        for (int i = activeMiners.Count - 1; i >= 0; i--)
        {
            if (activeMiners[i] != null)
                Destroy(activeMiners[i]);
        }

        activeMiners.Clear();
        currentMinerCount = 0;
    }

    public int GetCurrentMinerCount() => currentMinerCount;
    public int GetMaxMinerCount() => maxMiners;
    public List<GameObject> GetAllMiners() => new List<GameObject>(activeMiners);
    public bool CanSpawnMore() => currentMinerCount < maxMiners;

    void Update()
    {
        // Очищаем null-ссылки
        for (int i = activeMiners.Count - 1; i >= 0; i--)
        {
            if (activeMiners[i] == null)
            {
                activeMiners.RemoveAt(i);
                currentMinerCount--;
            }
        }
    }
}