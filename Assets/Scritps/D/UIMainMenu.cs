using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject Option;
    [SerializeField] private AudioSource melody;
    [SerializeField] private Slider sl;

    [SerializeField] private GameData data;
    [SerializeField] private float Volume;
    void Start()
    {
        
    }

    void Awake()
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

    public void MainMenu()
    {
        Menu.SetActive(true);
        Option.SetActive(false);
    }

    

    public void ChangeVolume()
    {
        Volume = sl.value;
        data.volume = Volume;
        melody.volume = Volume;
        data.SaveToJson();
    }

    public void Options()
    {
        Menu.SetActive(false);
        Option.SetActive(true);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        data.SaveToJson();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            
            Application.Quit();
#endif
    }

    
    void Update()
    {
        
    }
}
