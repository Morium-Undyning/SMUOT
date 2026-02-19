using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    [SerializeField] private Health healthComponent;
    private Slider slider;
    
    private void Start()
    {
        slider = GetComponent<Slider>();
        
        if (healthComponent == null)
        {
            healthComponent = GetComponentInParent<Health>();
        }
        
        if (slider != null && healthComponent != null)
        {
            InitializeHealthBar();
            healthComponent.onHealthChange.AddListener(UpdateHealthBar);
        }
    }
    
    private void InitializeHealthBar()
    {
        slider.maxValue = healthComponent.maxHealth;
        slider.value = healthComponent.CurrentHealth;
    }
    
    private void UpdateHealthBar(float health)
    {
        if (slider != null)
        {
            slider.maxValue = healthComponent.maxHealth;
            slider.value = health;
        }
    }
    
    private void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.onHealthChange.RemoveListener(UpdateHealthBar);
        }
    }
}