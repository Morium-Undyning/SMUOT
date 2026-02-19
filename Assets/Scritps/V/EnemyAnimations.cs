using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    public BaseEnemy enemy;
    
    public void Attack()
    {
        Debug.Log(1);
        if (enemy != null)
        {
            Debug.Log(2);
            
            BaseEnemy baseEnemy = enemy;
            if (baseEnemy != null)
            {
                Debug.Log(3);
                
                baseEnemy.PerformActualAttack();
            }
        }
    }
}
