using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGaming : MonoBehaviour
{
    [SerializeField] private AudioSource melody;
    [SerializeField] private Slider sl;

    [SerializeField] private GameObject menu;

    [SerializeField] private GameData data;
    [SerializeField] private float Volume;

    [SerializeField] private bool isMenu= false;

    private void Awake()
    {
        StartSound();
    }
    void StartSound()
    {
        data.LoadFromJson();
        Volume = data.volume;
        sl.value = Volume;
        melody.volume = Volume;
    }
    void Start()
    {
        
    }

    public void ChangeVolume()
    {
        Volume = sl.value;
        data.volume = Volume;
        melody.volume = Volume;
        data.SaveToJson();
    }

    void Pause()
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
        isMenu = true;
    }

    void Resume()
    {
        menu.SetActive(false);
        Time.timeScale = 1f;
        isMenu= false;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Menu()
    {
        if(isMenu)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    void KeyUp()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            Menu();
        }
    }

    void Update()
    {
        KeyUp();
    }
}
