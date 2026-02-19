using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    [SerializeField] private float attackAngle = 60f;
    [SerializeField] private float pushForce = 5f;
    [SerializeField] private GameObject hitEffectPrefab;

    protected override void Awake()
    {
        base.Awake();
    }
}
