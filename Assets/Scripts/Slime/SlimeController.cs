using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

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

    float rushTimer = 0;
    [SerializeField] float rushDelay = 3;
    float stompTimer = 0;
    [SerializeField] float stompDelay = 3;
    float absorbTimer = 0;
    [SerializeField] float absorbDelay = 5;

    private void Awake()
    {
        mouse = Mouse.current;
        keyboard = Keyboard.current;
    }

    void FixedUpdate()
    { 
        rushTimer += Time.fixedDeltaTime;
        stompTimer += Time.fixedDeltaTime;
        absorbTimer += Time.fixedDeltaTime;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            normalAttack.SetActive(true); // オブジェクトを有効化
            normalAttackScript.StartAttack();
        }
        if (mouse.rightButton.wasPressedThisFrame)
        {
            absorbAttack.SetActive(true); // オブジェクトを有効化
            absorbAttackScript.StartAttack();
        }
        if (keyboard.fKey.wasPressedThisFrame && rushDelay <= rushTimer)
        {
            skillRush.SetActive(true);
            skillRushScript.StartAttack();
            rushTimer = 0;
        }
        if (keyboard.rKey.wasPressedThisFrame && stompDelay <= stompTimer)
        { 
            skillStomp.SetActive(true);
            skillStompScript.StartAttack();
            stompTimer = 0;
        }
        if (keyboard.vKey.wasPressedThisFrame && absorbDelay <= absorbTimer)
        {
            skillAbsorb.SetActive(true);
            skillAbsorbScript.StartAttack();
            absorbTimer = 0;
        }
    }
}
