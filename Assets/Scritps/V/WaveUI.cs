using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveTitleText;
    [SerializeField] private TextMeshProUGUI waveDescriptionText;
    [SerializeField] private GameObject waveInfoPanel;
    [SerializeField] private Image waveImage;
    
    [SerializeField] private float showDuration = 3f;
    
    private EnemySpawner enemySpawner;
    private float hideTimer = 0f;
    private bool isShowing = false;
    private GameObject[] addictiveObjects;
    void Start()
    {
        enemySpawner = FindAnyObjectByType<EnemySpawner>();
        
        if (enemySpawner != null)
        {
            enemySpawner.onWaveStart.AddListener(ShowWaveInfo);
        }
        
        if (waveInfoPanel != null)
        {
            waveInfoPanel.SetActive(false);
        }
    }
    
    void Update()
    {
        if (isShowing)
        {
            hideTimer -= Time.deltaTime;
            
            if (hideTimer <= 0f)
            {
                HideWaveInfo();
            }
        }
    }
    
    void ShowWaveInfo(string waveName, string waveDescription,  Sprite waveIcon, GameObject[] _addictiveObjects)
    {
        if (waveTitleText != null)
        {
            waveTitleText.text = waveName;
        }
        
        if (waveDescriptionText != null)
        {
            waveDescriptionText.text = waveDescription;
        }
        
        if (waveInfoPanel != null)
        {
            waveInfoPanel.SetActive(true);
        }

        if (waveImage != null)
        {
            waveImage.sprite = waveIcon;
            //waveImage.SetNativeSize();
        }

        if (_addictiveObjects != null)
        {
            addictiveObjects = _addictiveObjects;
            for (int i = 0; i < addictiveObjects.Length; i++)
            {
                addictiveObjects[i].gameObject.SetActive(true);
            }
        }
        
        hideTimer = showDuration;
        isShowing = true;
        
    }
    
    void HideWaveInfo()
    {
        if (addictiveObjects != null)
        {
            for (int i = 0; i < addictiveObjects.Length; i++)
            {
                addictiveObjects[i].gameObject.SetActive(false);
            }
        }
        
        if (waveInfoPanel != null)
        {
            waveInfoPanel.SetActive(false);
        }
        
        isShowing = false;
        
    }
    
    
    void OnDestroy()
    {
        if (enemySpawner != null)
        {
            enemySpawner.onWaveStart.RemoveListener(ShowWaveInfo);
        }
    }
}