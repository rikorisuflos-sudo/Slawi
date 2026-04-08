using UnityEngine;
using UnityEngine.InputSystem;

public class SlimeController : MonoBehaviour
{

    private Mouse mouse;
    private Keyboard keyboard;

    [SerializeField] GameObject normalAttack;
    private SlimeNormalAttack normalAttackScript;
    [SerializeField] GameObject skillAttack1;
    private SkillAttack skillAttack1Script;
    private void Awake()
    {
        mouse = Mouse.current;
        keyboard = Keyboard.current;
    }

    private void Start()
    {
        normalAttackScript = normalAttack.GetComponent<SlimeNormalAttack>();
        skillAttack1Script = skillAttack1.GetComponent<SkillAttack>();
    }

    private void Update()
    {
        if (mouse.leftButton.wasPressedThisFrame)
        {
            normalAttack.SetActive(true); // オブジェクトを有効化
            normalAttackScript.StartAttack();
        }
        if (keyboard.rKey.wasPressedThisFrame)
        {
            skillAttack1.SetActive(true);
            skillAttack1Script.StartAttack();
        }
    }
}
