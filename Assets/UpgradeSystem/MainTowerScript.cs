using System;
using UnityEngine;
using UnityEngine.Events;

public class MainTowerScript : MonoBehaviour
{
    
    public UnityEvent onClick;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        onClick?.Invoke();
    }

}
