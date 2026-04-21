using UnityEngine;
using System.IO; // Важно для работы с файлами!

public class SaveManager : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        // Формируем путь: "Папка_Игры/save.json"
        saveFilePath = Application.persistentDataPath + "/save.json"; 
    }

    // --- СОХРАНЕНИЕ ---
    public void SaveGame(int waveToSave, float hpToSave)
    {
        // 1. Упаковываем данные в контейнер
        SaveData data = new SaveData();
        data.currentWave = waveToSave;
        data.playerHealth = hpToSave;

        // 2. Превращаем контейнер в строку JSON
        string json = JsonUtility.ToJson(data);

        // 3. Записываем строку в файл на жесткий диск
        File.WriteAllText(saveFilePath, json);
        
        Debug.Log("Игра сохранена по пути: " + saveFilePath);
    }

    // --- ЗАГРУЗКА ---
    public SaveData LoadGame()
    {
        // Проверяем, существует ли вообще файл сохранения
        if (File.Exists(saveFilePath))
        {
            // Читаем текст из файла
            string json = File.ReadAllText(saveFilePath);
            
            // Расшифровываем JSON обратно в наш класс SaveData
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