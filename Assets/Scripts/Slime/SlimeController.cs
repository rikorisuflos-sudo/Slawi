using System.IO;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SlimeController : MonoBehaviour
{

    private Mouse mouse;
    private Keyboard keyboard;



    [SerializeField] GameObject normalAttack;
    [SerializeField] NormalAttack normalAttackScript;
    [SerializeField] GameObject absorbAttack;
    [SerializeField] AbsorbAttack absorbAttackScript;
    [SerializeField] GameObject skillRush;
    [SerializeField] SkillRush skillRushScript;
    [SerializeField] GameObject skillStomp;
    [SerializeField] SkillStomp skillStompScript;
    [SerializeField] GameObject skillAbsorb;
    [SerializeField] SkillAbsorb skillAbsorbScript;

    [SerializeField] Slider rushSlider;
    [SerializeField] TextMeshProUGUI rushKeyText;
    float rushTimer = 0;
    [SerializeField] float rushDelay = 3;

    [SerializeField] Slider stompSlider;
    [SerializeField] TextMeshProUGUI stompKeyText;
    float stompTimer = 0;
    [SerializeField] float stompDelay = 3;

    [SerializeField] Slider absorbSlider;
    [SerializeField] TextMeshProUGUI absorbKeyText;
    float absorbTimer = 0;
    [SerializeField] float absorbDelay = 5;
    [SerializeField] Image absorbImage;
    [SerializeField] Sprite noneSprite;
    [SerializeField] Sprite fireBallSprite;

    private InputSystem_Actions _controls;

    bool isAttacking = false;

    public bool IsAttacking
    {
        get { return isAttacking; }
        set { isAttacking = value; }
    }

    AbsorbSkill absorbSkill = AbsorbSkill.None;
    public AbsorbSkill AbsorbSkill
    {
        set
        {
            absorbSkill = value;
            absorbSlider.maxValue = absorbDelay;
            absorbSlider.value = absorbTimer;
            absorbImage.sprite = fireBallSprite;
        } // setterの部分
    }

    private const string RebindSaveKey = "KeyConfig";

    


    private void Awake()
    {
        //マウスとキーボードの取得
        mouse = Mouse.current;
        keyboard = Keyboard.current;

        _controls = new InputSystem_Actions();

        LoadBindingOverrides();

        isAttacking = false;

        //突進の入力関連設定
        //タイマーの初期化
        rushTimer = rushDelay;
        //スライダーの設定
        rushSlider.maxValue = rushDelay;
        rushSlider.value = rushTimer;
        //キーの取得と表示
        string rushPath = _controls.Player.SkillRush.bindings[0].effectivePath;
        string rushKey = InputControlPath.ToHumanReadableString(rushPath,InputControlPath.HumanReadableStringOptions.OmitDevice);
        rushKeyText.text = rushKey;

        //落下の入力関連設定
        //タイマーの初期化
        stompTimer = stompDelay;
        //スライダーの設定
        stompSlider.maxValue = stompDelay;
        absorbSlider.value = stompTimer;
        //キーの取得と表示
        string stompPath = _controls.Player.SkillStomp.bindings[0].effectivePath;
        string stompKey = InputControlPath.ToHumanReadableString(stompPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        stompKeyText.text = stompKey;

        //吸収スキルの入力関連設定
        //タイマーの初期化
        absorbTimer = absorbDelay;
        //スライダーの設定
        absorbImage.sprite = noneSprite;
        absorbSlider.maxValue = 1;
        absorbSlider.value = 1;
        //キーの取得と表示
        string absorbPath = _controls.Player.SkillAbsorb.bindings[0].effectivePath;
        string absorbKey = InputControlPath.ToHumanReadableString(absorbPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        absorbKeyText.text = absorbKey;


    }

    void OnEnable()
    {
        _controls.Enable();

        //イベント登録
        _controls.Player.NormalAttack.performed += OnNormalAttack;
        _controls.Player.AbsorbAttack.performed += OnAbsorbAttack;
        _controls.Player.SkillRush.performed += OnSkillRush;
        _controls.Player.SkillStomp.performed += OnSkillStomp;
        _controls.Player.SkillAbsorb.performed += OnSkillAbsorb;
    }

    void OnDisable()
    {
        //イベント登録
        _controls.Player.NormalAttack.performed -= OnNormalAttack;
        _controls.Player.AbsorbAttack.performed -= OnAbsorbAttack;
        _controls.Player.SkillRush.performed -= OnSkillRush;
        _controls.Player.SkillStomp.performed -= OnSkillStomp;
        _controls.Player.SkillAbsorb.performed -= OnSkillAbsorb;

        _controls.Disable();
    }

    void Update()
    {
        rushTimer += Time.deltaTime;
        stompTimer += Time.deltaTime;
        absorbTimer += Time.deltaTime;

        rushSlider.value = rushTimer;
        stompSlider.value = stompTimer;
        absorbSlider.value = absorbTimer;
    }

    private void LoadBindingOverrides()
    {
        if (PlayerPrefs.HasKey(RebindSaveKey))
        {
            string json = PlayerPrefs.GetString(RebindSaveKey);
            _controls.asset.LoadBindingOverridesFromJson(json);
        }
    }

    void OnNormalAttack(InputAction.CallbackContext context)
    {
        if (!isAttacking && normalAttack.activeSelf) return;
        normalAttack.SetActive(true); // オブジェクトを有効化
        normalAttackScript.StartAttack();
    }

    void OnAbsorbAttack(InputAction.CallbackContext context)
    {
        if (!isAttacking && absorbAttack.activeSelf) return;
        absorbAttack.SetActive(true); // オブジェクトを有効化
        absorbAttackScript.StartAttack();
    }

    void OnSkillRush(InputAction.CallbackContext context)
    {
        if (!isAttacking && rushDelay <= rushTimer)
        {
            skillRush.SetActive(true);
            skillRushScript.StartAttack();
            rushTimer = 0;
        }
    }

    void OnSkillStomp(InputAction.CallbackContext context)
    {
        if (!isAttacking && stompDelay <= stompTimer)
        {
            skillStomp.SetActive(true);
            skillStompScript.StartAttack();
            stompTimer = 0;
        }
    }

    void OnSkillAbsorb(InputAction.CallbackContext context)
    {
        if (!isAttacking && absorbDelay <= absorbTimer)
        {
            if (absorbSkill == AbsorbSkill.None)
            {
                return;
            }
            if (absorbSkill == AbsorbSkill.FireBallShot)
            {
                absorbSlider.maxValue = absorbDelay;
                absorbSlider.value = absorbTimer;
                absorbImage.sprite = fireBallSprite;
            }
            skillAbsorb.SetActive(true);
            skillAbsorbScript.StartAttack();
            absorbTimer = 0;
        }
    }

}
