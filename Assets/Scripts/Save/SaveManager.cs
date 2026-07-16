using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static string path = Application.persistentDataPath + "PlayerData.json";

    public static void Save (SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static SaveData Load()
    {
        if (!File.Exists(path))
        {
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }
}
