using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    
    public event Action OnStatsChanged;
    
    [SerializeField] private float maxHp;
    [SerializeField] private float dmg;
    [SerializeField] private float range;
    
    public float MaxHp
    {
        get => maxHp;
        set
        {
            maxHp = value;
            OnStatsChanged?.Invoke();
        }
    }
    
    public float Dmg
    {
        get => dmg;
        set
        {
            dmg = value;
            OnStatsChanged?.Invoke();
        }
    }
    
    public float Range
    {
        get => range;
        set
        {
            range = value;
            OnStatsChanged?.Invoke();
        }
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    
    public void NotifyStatsChanged()
    {
        OnStatsChanged?.Invoke();
    }
    
    private void OnDestroy()
    {
        OnStatsChanged = null;
    }
}