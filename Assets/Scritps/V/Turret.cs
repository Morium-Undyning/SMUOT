using UnityEngine;

public class SimpleTurret : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float projectileSpeed = 20f;
    
    private Transform currentTarget;
    private float nextFireTime;
    
    void Start()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnStatsChanged += OnStatsChanged;
        }
    }
    
    void Update()
    {
        if (currentTarget == null)
        {
            FindClosestEnemy();
        }
        else
        {
            if (!IsTargetValid())
            {
                currentTarget = null;
                return;
            }
            
            RotateToTarget();
            
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + GetFireRate();
            }
        }
    }
    
    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        float maxRange = GetRange();
        
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null || !enemy.activeSelf) continue;
            
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            
            if (distance < closestDistance && distance <= maxRange)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }
        
        currentTarget = closestEnemy;
    }
    
    bool IsTargetValid()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeSelf) return false;
        
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        return distance <= GetRange();
    }
    
    void RotateToTarget()
    {
        if (currentTarget == null) return;
        
        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0;
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    void Shoot()
    {
        if (firePoint == null || bulletPrefab == null || currentTarget == null) return;
        
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        Projectile projectileScript = bullet.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(currentTarget, GetDamage(), projectileSpeed);
        }
        else
        {
            Destroy(bullet, 5f);
        }
    }
    
    float GetRange()
    {
        return PlayerStats.Instance != null ? PlayerStats.Instance.Range : 20f;
    }
    
    float GetDamage()
    {
        return PlayerStats.Instance != null ? PlayerStats.Instance.Dmg : 10f;
    }
    
    float GetFireRate()
    {
        return 1f;
    }
    
    void OnStatsChanged()
    {
    }
    
    void OnDestroy()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnStatsChanged -= OnStatsChanged;
        }
    }
}