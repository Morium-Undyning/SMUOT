using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeSlotScript : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI costText;
    
    private UpgradeData myUpgradeData;
    private bool isPurchased = false;
    
    public event Action<UpgradeData, GameObject> OnCardSelected;
    
    public void Setup(UpgradeData upgradeData)
    {
        myUpgradeData = upgradeData;
        
        if (iconImage != null) iconImage.sprite = upgradeData.icon;
        if (nameText != null) nameText.text = upgradeData.displayName;
        if (descriptionText != null) descriptionText.text = upgradeData.description;
        if (rarityText != null) rarityText.text = upgradeData.rarity.ToString();
        if (costText != null) 
        {
            costText.text = upgradeData.GetCostString();
            UpdateCostColor();
        }
        
        if (backgroundImage != null)
        {
            UpdateBackgroundColor();
        }
    }
    
    private void OnMouseDown()
    {
        if (isPurchased) return;
        
        if (myUpgradeData.Purchase())
        {
            isPurchased = true;
            OnCardSelected?.Invoke(myUpgradeData, gameObject);
            
            if (backgroundImage != null)
            {
                backgroundImage.color = Color.gray;
            }
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }
    
    private System.Collections.IEnumerator FlashRed()
    {
        if (backgroundImage != null)
        {
            Color originalColor = backgroundImage.color;
            backgroundImage.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            backgroundImage.color = originalColor;
        }
    }
    
    private void UpdateCostColor()
    {
        if (costText != null && myUpgradeData != null)
        {
            costText.color = myUpgradeData.CanBuy() ? Color.white : Color.red;
        }
    }
    
    private void UpdateBackgroundColor()
    {
        if (backgroundImage != null && myUpgradeData != null)
        {
            if (isPurchased)
            {
                backgroundImage.color = Color.gray;
            }
            else if (!myUpgradeData.CanBuy())
            {
                backgroundImage.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            }
            else
            {
                backgroundImage.color = GetRarityColor(myUpgradeData.rarity);
            }
        }
    }
    
    private Color GetRarityColor(RARITY rarity)
    {
        switch (rarity)
        {
            case RARITY.Common: return new Color(1f, 1f, 1f);
            case RARITY.Rare: return new Color(0.4f, 0.6f, 1f);
            case RARITY.Epic: return new Color(1f, 0f, 1f);
            case RARITY.Legend: return new Color(1f, 0.5f, 0f);
            default: return Color.white;
        }
    }
    
    void Update()
    {
        if (!isPurchased && myUpgradeData != null)
        {
            UpdateCostColor();
            UpdateBackgroundColor();
        }
    }
    
    private void OnDestroy()
    {
        OnCardSelected = null;
    }
}