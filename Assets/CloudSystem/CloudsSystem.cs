using UnityEngine;
using System.Collections;

public class CloudsSystem : MonoBehaviour
{
    [SerializeField] private Sprite[] cloudsPrefabs;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float minY = 2f;
    [SerializeField] private float maxY = 5f;
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.5f;
    
    [SerializeField] private float spawnX = 12f;
    [SerializeField] private float destroyX = -12f;
    
    [SerializeField] private int initialCloudCount = 5;
    [SerializeField] private float minInitialX = -8f;
    [SerializeField] private float maxInitialX = 8f;
    
    void Start()
    {
        SpawnInitialClouds();
        StartCoroutine(SpawnClouds());
    }
    
    void SpawnInitialClouds()
    {
        for (int i = 0; i < initialCloudCount; i++)
        {
            SpawnSingleCloud(true);
        }
    }
    
    IEnumerator SpawnClouds()
    {
        while (true)
        {
            SpawnSingleCloud(false);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    void SpawnSingleCloud(bool isInitial)
    {
        if (cloudsPrefabs == null || cloudsPrefabs.Length == 0)
        {
            return;
        }
        
        GameObject cloud = new GameObject("Cloud");
        
        MeshRenderer meshRenderer = cloud.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = cloud.AddComponent<MeshFilter>();
        
        Sprite sprite = cloudsPrefabs[Random.Range(0, cloudsPrefabs.Length)];
        
        Mesh mesh = new Mesh();
        mesh.vertices = System.Array.ConvertAll(sprite.vertices, i => (Vector3)i);
        mesh.uv = sprite.uv;
        mesh.triangles = System.Array.ConvertAll(sprite.triangles, i => (int)i);
        
        meshFilter.mesh = mesh;
        
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        meshRenderer.receiveShadows = true;
        
        float randomY = Random.Range(minY, maxY);
        float randomX = isInitial ? Random.Range(minInitialX, maxInitialX) : spawnX;
        cloud.transform.position = new Vector3(randomX, 35, randomY);
        
        float randomScale = Random.Range(minScale, maxScale);
        cloud.transform.localScale = new Vector3(randomScale, randomScale, 1);
        
        Color cloudColor = Color.white;
        cloudColor.a = Random.Range(0.6f, 0.9f);
        meshRenderer.material.color = cloudColor;
        
        CloudMovement movement = cloud.AddComponent<CloudMovement>();
        movement.moveSpeed = Random.Range(minSpeed, maxSpeed);
        movement.destroyX = destroyX;
        
        cloud.transform.rotation = Quaternion.Euler(90, 0, 0);
        cloud.transform.SetParent(transform);
        
    }
}

public class CloudMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float destroyX = -12f;
    
    void Update()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        
        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}