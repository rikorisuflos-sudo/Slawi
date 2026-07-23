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

    InputAction normalAttackAction;
    InputAction absorbAttackAction;

    [SerializeField] Slider rushSlider;
    InputAction rushAction;
    float rushTimer = 0;
    [SerializeField] float rushDelay = 3;

    [SerializeField] Slider stompSlider;
    InputAction stompAction;
    float stompTimer = 0;
    [SerializeField] float stompDelay = 3;

    [SerializeField] Slider absorbSlider;
    InputAction absorbAction;
    float absorbTimer = 0;
    [SerializeField] float absorbDelay = 5;
    [SerializeField] Image absorbImage;
    [SerializeField] Sprite noneSprite;
    [SerializeField] Sprite fireBallSprite;

    AbsorbSkill absorbSkill = AbsorbSkill.None;
    public AbsorbSkill AbsorbSkill
    {
        set { absorbSkill = value;
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
        
    }

    void Update()
    { 
        rushTimer += Time.fixedDeltaTime;
        stompTimer += Time.fixedDeltaTime;
        absorbTimer += Time.fixedDeltaTime;

        if (normalAttackAction.IsPressed())
        {
            normalAttack.SetActive(true); // オブジェクトを有効化
            normalAttackScript.StartAttack();
        }
        if (absorbAttackAction.IsPressed())
        {
            absorbAttack.SetActive(true); // オブジェクトを有効化
            absorbAttackScript.StartAttack();
        }
        if (rushAction.IsPressed() && rushDelay <= rushTimer)
        {
            skillRush.SetActive(true);
            skillRushScript.StartAttack();
            rushTimer = 0;
        }
        if (stompAction.IsPressed() && stompDelay <= stompTimer)
        { 
            skillStomp.SetActive(true);
            skillStompScript.StartAttack();
            stompTimer = 0;
        }
        if (absorbAction.IsPressed() && absorbDelay <= absorbTimer)
        {
            if (absorbSkill == AbsorbSkill.None)
            {
                return;
            }
            //if(absorbSkill == AbsorbSkill.FireBallShot)
            //{
            //    absorbSlider.maxValue = absorbDelay;
            //    absorbSlider.value = absorbTimer;
            //    absorbImage.sprite = fireBallSprite;
            //}
            skillAbsorb.SetActive(true);
            skillAbsorbScript.StartAttack();
            absorbTimer = 0;
        }

        rushSlider.value = rushTimer;
        stompSlider.value = stompTimer;
        absorbSlider.value = absorbTimer;
    }
}
