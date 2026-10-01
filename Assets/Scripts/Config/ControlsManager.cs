using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConfigKeyConfigManager : MonoBehaviour
{
    // リバインド対象のAction
    [SerializeField] private InputActionReference _actionRef;

    // リバインド対象のBinding番号
    [SerializeField] private int _bindingIndex = 0;

    // 現在のBindingのパスを表示するテキスト
    [SerializeField] private TMP_Text _pathText;

    // リバインド中のマスク用オブジェクト
    [SerializeField] private GameObject _mask;

    private InputAction _action;

    private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

    // 保存用キー
    private const string RebindSaveKey = "KeyConfig";

    private void Awake()
    {
        if (_actionRef == null) return;

        _action = _actionRef.action;

        // 保存されているキー設定を読み込む
        LoadBindingOverrides();

        RefreshDisplay();
    }

    private void OnDestroy()
    {
        CleanUpOperation();
    }

    public void StartRebinding()
    {
        if (_action == null) return;

        // すでにリバインド中ならキャンセル
        _rebindOperation?.Cancel();

        // Binding番号が範囲外なら終了
        if (_bindingIndex < 0 || _bindingIndex >= _action.bindings.Count)
            return;

        _action.Disable();

        if (_mask != null)
            _mask.SetActive(true);

        void OnFinished()
        {
            CleanUpOperation();

            _action.Enable();

            if (_mask != null)
                _mask.SetActive(false);
        }

        _rebindOperation = _action
            .PerformInteractiveRebinding(_bindingIndex)
            .OnComplete(_ =>
            {
                // キー設定を保存
                SaveBindingOverrides();

                RefreshDisplay();

                OnFinished();
            })
            .OnCancel(_ =>
            {
                OnFinished();
            })
            .Start();
    }

    public void RefreshDisplay()
    {
        if (_action == null || _pathText == null)
            return;

        if (_bindingIndex < 0 || _bindingIndex >= _action.bindings.Count)
            return;

        _pathText.text = _action.GetBindingDisplayString(_bindingIndex);
    }

    // 上書きされた情報をリセットする

    public void ResetOverrides()
    {
        _action?.RemoveAllBindingOverrides();

        PlayerPrefs.DeleteKey(RebindSaveKey);
        PlayerPrefs.Save();

        RefreshDisplay();
    }



    private void SaveBindingOverrides()
    {
        if (_action == null)
            return;

        string json = _action.actionMap.asset.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString(RebindSaveKey, json);
        PlayerPrefs.Save();


    }

    private void LoadBindingOverrides()
    {
        if (_action == null)
            return;

        if (PlayerPrefs.HasKey(RebindSaveKey))
        {
            string json = PlayerPrefs.GetString(RebindSaveKey);

            _action.actionMap.asset.LoadBindingOverridesFromJson(json);
        }
    }

    private void CleanUpOperation()
    {
        _rebindOperation?.Dispose();
        _rebindOperation = null;
    }
}