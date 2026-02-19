using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private GameObject hitEffect;
    
    public float damage;
    private Transform target;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Transform target, float damage, float speed)
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
    
        this.target = target;
        this.damage = damage;
        this.speed = speed;
    
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == target)
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
            
        }
        
    }

}