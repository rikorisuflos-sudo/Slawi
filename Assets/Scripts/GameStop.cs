using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStop : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI myText;
    [SerializeField] GameObject canvas;
    public void OnClick()
    {
        canvas.SetActive(true);
        Time.timeScale = 0;
        myText.text = "Pause";
    }

    public void Close()
    {
        canvas.SetActive(false);
        Time.timeScale = 1;
    }
}
