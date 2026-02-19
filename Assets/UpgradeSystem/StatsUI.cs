using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    
    void Start()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
        
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnStatsChanged += UpdateUI;
        }
        
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (PlayerStats.Instance != null && text != null)
        {
            text.text = $"Max HP: {PlayerStats.Instance.MaxHp:F1}\n" +
                        $"Range: {PlayerStats.Instance.Range:F1}\n" +
                        $"Damage: {PlayerStats.Instance.Dmg:F1}";
        }

    }
}