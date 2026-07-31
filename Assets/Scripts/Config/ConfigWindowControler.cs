using UnityEngine;
using UnityEngine.UI;

//全体の制御
public class ConfigWindowController : MonoBehaviour
{
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GraphicsManager graphicsManager;
    [SerializeField] private ConfigKeyConfigManager keyConfigManager;

    [Header("操作ボタン")]
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button resetButton;

    [Header("戻り先")]
    [SerializeField] private GameObject pausePanel;

    private void Awake()
    {
        InitializeListeners();
    }

    private void InitializeListeners()
    {
        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmClicked);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnCancelClicked);
        if (resetButton != null) resetButton.onClick.AddListener(OnResetClicked);
    }

    //項目設定を保存
    private void OnConfirmClicked()
    {
        if (systemManager != null) systemManager.SaveSettings();
        if (audioManager != null) audioManager.SaveSettings();
        if (graphicsManager != null) graphicsManager.SaveSettings();
        if (keyConfigManager != null) keyConfigManager.SaveKeyBinds();

        PlayerPrefs.Save();
        CloseWindow();
    }

    //変更を破棄して閉じる
    private void OnCancelClicked()
    {
        //入力待ち中にキャンセルされた場合の安全対策
        if (keyConfigManager != null) keyConfigManager.CancelRebind();

        CloseWindow();
    }

    //項目の設定を全て初期化
    private void OnResetClicked()
    {
        if (systemManager != null) systemManager.ResetToDefault();
        if (audioManager != null) audioManager.ResetToDefault();
        if (graphicsManager != null) graphicsManager.ResetToDefault();
        if (keyConfigManager != null) keyConfigManager.ResetToDefault();
    }

    //設定画面を閉じてポーズ画面に戻る
    private void CloseWindow()
    {
        gameObject.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }
}