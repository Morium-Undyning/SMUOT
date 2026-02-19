using System;
using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    PlayerStats stats;
    private Health _health;
    
    private void Start()
    {
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        _health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }

    public void AddHealth(float amount)
    {
        stats.MaxHp += amount;
        Debug.Log($"Добавлено {amount} здоровья");
    }
    
    public void AddDamage(float amount)
    {
        stats.Dmg += amount;
        Debug.Log($"Добавлено {amount} урона");
    }
    
    public void AddRange(float amount)
    {
        stats.Range += amount;
        Debug.Log($"Добавлено {amount} дальности");
    }
    public void Heal(float amount)
    {
        _health.Heal(amount);
        Debug.Log($"Восстановлено {amount} здоровья");
    }
    public void AddMiner(float amount)
    {
        MinerManager minerManager = FindAnyObjectByType<MinerManager>();
        Debug.Log(minerManager == null);
        minerManager.SpawnNewMiner();
        Debug.Log($"Добавлен новый рабочий");
    }
}