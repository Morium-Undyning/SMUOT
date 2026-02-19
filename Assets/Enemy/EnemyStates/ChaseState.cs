using Enemy.EnemyVariants;
using UnityEngine;

public class ChaseState : IState
{
    private IEnemy enemy;
    private Transform target;

    public ChaseState(IEnemy enemy, Transform target)
    {
        this.enemy = enemy;
        this.target = target;
    }

    public void Enter()
    {
        
        if (enemy.Agent != null && target != null)
        {
            enemy.Animator.SetBool("isRunning", true);
            enemy.Agent.isStopped = false;
            enemy.Agent.SetDestination(target.position);
        }
    }

    public void Update()
    {
        if (target == null)
        {
            enemy.DetectTargets();
            return;
        }

        if (enemy.Agent != null && enemy.Agent.isOnNavMesh && target != null)
        {
            enemy.Agent.SetDestination(target.position);
        }

        if (target != null)
        {
            float distance = Vector3.Distance(enemy.Transform.position, target.position);
            
            if (distance <= enemy.Stats.AttackRange)
            {
                bool isRangedEnemy = enemy.GameObject.GetComponent<RangedEnemy>() != null;
                
                if (isRangedEnemy)
                {
                    enemy.SetState(new RangedAttackState(enemy, target));
                }
                else
                {
                    enemy.SetState(new AttackState(enemy, target));
                }
            }
        }
    }

    public void Exit() 
    {
        if (enemy.Agent != null)
        {
            enemy.Agent.isStopped = true;
        }
    }
}
