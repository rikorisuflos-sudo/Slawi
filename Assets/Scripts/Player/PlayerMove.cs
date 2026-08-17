using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Xml.Linq;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    const float MoveRun = 1.0f;//歩いているか
    const float MoveWalk = 0.5f;//走っているか

    Vector2 moveInput;//インプットシステム　左右移動
    bool isRunning;//インプットシステム　走っているかいないか


    float move = 0f;//プレイヤーが走っているかいないかを追尾クラスに送る
    public float Move
    {
        get { return move; } // getterの部分
    }

    [SerializeField] float jumpForce;//ジャンプの高さ
    [SerializeField] float walkForce;//速度の上昇値
    bool isGrounded = true;//地面に触れているかいないか

    const float GroundCatch = 0.5f;
    [SerializeField] LayerMask groundLayer;

    private InputSystem_Actions _controls;

    Keyboard keyboard;

    void Awake()
    {
        //初期化
        Application.targetFrameRate = 60;
        keyboard = Keyboard.current;

        _controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _controls.Enable();

        _controls.Player.Move.started += OnMove;
        _controls.Player.Move.performed += OnMove;
        _controls.Player.Move.canceled += OnMove;

        _controls.Player.Run.started += OnRun;
        _controls.Player.Run.performed += OnRun;
        _controls.Player.Run.canceled += OnRun;

        _controls.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        _controls.Player.Move.started -= OnMove;
        _controls.Player.Move.performed -= OnMove;
        _controls.Player.Move.canceled -= OnMove;

        _controls.Player.Run.started -= OnRun;
        _controls.Player.Run.performed -= OnRun;
        _controls.Player.Run.canceled -= OnRun;

        _controls.Player.Jump.performed -= OnJump;

        _controls.Disable();
    }

    private void FixedUpdate()
    {
        float speed = isRunning ? MoveRun : MoveWalk;

        rb.linearVelocity = new Vector2(moveInput.x * walkForce * speed,rb.linearVelocity.y);

        if (moveInput.x == 0) return;
        transform.localScale = new Vector3(moveInput.x, 1, 0);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //地面から離れたかどうか
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)//接触点の取得
            {
                // 下から接触 → 地面
                if (contact.normal.y > GroundCatch)
                {
                    isGrounded = true;
                }
            }
        }

    }
    void OnCollisionExit2D(Collision2D collision)
    {
        //地面から離れたかどうか
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    /// <summary>
    /// ジャンプする
    /// </summary>
    void OnJump(InputAction.CallbackContext context)
    {
        if (!isGrounded) Debug.Log("空中にいます");
        if (isGrounded)
        {
            Debug.Log("ジャンプ");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            //DebugLogger.Log("ジャンプ");
        }
    }
}
