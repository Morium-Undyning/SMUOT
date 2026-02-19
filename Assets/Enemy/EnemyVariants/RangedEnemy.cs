using UnityEngine;

namespace Enemy.EnemyVariants
{
    public class RangedEnemy : BaseEnemy
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float projectileSpeed = 15f;

        public GameObject ProjectilePrefab => projectilePrefab;
        public Transform FirePoint => firePoint;
        public float ProjectileSpeed => projectileSpeed;

        protected override void Awake()
        {
            base.Awake();
        
            if (agent != null)
            {
                agent.stoppingDistance = currentStats.AttackRange * 0.8f;
            }
        }
    }
}
