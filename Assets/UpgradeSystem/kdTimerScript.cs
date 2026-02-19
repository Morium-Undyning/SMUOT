using System;
using TMPro;
using UnityEngine;

public class kdTimerScript : MonoBehaviour
{
    public float interval;
    private float time;
    bool isRunning = false;
    public bool getIsRunning() => isRunning;
    private TextMeshProUGUI timerText;

    void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        timerText.enabled = false;
    }
    
    private void Update()
    {
        if (isRunning)
        {
            time += Time.deltaTime;
            if (time >= interval)
            {
                EndTimer();
                time = 0;
            }
            
        }


        timerText.text = $"{Mathf.Round(interval - time)}s";
    }

    public void StartTimer(float time)
    {
        interval = time;
        isRunning = true;
        timerText.enabled = true;
        
    }

    private void EndTimer()
    {
        isRunning = false;
        timerText.enabled = false;
        
    }
}
