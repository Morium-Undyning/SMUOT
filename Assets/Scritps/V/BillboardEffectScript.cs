using UnityEngine;

[ExecuteAlways]
public class BillboardEffectScript : MonoBehaviour
{
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
        
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }
    
    void Update()
    {
        if (mainCamera == null)
            return;
            
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
    
    void LateUpdate()
    {
        UpdateBillboard();
    }
    
    void UpdateBillboard()
    {
        if (mainCamera == null)
            return;
        
        transform.rotation = mainCamera.transform.rotation;
    }
}
