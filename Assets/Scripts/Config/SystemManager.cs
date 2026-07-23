using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SystemManager : MonoBehaviour
{
    [SerializeField] private Toggle itemDmgDisplay;
    [SerializeField] private TMP_Dropdown itemHPGauge;

    private void OnEnable()
    {
        LoadSettings();
    }

  
    //データの読み込み
    public void LoadSettings()
    {
        if (itemDmgDisplay != null)
            itemDmgDisplay.isOn = PlayerPrefs.GetInt("DmgDisplay", 1) == 1;

        if (itemHPGauge != null)
            itemHPGauge.value = PlayerPrefs.GetInt("HPGaugePos", 0);
    }

    //データの保存
    public void SaveSettings()
    {
        if (itemDmgDisplay != null)
            PlayerPrefs.SetInt("DmgDisplay", itemDmgDisplay.isOn ? 1 : 0);

        if (itemHPGauge != null)
            PlayerPrefs.SetInt("HPGaugePos", itemHPGauge.value);
    }

    //デフォルトにリセット
    public void ResetToDefault()
    {
        if (itemDmgDisplay != null) itemDmgDisplay.isOn = true;
        if (itemHPGauge != null) itemHPGauge.value = 0;
    }
}