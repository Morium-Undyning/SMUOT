using UnityEngine;

public class AttackState : IState
{
    protected IEnemy enemy;
    protected Transform target;
    protected float attackTimer;

    public AttackState(IEnemy enemy, Transform target)
    {
        this.enemy = enemy;
        this.target = target;
    }

    public virtual void Enter()
    {
        
        if (enemy.Agent != null)
        {
            enemy.Agent.isStopped = true;
        }
        attackTimer = enemy.Stats.AttackCooldown;
    }

    public virtual void Update()
    {
        if (target == null)
        {
            enemy.DetectTargets();
            return;
        }

        float distance = Vector3.Distance(enemy.Transform.position, target.position);
        
        if (distance > enemy.Stats.AttackRange)
        {
            enemy.SetState(new ChaseState(enemy, target));
            return;
        }

        if (target != null)
        {
            enemy.Transform.LookAt(new Vector3(target.position.x, enemy.Transform.position.y, target.position.z));
        }

        attackTimer += Time.deltaTime;
        if (attackTimer >= enemy.Stats.AttackCooldown)
        {
            Attack();
            attackTimer = 0f;
        }
    }
    
    public void ExecuteAttack()
    {
        if (target == null || !IsTargetAlive(target))
        {
            enemy.DetectTargets();
            return;
        }

        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(enemy.Stats.Damage);
        }
    }
    
    
    private bool IsTargetAlive(Transform target)
    {
        if (target == null) return false;
        var health = target.GetComponent<Health>();
        return health == null || health.CurrentHealth > 0;
    }

    public virtual void Exit() 
    {
        attackTimer = 0f;
    }

    protected virtual void Attack()
    {
        enemy.Animator.SetTrigger("Attack");
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(enemy.Stats.Damage);
        }
    }
}
