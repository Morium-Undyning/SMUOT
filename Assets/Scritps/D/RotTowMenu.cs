using UnityEngine;

public class RotTowMenu : MonoBehaviour
{

    void RotTower()
    {
        this.transform.Rotate(0, 10 * Time.deltaTime, 0);
    }
    // Update is called once per frame
    void Update()
    {
        RotTower();
    }
}
