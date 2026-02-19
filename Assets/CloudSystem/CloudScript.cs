using UnityEngine;

public class CloudScript : MonoBehaviour
{
    public float speed = 5f;
    void Start()
    {
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime, 0, 0);
    }
}
