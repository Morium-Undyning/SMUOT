using UnityEngine;

public class LosePanel : MonoBehaviour
{
    private Health _health;
    void Start()
    {
        _health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        _health.onDeath.AddListener(Show);
    }

    void Show()
    {
        gameObject.SetActive(true);
    }
}
