using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] bool isPlayer = false;
    [SerializeField] public float maxHealth = 100f;
    private float _currentHealth;
    
    public bool IsAlive => _currentHealth > 0;
    public float CurrentHealth => _currentHealth;

    public UnityEvent onHealed;
    public UnityEvent onDamaged;
    public UnityEvent onDeath;
    public UnityEvent<float> onHealthChange;
    
    private void Start()
    {
        InitializeHealth();
        
        if (isPlayer && PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnStatsChanged += OnPlayerStatsChanged;
        }
    }

    private void InitializeHealth()
    {
        if (isPlayer && PlayerStats.Instance != null)
        {
            maxHealth = PlayerStats.Instance.MaxHp;
        }
        
        _currentHealth = maxHealth;
        onHealthChange?.Invoke(_currentHealth);
    }

    private void OnPlayerStatsChanged()
    {
        if (isPlayer && PlayerStats.Instance != null)
        {
            UpdateMaxHealth(PlayerStats.Instance.MaxHp);
        }
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;
        
        _currentHealth -= amount;
        _currentHealth = Mathf.Max(_currentHealth, 0);
        
        onDamaged?.Invoke();
        onHealthChange?.Invoke(_currentHealth);
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (!IsAlive) return;
        
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, maxHealth);
        
        onHealed?.Invoke();
        onHealthChange?.Invoke(_currentHealth);
    }

    private void Die()
    {
        onDeath?.Invoke();
        if (isPlayer)
        {
        }
            
    }

    public void UpdateMaxHealth(float newMaxHealth)
    {
        float healthPercentage = _currentHealth / maxHealth;
        maxHealth = newMaxHealth;
        _currentHealth = maxHealth * healthPercentage;
        onHealthChange?.Invoke(_currentHealth);
    }

    private void OnDestroy()
    {
        if (isPlayer && PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnStatsChanged -= OnPlayerStatsChanged;
        }
    }
}