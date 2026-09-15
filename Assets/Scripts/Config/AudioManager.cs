using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


//音量設定
public class AudioManager : MonoBehaviour
{
    private const float DefaultVolume = 0.5f;
    private const float MaxVolume = 1.0f;
    private const float MinVolume = 0.0f;

    [SerializeField] GameManager gameManager;
    [SerializeField] AudioSource audioSource;

    float volume;
    public float Volume{get{ return volume; }}

    //[SerializeField] private Slider itemMasterVal;
    [SerializeField] private Slider itemBGMVal;
    //[SerializeField] private Slider itemSEVal;

    private void Awake()
    {
        itemBGMVal.maxValue = MaxVolume;
        itemBGMVal.minValue = MinVolume;
        itemBGMVal.value = DefaultVolume;

        audioSource.volume = itemBGMVal.value;
        gameManager.Volume = itemBGMVal.value;
    }

    public void OnValueChanged(float value)
    {
        volume = itemBGMVal.value;

        audioSource.volume = itemBGMVal.value;
        gameManager.Volume = itemBGMVal.value;
        Debug.Log(itemBGMVal.value);
    }
    //private void OnEnable()
    //{
    //    //LoadSettings();
    //}

    //データの読み込み
    //public void LoadSettings()
    //{
    //    if (itemMasterVal != null) itemMasterVal.value = PlayerPrefs.GetFloat("MasterVal", DefaultVolume);
    //    if (itemBGMVal != null) itemBGMVal.value = PlayerPrefs.GetFloat("BGMVal", DefaultVolume);
    //    if (itemSEVal != null) itemSEVal.value = PlayerPrefs.GetFloat("SEVal", DefaultVolume);
    //}

    ////データの保存
    //public void SaveSettings()
    //{
    //    if (itemMasterVal != null) PlayerPrefs.SetFloat("MasterVal", itemMasterVal.value);
    //    if (itemBGMVal != null) PlayerPrefs.SetFloat("BGMVal", itemBGMVal.value);
    //    if (itemSEVal != null) PlayerPrefs.SetFloat("SEVal", itemSEVal.value);
    //}

    ////デフォルトにリセット
    //public void ResetToDefault()
    //{
    //    if (itemMasterVal != null) itemMasterVal.value = DefaultVolume;
    //    if (itemBGMVal != null) itemBGMVal.value = DefaultVolume;
    //    if (itemSEVal != null) itemSEVal.value = DefaultVolume;
    //}
}