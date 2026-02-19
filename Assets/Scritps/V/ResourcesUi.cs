using TMPro;
using UnityEngine;

public class ResourcesUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourceText;
    private ResourcesSystem resourcesSystem;
    
    void Start()
    {
        resourcesSystem = FindObjectOfType<ResourcesSystem>();
        if (resourceText == null) resourceText = GetComponent<TextMeshProUGUI>();
    }
    
    void Update()
    {
        if (resourcesSystem != null && resourceText != null)
        {
            resourceText.text = $"Дерево: {resourcesSystem.Wood}\n" +
                                $"Камень: {resourcesSystem.Stone}\n" +
                                $"Кристаллы: {resourcesSystem.OilCristall}";
        }
    }
}
