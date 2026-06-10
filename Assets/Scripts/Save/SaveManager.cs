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
}
