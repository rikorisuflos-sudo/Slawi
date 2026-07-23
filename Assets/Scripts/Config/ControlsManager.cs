using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ConfigKeyConfigManager : MonoBehaviour
{
    private const string WaitingKeyText = "...Press Key"; //入力待ち表示
    private const string DisplayBindingGroup = "Keyboard&Mouse"; //適用対象のスキーム名

    [SerializeField] private PlayerInput playerInput; //プレイヤーに使っている PlayerInput コンポーネント

    [Header("キー設定ボタン")]
    [SerializeField] private Button itemSkillAbsorb;
    [SerializeField] private Button itemSkillCharge;
    [SerializeField] private Button itemSkillStomp;
    [SerializeField] private Button itemAtkAbsorb;
    [SerializeField] private Button itemAttack;
    [SerializeField] private Button itemDash;
    [SerializeField] private Button itemJump;
    [SerializeField] private Button itemLeft;
    [SerializeField] private Button itemRight;

    private Dictionary<string, Button> keyButtons = new Dictionary<string, Button>();
    private InputActionRebindingExtensions.RebindingOperation currentRebindOperation;

    private void Awake()
    {
        InitializeMapping();
        InitializeListeners();
    }

    private void OnEnable()
    {
        UpdateAllButtonTexts();
    }

    private void OnDisable()
    {
        //閉じた際に入力待ち処理があればクリーンアップ
        currentRebindOperation?.Dispose();
    }

    //アクション名と UI のマッピング初期化
    private void InitializeMapping()
    {
        keyButtons.Clear();
        if (itemSkillAbsorb != null) keyButtons.Add("Skill_Absorb", itemSkillAbsorb);
        if (itemSkillCharge != null) keyButtons.Add("Skill_Charge", itemSkillCharge);
        if (itemSkillStomp != null) keyButtons.Add("Skill_Stomp", itemSkillStomp);
        if (itemAtkAbsorb != null) keyButtons.Add("ATK_Absorb", itemAtkAbsorb);
        if (itemAttack != null) keyButtons.Add("Attack", itemAttack);
        if (itemDash != null) keyButtons.Add("Dash", itemDash);
        if (itemJump != null) keyButtons.Add("Jump", itemJump);
        if (itemLeft != null) keyButtons.Add("Left", itemLeft);
        if (itemRight != null) keyButtons.Add("Right", itemRight);
    }

    private void InitializeListeners()
    {
        foreach (KeyValuePair<string, Button> pair in keyButtons)
        {
            string actionName = pair.Key;
            Button btn = pair.Value;
            if (btn != null)
            {
                btn.onClick.AddListener(() => StartInteractiveRebind(actionName));
            }
        }
    }

    //ボタン表示を入力システムのキー設定に合わせる
    private void UpdateAllButtonTexts()
    {
        if (playerInput == null) return;

        foreach (KeyValuePair<string, Button> pair in keyButtons)
        {
            string actionName = pair.Key;
            InputAction action = playerInput.actions.FindAction(actionName);

            if (action != null)
            {
                //表示名を取得　UIに反映
                string displayString = action.GetBindingDisplayString(0);
                UpdateButtonText(actionName, displayString);
            }
        }
    }

    //インタラクティブリバインド処理
    private void StartInteractiveRebind(string actionName)
    {
        if (playerInput == null) return;

        InputAction action = playerInput.actions.FindAction(actionName);
        if (action == null) return;

        //進行中リバインドはキャンセル
        currentRebindOperation?.Cancel();

        //一時的に入力を無効化
        playerInput.currentActionMap.Disable();

        //UI表示を入力待ちに
        UpdateButtonText(actionName, WaitingKeyText);

        //リバインド機能実行
        currentRebindOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("<Pointer>/position") // マウス移動を除外
            .WithControlsExcluding("<Pointer>/delta")
            .WithCancelingThrough("<Keyboard>/escape") // Escキーでキャンセル
            .OnCancel(operation =>
            {
                //キャンセル処理
                playerInput.currentActionMap.Enable();
                UpdateButtonText(actionName, action.GetBindingDisplayString(0));
                operation.Dispose();
            })
            .OnComplete(operation =>
            {
                //入力完了処理
                playerInput.currentActionMap.Enable();
                UpdateButtonText(actionName, action.GetBindingDisplayString(0));
                operation.Dispose();
            });

        currentRebindOperation.Start();
    }

    //ボタンの表示テキストを更新
    private void UpdateButtonText(string actionName, string text)
    {
        if (!keyButtons.ContainsKey(actionName) || keyButtons[actionName] == null) return;

        Button targetButton = keyButtons[actionName];
        TMP_Text tmpText = targetButton.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = text;
            return;
        }

        Text legacyText = targetButton.GetComponentInChildren<Text>();
        if (legacyText != null)
        {
            legacyText.text = text;
        }
    }

    //リバインド状態を JSON 形式で保存
    public void SaveKeyBinds()
    {
        if (playerInput == null) return;

        //設定された情報をJSON文字列として保存
        string rebindsJson = playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("InputBindingOverrides", rebindsJson);
    }

    //JSONからキー設定を復元
    public void LoadKeyBinds()
    {
        if (playerInput == null) return;

        string rebindsJson = PlayerPrefs.GetString("InputBindingOverrides", string.Empty);
        if (!string.IsNullOrEmpty(rebindsJson))
        {
            playerInput.actions.LoadBindingOverridesFromJson(rebindsJson);
            UpdateAllButtonTexts();
        }
    }

    //割り当てを初期化
    public void ResetToDefault()
    {
        if (playerInput == null) return;

        //キー変更を消去
        playerInput.actions.RemoveAllBindingOverrides();
        UpdateAllButtonTexts();
    }

    //処理の中断（設定画面をそのまま閉じた場合の安全対策）
    public void CancelRebind()
    {
        if (currentRebindOperation != null && currentRebindOperation.started)
        {
            currentRebindOperation.Cancel();
        }
    }
}