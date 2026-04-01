using UnityEngine;
using UnityEngine.InputSystem;

public class SlimeController : MonoBehaviour
{

    private Mouse mouse;

    [SerializeField] GameObject normalAttack;
    private SlimeNormalAttack normalAttackScript;
    private void Awake()
    {
        mouse = Mouse.current;
    }

    private void Start()
    {
        normalAttackScript = normalAttack.GetComponent<SlimeNormalAttack>();
    }

    private void Update()
    {
        if (mouse.leftButton.wasPressedThisFrame)
        {
            normalAttack.SetActive(true); // オブジェクトを有効化
            normalAttackScript.StartAttack();
        }
    }
}
