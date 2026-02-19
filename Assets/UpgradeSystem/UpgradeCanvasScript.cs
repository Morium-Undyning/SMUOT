using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UpgradeCanvasScript : MonoBehaviour
{
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Button rerollButton;
    [SerializeField] private TextMeshProUGUI timerText;
    
    private List<GameObject> currentCards = new List<GameObject>();
    
    private kdTimerScript kdTimerScript;
    private int selectedCount = 0;
    
    void Start()
    {
        kdTimerScript = timerText.GetComponent<kdTimerScript>();
        
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
        
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OnUpgradesReady.AddListener(ShowUpgrades);
            UpgradeManager.Instance.OnUpgradeSelected.AddListener(OnAnyUpgradeSelected);
        }

        if (rerollButton != null)
        {
            rerollButton.onClick.AddListener(RerollUpgrades);
        }
    }

    private void Update()
    {
        if (rerollButton != null && kdTimerScript != null)
        {
            rerollButton.interactable = !kdTimerScript.getIsRunning();
        }
    }

    public void ToggleUpgradeMenu()
    {
        if (upgradePanel.activeSelf)
        {
            HideUpgrades();
        }
        else
        {
            OpenUpgradeMenu();
        }
    }
    
    public void ShowUpgrades(List<UpgradeData> upgrades)
    {
        if (upgradePanel == null || cardPrefab == null) return;
        
        ClearCards();
        selectedCount = 0;
        
        upgradePanel.SetActive(true);
        
        foreach (var upgrade in upgrades)
        {
            GameObject card = Instantiate(cardPrefab, cardsContainer);
            UpgradeSlotScript cardScript = card.GetComponent<UpgradeSlotScript>();
            
            if (cardScript != null)
            {
                cardScript.Setup(upgrade);
                cardScript.OnCardSelected += OnCardSelected;
                currentCards.Add(card);
            }
        }
    }

    public void HideUpgrades()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
    }
    
    public void OpenUpgradeMenu()
    {
        if (upgradePanel.activeSelf) return;
        
        if (selectedCount >= 3 || currentCards.Count == 0)
        {
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.GenerateRandomUpgrades();
            }
        }
        else
        {
            upgradePanel.SetActive(true);
        }
    }
    
    public void RerollUpgrades()
    {
        if (UpgradeManager.Instance != null && kdTimerScript != null)
        {
            if (!kdTimerScript.getIsRunning())
            {
                ClearCards();
                selectedCount = 0;
                UpgradeManager.Instance.GenerateRandomUpgrades();
                kdTimerScript.StartTimer(6);
            }
        }
    }
    
    private void OnCardSelected(UpgradeData upgrade, GameObject cardObject)
    {
        selectedCount++;
        
        if (cardObject != null)
        {
            cardObject.SetActive(false);
        }
        
        if (selectedCount >= 3)
        {
            StartCoroutine(ShowNewUpgradesAfterDelay(0.5f));
        }
    }
    
    private void OnAnyUpgradeSelected(UpgradeData upgrade)
    {
    }
    
    private void ClearCards()
    {
        foreach (var card in currentCards)
        {
            if (card != null)
            {
                var cardScript = card.GetComponent<UpgradeSlotScript>();
                if (cardScript != null)
                {
                    cardScript.OnCardSelected -= OnCardSelected;
                }
                Destroy(card);
            }
        }
        currentCards.Clear();
        selectedCount = 0;
    }
    
    private IEnumerator ShowNewUpgradesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        ClearCards();
        
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.GenerateRandomUpgrades();
        }
    }
    
    private void OnDestroy()
    {
        ClearCards();
        
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OnUpgradesReady.RemoveListener(ShowUpgrades);
            UpgradeManager.Instance.OnUpgradeSelected.RemoveListener(OnAnyUpgradeSelected);
        }
    }
}