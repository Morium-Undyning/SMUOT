using UnityEngine;
using System.Collections.Generic;
using Enemy.EnemyVariants;
using UnityEngine.AI;

[System.Serializable]
public class EnemyStats
{
    public float Damage = 10f;
    public float MoveSpeed = 3.5f;
    public float AttackRange = 2f;
    public float AttackCooldown = 1f;
    public float DetectionRange = 15f;
    public float SecondaryTargetRange = 5f;
    public float MaxHealth = 100f;
}

public abstract class BaseEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] protected EnemyStats baseStats = new EnemyStats();
    protected EnemyStats currentStats = new EnemyStats();

    Health healthComponent;
    [SerializeField] protected LayerMask targetLayer;

    [SerializeField] protected Transform mainTarget;

    protected NavMeshAgent agent;
    protected Transform currentTarget;
    protected IState currentState;

    public Transform Transform => transform;
    public GameObject GameObject => gameObject;
    public NavMeshAgent Agent => agent;
    public EnemyStats Stats => currentStats;
    public Animator animator;

    public Transform CurrentTarget
    {
        get => currentTarget;
        set => currentTarget = value;
    }

    public Animator Animator { get => animator; set => animator = value; }

    public Transform MainTarget
    {
        get => mainTarget;
        set => mainTarget = value;
    }

    protected virtual void Awake()
    {
        // Копируем базовые статы в текущие
        CopyStats(baseStats, currentStats);

        healthComponent = GetComponent<Health>();
        healthComponent.onDeath.AddListener(Die);

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            agent = gameObject.AddComponent<NavMeshAgent>();

        ApplyCurrentStats();

        if (mainTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                mainTarget = player.transform;
        }
    }

    protected virtual void Start()
    {
        if (mainTarget != null)
        {
            currentTarget = mainTarget;
            SetState(new ChaseState(this, mainTarget));
        }
        else
        {
            Invoke("FindInitialTarget", 0.5f);
        }
    }

    protected virtual void Update()
    {
        currentState?.Update();
    }

    public void PerformActualAttack()
    {
        if (currentTarget == null || !IsTargetAlive(currentTarget))
            return;

        if (currentState is RangedAttackState rangedState)
        {
            rangedState.ExecuteRangedAttack();
            Debug.Log(2);

        }
        else if (currentState is MeleeAttackState meleeState)
        {
            meleeState.ExecuteMeleeAttack();
        }
        else if (currentState is AttackState attackState)
        {
            attackState.ExecuteAttack();
        }
    }

    private void FindInitialTarget()
    {
        if (mainTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                mainTarget = player.transform;
                currentTarget = mainTarget;
                SetState(new ChaseState(this, mainTarget));
            }
        }
    }

    public virtual void TakeDamage(float amount)
    {
        if (healthComponent != null)
        {
            healthComponent.TakeDamage(amount);
        }
    }

    public virtual void SetState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public virtual void DetectTargets()
    {
        if (currentTarget != null && IsTargetAlive(currentTarget))
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);

            if (distance <= currentStats.AttackRange)
            {
                if (currentState is ChaseState)
                {
                    bool isRangedEnemy = gameObject.GetComponent<RangedEnemy>() != null;
                    if (isRangedEnemy)
                    {
                        SetState(new RangedAttackState(this, currentTarget));
                    }
                    else
                    {
                        SetState(new AttackState(this, currentTarget));
                    }
                }
            }
            else
            {
                if (!(currentState is ChaseState))
                {
                    SetState(new ChaseState(this, currentTarget));
                }
            }
            return;
        }

        Transform foundTarget = FindNearestTarget();

        if (foundTarget != null)
        {
            currentTarget = foundTarget;
            float distance = Vector3.Distance(transform.position, foundTarget.position);

            if (distance <= currentStats.AttackRange)
            {
                bool isRangedEnemy = gameObject.GetComponent<RangedEnemy>() != null;
                if (isRangedEnemy)
                {
                    SetState(new RangedAttackState(this, foundTarget));
                }
                else
                {
                    SetState(new AttackState(this, foundTarget));
                }
            }
            else
            {
                SetState(new ChaseState(this, foundTarget));
            }
        }
        else
        {
            currentTarget = null;
            if (agent != null)
            {
                agent.isStopped = true;
            }
        }
    }

    private Transform FindNearestTarget()
    {
        if (mainTarget != null && IsTargetAlive(mainTarget))
        {
            return mainTarget;
        }

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            currentStats.DetectionRange,
            targetLayer
        );

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            Transform potentialTarget = collider.transform;

            if (IsTargetAlive(potentialTarget))
            {
                float distance = Vector3.Distance(transform.position, potentialTarget.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = potentialTarget;
                }
            }
        }

        return closestTarget;
    }

    protected virtual bool IsTargetAlive(Transform target)
    {
        if (target == null) return false;

        var health = target.GetComponent<Health>();
        return health == null || health.CurrentHealth > 0;
    }

    public void Die()
    {
        ResourcesSystem.Instance.AddCristall(1);
        Destroy(gameObject, 0.1f);
    }

    protected virtual void ApplyCurrentStats()
    {
        if (agent != null)
        {
            agent.speed = currentStats.MoveSpeed;
            agent.stoppingDistance = currentStats.AttackRange - 0.1f;
        }

        if (healthComponent != null)
        {
            healthComponent.maxHealth = Mathf.RoundToInt(currentStats.MaxHealth);
            //healthComponent.CurrentHealth = healthComponent.maxHealth;
        }
    }

    public virtual void ScaleStats(float multiplier)
    {
        currentStats.Damage = baseStats.Damage * multiplier;
        currentStats.AttackRange = baseStats.AttackRange;
        currentStats.MaxHealth = baseStats.MaxHealth * multiplier;
        currentStats.SecondaryTargetRange = baseStats.SecondaryTargetRange;

        ApplyCurrentStats();
    }

    protected void CopyStats(EnemyStats source, EnemyStats destination)
    {
        destination.Damage = source.Damage;
        destination.MoveSpeed = source.MoveSpeed;
        destination.AttackRange = source.AttackRange;
        destination.AttackCooldown = source.AttackCooldown;
        destination.DetectionRange = source.DetectionRange;
        destination.SecondaryTargetRange = source.SecondaryTargetRange;
        destination.MaxHealth = source.MaxHealth;
    }

    public virtual void ResetStats()
    {
        CopyStats(baseStats, currentStats);
        ApplyCurrentStats();
    }

    public EnemyStats GetBaseStats()
    {
        return baseStats;
    }
}