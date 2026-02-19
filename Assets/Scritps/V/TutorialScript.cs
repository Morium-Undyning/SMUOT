using System;
using DG.Tweening;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    bool isShow = true;
    [SerializeField] float timeToHide;
    private float time;

    [SerializeField] private GameObject circle;
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject text;
    
    private void Start()
    {
        isShow = true;
        gameObject.SetActive(true);
        circle.transform.DOScale((Vector3.one * 0.15f) * 1.2f, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);

        Time.timeScale = 0;
    }

    private void Update()
    {
        if (isShow)
        {
            time += Time.deltaTime;
            if (time > timeToHide)
            {
                isShow = false;
                time = 0;
                gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }
        
        if (isShow && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            gameObject.SetActive(false);
            isShow = false;
            Time.timeScale = 1;
        }
    }
}
