using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData")]
public class GameData : ScriptableObject
{
    //public int coins;
    //public string playerName;
    public float volume;
    
    public void SaveToJson()
    {
        string json = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/save.json", json);
    }


    public void LoadFromJson()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, this); 
        }
    }
}