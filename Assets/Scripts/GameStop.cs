using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameStop : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI myText;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject button;

    private InputSystem_Actions _controls;
    private const string RebindSaveKey = "KeyConfig";

    //private void Awake()
    //{
    //    _controls = new InputSystem_Actions();
    //}
    public void OnClick()
    {
        canvas.SetActive(true);
        button.SetActive(false);
        Time.timeScale = 0;
        myText.text = "Pause";
    }

    public void Close()
    {
        canvas.SetActive(false);
        button.SetActive(true);
        Time.timeScale = 1;
        //LoadBindingOverrides();
    }
    //private void LoadBindingOverrides()
    //{
    //    if (PlayerPrefs.HasKey(RebindSaveKey))
    //    {
    //        string json = PlayerPrefs.GetString(RebindSaveKey);
    //        _controls.asset.LoadBindingOverridesFromJson(json);
    //    }
    //}
}
