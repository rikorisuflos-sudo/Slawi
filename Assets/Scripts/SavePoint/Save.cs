using UnityEngine;

public class Save : MonoBehaviour
{
    public void executeSave()
    {
        SaveData data = new SaveData();

        SaveManager.Save(data);
    }
}
