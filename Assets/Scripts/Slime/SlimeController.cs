using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SlimeController : MonoBehaviour
{

    private Mouse mouse;
    private Keyboard keyboard;

    //[SerializeField] Animator animator;
    //const float NormalizedTime = 0f;
    //const int Layer = -1;

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
    private void Awake()
    {
        mouse = Mouse.current;
        keyboard = Keyboard.current;
    }

    private void Start()
    {
        //normalAttackScript = normalAttack.GetComponent<SkillRush>();
        //skillAttack1Script = skillAttack1.GetComponent<SkillStomp>();
    }

    private void Update()
    {
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
        if (keyboard.fKey.wasPressedThisFrame)
        {
            skillRush.SetActive(true);
            skillRushScript.StartAttack();
        }
        if (keyboard.rKey.wasPressedThisFrame)
        { 
            skillStomp.SetActive(true);
            skillStompScript.StartAttack();
        }
        if (keyboard.vKey.wasPressedThisFrame)
        {
            skillAbsorb.SetActive(true);
            skillAbsorbScript.StartAttack();
        }
    }
}
