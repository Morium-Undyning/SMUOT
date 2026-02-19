using System;
using UnityEngine;

[Serializable]
public enum RARITY
{
    Common,
    Rare,
    Epic,
    Legend
}

[Serializable]
public enum ResourceType
{
    Wood,
    Stone,
    Crystal,
    None
}


[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string displayName;
    public string description;
    public Sprite icon;
    public RARITY rarity;
    
    public string idEffect;
    public float value;
    
    public ResourceType resourceType = ResourceType.None;
    public int costAmount = 0;
    
    public bool CanBuy()
    {
        if (resourceType == ResourceType.None || costAmount <= 0)
            return true;
        
        ResourcesSystem rs = FindObjectOfType<ResourcesSystem>();
        if (rs == null) return false;
        
        switch (resourceType)
        {
            case ResourceType.Wood: return rs.Wood >= costAmount;
            case ResourceType.Stone: return rs.Stone >= costAmount;
            case ResourceType.Crystal: return rs.OilCristall >= costAmount;
            default: return true;
        }
    }
    
    public bool Purchase()
    {
        if (!CanBuy()) return false;
        
        if (resourceType != ResourceType.None && costAmount > 0)
        {
            ResourcesSystem rs = FindObjectOfType<ResourcesSystem>();
            if (rs == null) return false;
            
            switch (resourceType)
            {
                case ResourceType.Wood:
                    if (!rs.SubWood(costAmount)) return false;
                    break;
                case ResourceType.Stone:
                    if (!rs.SubStone(costAmount)) return false;
                    break;
                case ResourceType.Crystal:
                    if (!rs.SubCristall(costAmount)) return false;
                    break;
            }
        }
        
        ApplyEffect();
        return true;
    }
    
    private void ApplyEffect()
    {
        PlayerUpgrades playerUpgrades = FindObjectOfType<PlayerUpgrades>();
        if (playerUpgrades != null)
        {
            switch (idEffect)
            {
                case "health":
                    playerUpgrades.AddHealth(value);
                    break;
                case "damage":
                    playerUpgrades.AddDamage(value);
                    break;
                case "range":
                    playerUpgrades.AddRange(value);
                    break;
                case "heal":
                    playerUpgrades.Heal(value);
                    break;
                case "addMiner":
                    playerUpgrades.AddMiner(value);
                    break;
            }
        }
    }
    
    public string GetCostString()
    {
        if (resourceType == ResourceType.None || costAmount <= 0)
            return "Free";
        
        return $"{costAmount} {resourceType}";
    }
}