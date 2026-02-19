using UnityEngine;
using UnityEngine.AI;

public interface IEnemy
{
    Transform Transform { get; }
    GameObject GameObject { get; }
    NavMeshAgent Agent { get; }
    EnemyStats Stats { get; }
    Transform CurrentTarget { get; set; }
    Transform MainTarget { get; set; }
    Animator Animator { get; set; }
    
    
    void TakeDamage(float amount);
    void SetState(IState newState);
    void DetectTargets();
}
