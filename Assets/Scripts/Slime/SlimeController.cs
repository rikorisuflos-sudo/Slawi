using System.Security.Cryptography.X509Certificates;
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
    float rushTimer = 0;
    [SerializeField] float rushDelay = 3;

    [SerializeField] Slider stompSlider;
    float stompTimer = 0;
    [SerializeField] float stompDelay = 3;

    [SerializeField] Slider absorbSlider;
    float absorbTimer = 0;
    [SerializeField] float absorbDelay = 5;
    [SerializeField] Image absorbImage;
    [SerializeField] Sprite noneSprite;
    [SerializeField] Sprite fireBallSprite;

    private InputSystem_Actions _controls;

    bool isAttacking = true;

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


    private void Awake()
    {
        mouse = Mouse.current;
        keyboard = Keyboard.current;

        rushTimer = rushDelay;
        rushSlider.maxValue = rushDelay;
        rushSlider.value = rushTimer;

        stompTimer = stompDelay;
        stompSlider.maxValue = stompDelay;
        absorbSlider.value = stompTimer;

        absorbTimer = absorbDelay;
        absorbImage.sprite = noneSprite;
        absorbSlider.maxValue = 1;
        absorbSlider.value = 1;


        _controls = new InputSystem_Actions();
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
        rushTimer += Time.fixedDeltaTime;
        stompTimer += Time.fixedDeltaTime;
        absorbTimer += Time.fixedDeltaTime;

        rushSlider.value = rushTimer;
        stompSlider.value = stompTimer;
        absorbSlider.value = absorbTimer;
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
