using UnityEngine;
using UnityEngine.UI;
using TMPro;

//画質設定
public class GraphicsManager : MonoBehaviour
{
    private const float DefaultBrightness = 1.0f;
    private const int DefaultWindowMode = 1; //フルスクリーン
    private const int DefaultResolution = 2; //1080p (FHD)

    [SerializeField] private TMP_Dropdown itemWindowMode;
    [SerializeField] private TMP_Dropdown itemResolution;
    [SerializeField] private Slider itemBrightness;

    private void OnEnable()
    {
        LoadSettings();
    }

    //データの読み込み
    public void LoadSettings()
    {
        if (itemWindowMode != null) itemWindowMode.value = PlayerPrefs.GetInt("WindowMode", DefaultWindowMode);
        if (itemResolution != null) itemResolution.value = PlayerPrefs.GetInt("Resolution", DefaultResolution);
        if (itemBrightness != null) itemBrightness.value = PlayerPrefs.GetFloat("Brightness", DefaultBrightness);
    }

    //データの保存
    public void SaveSettings()
    {
        if (itemWindowMode != null) PlayerPrefs.SetInt("WindowMode", itemWindowMode.value);
        if (itemResolution != null) PlayerPrefs.SetInt("Resolution", itemResolution.value);
        if (itemBrightness != null) PlayerPrefs.SetFloat("Brightness", itemBrightness.value);
    }

    //デフォルトにリセット
    public void ResetToDefault()
    {
        if (itemWindowMode != null) itemWindowMode.value = DefaultWindowMode;
        if (itemResolution != null) itemResolution.value = DefaultResolution;
        if (itemBrightness != null) itemBrightness.value = DefaultBrightness;
    }
}