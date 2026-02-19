using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    
    [SerializeField] private List<UpgradeData> allUpgrades = new List<UpgradeData>();
    [SerializeField] private int upgradesToShow = 3;
    
    public UnityEvent<List<UpgradeData>> OnUpgradesReady = new UnityEvent<List<UpgradeData>>();
    public UnityEvent<UpgradeData> OnUpgradeSelected = new UnityEvent<UpgradeData>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void GenerateRandomUpgrades()
    {
        List<UpgradeData> randomUpgrades = new List<UpgradeData>();
        
        for (int i = 0; i < upgradesToShow && i < allUpgrades.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, allUpgrades.Count);
            randomUpgrades.Add(allUpgrades[randomIndex]);
        }
        
        OnUpgradesReady?.Invoke(randomUpgrades);
    }
    
    public void SelectUpgrade(UpgradeData upgrade)
    {
        OnUpgradeSelected?.Invoke(upgrade);
    }
}