using UnityEngine;
using System.IO; 

public class SaveManager : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/save.json"; 
    }

    public void SaveGame(int waveToSave, float hpToSave)
    {
        SaveData data = new SaveData();
        data.currentWave = waveToSave;
        data.playerHealth = hpToSave;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(saveFilePath, json);
        
        Debug.Log("Игра сохранена по пути: " + saveFilePath);
    }

    public SaveData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            
            SaveData loadedData = JsonUtility.FromJson<SaveData>(json);
            return loadedData;
        }
        else
        {
            Debug.Log("Файл сохранения не найден!");
            return null; 
        }
    }
}
