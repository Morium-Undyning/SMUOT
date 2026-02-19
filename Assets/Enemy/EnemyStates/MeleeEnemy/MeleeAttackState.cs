using UnityEngine;

public class MeleeAttackState : AttackState
{
    private float attackAngle = 60f;
    private GameObject hitEffectPrefab;
    

    public MeleeAttackState(IEnemy enemy, Transform target) : base(enemy, target)
    {
        var meleeEnemy = enemy as MeleeEnemy;
        if (meleeEnemy != null)
        {
            
        }
    }
    
    public void ExecuteMeleeAttack()
    {
        if (target == null || !IsTargetAlive(target)) 
            return;

        Vector3 direction = (target.position - enemy.Transform.position).normalized;
        float angle = Vector3.Angle(enemy.Transform.forward, direction);
        
        if (angle <= attackAngle)
        {
            var health = target.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(enemy.Stats.Damage);
                
                if (hitEffectPrefab != null)
                {
                    GameObject.Instantiate(hitEffectPrefab, target.position, Quaternion.identity);
                }
            }
        }
    }

    private bool IsTargetAlive(Transform target)
    {
        if (target == null) return false;
        var health = target.GetComponent<Health>();
        return health == null || health.CurrentHealth > 0;
    }

    protected override void Attack()
    {
        base.Attack();
        
        Vector3 direction = (target.position - enemy.Transform.position).normalized;
        float angle = Vector3.Angle(enemy.Transform.forward, direction);
        
        if (angle <= attackAngle)
        {
            var health = target.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(enemy.Stats.Damage);
                
                if (hitEffectPrefab != null)
                {
                    GameManager.Instantiate(hitEffectPrefab, target.position, Quaternion.identity);
                }
            }
        }
    }
}