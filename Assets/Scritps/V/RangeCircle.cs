using UnityEngine;

public class RangeCircle : MonoBehaviour
{
    [SerializeField] private int segments = 50;
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private Color lineColor = new Color(1, 0, 0, 0.5f);
    
    private LineRenderer lineRenderer;
    private float currentRange;
    
    void Start()
    {
        CreateCircle();
        
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnStatsChanged += UpdateCircle;
        }
    }
    
    void CreateCircle()
    {
        GameObject circle = new GameObject("RangeCircle");
        circle.transform.SetParent(transform);
        circle.transform.localPosition = Vector3.zero;
        
        lineRenderer = circle.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.loop = true;
        
        UpdateCircle();
    }
    
    void UpdateCircle()
    {
        if (PlayerStats.Instance != null)
        {
            currentRange = PlayerStats.Instance.Range;
        }
        
        if (lineRenderer != null)
        {
            DrawCircle(currentRange);
        }
    }
    
    void DrawCircle(float radius)
    {
        lineRenderer.positionCount = segments + 1;
        
        float angle = 0f;
        float angleStep = 360f / segments;
        
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            
            lineRenderer.SetPosition(i, new Vector3(x, 0.01f, z));
            angle += angleStep;
        }
    }
    
    void OnDestroy()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnStatsChanged -= UpdateCircle;
        }
    }
}