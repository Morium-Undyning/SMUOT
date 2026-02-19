using UnityEngine;

public class ResGeneration : MonoBehaviour
{
    [SerializeField] private GameObject resPrefabs;
    [SerializeField] private float spawnInterval = 3f;
    private float currentTimer = 0f;

    [SerializeField] private float minSpawnRadius = 3f;
    [SerializeField] private float maxSpawnRadius = 10f;

    [SerializeField] private int maxTotalResources = 20;
    private int currentResourcesCount = 0;

    [SerializeField] private bool spawnAroundPlayer = true;
    [SerializeField] private Transform spawnCenter;

    [Header("Настройки спавна ресурсов")]
    [SerializeField] private float resourceHeightOffset = 0.2f;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask resourceLayer;
    [SerializeField] private int maxSpawnAttempts = 30;

    private Transform player;
    private Transform resourcesContainer;
    private MapGenerator mapGenerator;

    void Start()
    {
        if (spawnAroundPlayer)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                spawnAroundPlayer = false;
            }
        }

        mapGenerator = FindFirstObjectByType<MapGenerator>();

        GameObject container = new GameObject("GeneratedResources");
        resourcesContainer = container.transform;

        if (groundLayer == 0)
            groundLayer = LayerMask.GetMask("Ground");
        if (resourceLayer == 0)
            resourceLayer = LayerMask.GetMask("Resources");

        currentTimer = spawnInterval;
    }

    void Update()
    {
        Generate();
    }

    void Generate()
    {
        if (currentTimer <= 0)
        {
            if (currentResourcesCount < maxTotalResources)
            {
                TrySpawnResource();
            }
            currentTimer = spawnInterval;
        }
        else
        {
            currentTimer -= Time.deltaTime;
        }
    }

    void TrySpawnResource()
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            if (TryGenerateResource())
                return;
        }

    }

    bool TryGenerateResource()
    {
        if (currentResourcesCount >= maxTotalResources)
            return false;

        Vector3 centerPoint = GetSpawnCenter();
        Vector3 spawnPosition = GetRandomPositionAroundCenter(centerPoint);

        if (GetSurfaceHeight(ref spawnPosition))
        {
            if (!IsPositionOccupied(spawnPosition))
            {
                SpawnResource(spawnPosition);
                return true;
            }
        }

        return false;
    }

    Vector3 GetSpawnCenter()
    {
        if (spawnAroundPlayer && player != null)
            return player.position;
        else if (spawnCenter != null)
            return spawnCenter.position;
        else
            return transform.position;
    }

    Vector3 GetRandomPositionAroundCenter(Vector3 center)
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minSpawnRadius, maxSpawnRadius);

        return new Vector3(
            center.x + randomCircle.x * distance,
            center.y + 10f,
            center.z + randomCircle.y * distance
        );
    }

    bool GetSurfaceHeight(ref Vector3 position)
    {
        if (mapGenerator != null)
        {
            var tileData = mapGenerator.GetTileAtWorldPosition(position);
            if (tileData != null)
            {
                position.y = tileData.height + resourceHeightOffset;
                return true;
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 20f, groundLayer))
        {
            position.y = hit.point.y + resourceHeightOffset;
            return true;
        }

        return false;
    }

    bool IsPositionOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, checkRadius, resourceLayer);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Resources") ||
                collider.gameObject.GetComponent<ResourceTracker>() != null)
            {
                return true;
            }
        }

        return false;
    }

    void SpawnResource(Vector3 position)
    {
        GameObject newResource = Instantiate(resPrefabs, position, Quaternion.identity, resourcesContainer);

        newResource.tag = "Resources";

        ResourceTracker tracker = newResource.AddComponent<ResourceTracker>();
        tracker.Initialize(this);

        currentResourcesCount++;
    }

    public void OnResourceDestroyed()
    {
        if (currentResourcesCount > 0)
        {
            currentResourcesCount--;
        }
    }

    public int GetCurrentResourceCount() => currentResourcesCount;
    public int GetMaxResourceCount() => maxTotalResources;
    public float GetSpawnProgress() => (float)currentResourcesCount / maxTotalResources;

    public bool CanSpawnMoreResources() => currentResourcesCount < maxTotalResources;

    public class ResourceTracker : MonoBehaviour
    {
        private ResGeneration generator;
        private bool isDestroyed = false;

        public void Initialize(ResGeneration gen)
        {
            generator = gen;
        }

        void OnDestroy()
        {
            if (!isDestroyed && generator != null && gameObject.scene.isLoaded)
            {
                isDestroyed = true;
                generator.OnResourceDestroyed();
            }
        }

        void OnApplicationQuit()
        {
            isDestroyed = true;
        }
    }
}