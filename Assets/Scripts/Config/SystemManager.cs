//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class SystemManager : MonoBehaviour
//{
//    [SerializeField] private Toggle itemDmgDisplay;

//    [SerializeField] GameManager gameManager;

//    bool isOn;
//    public bool IsOn { get { return isOn; } }

//    private void Awake()
//    {
//        itemDmgDisplay.isOn = true;
//        isOn = true;
//    }

//    public void OnToggleChanged(bool isOn)
//    {
//        gameManager.Indication = isOn;
//    }
//}