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
    //bool isGrounded = true;//地面に触れているかいないか
    [SerializeField] float groundCheckDistance;

    const float GroundCatch = 0.53f;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] Collider2D moveArea;//移動制限用コライダー

    [SerializeField] private InputActionReference _actionRef;
    private InputAction _action;
    private InputSystem_Actions _controls;

    Keyboard keyboard;

    private const string RebindSaveKey = "KeyConfig";


    void Awake()
    {
        //初期化
        Application.targetFrameRate = 60;
        keyboard = Keyboard.current;

        _controls = new InputSystem_Actions();

        _action = _actionRef.action;

        LoadBindingOverrides();
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

        float moveX = moveInput.x * walkForce * speed;

        Bounds bounds = moveArea.bounds;

        // 現在位置
        Vector2 currentPos = rb.position;

        // X方向の次の位置
        float nextX = currentPos.x + moveX * Time.fixedDeltaTime;

        // X方向の範囲制限
        if (nextX < bounds.min.x && moveX < 0)
        {
            moveX = 0f;
        }
        else if (nextX > bounds.max.x && moveX > 0)
        {
            moveX = 0f;
        }

        rb.linearVelocity = new Vector2(
            moveX,
            rb.linearVelocity.y
        );

        if (moveInput.x != 0)
        {
            transform.localScale = new Vector3(moveInput.x, 1, 0);
        }
    }


    public void LoadBindingOverrides()
    {
        if (PlayerPrefs.HasKey(RebindSaveKey))
        {
            string json = PlayerPrefs.GetString(RebindSaveKey);
            _controls.asset.LoadBindingOverridesFromJson(json);
        }
        else
        {
            // 保存された設定がない場合は、全て初期状態に戻す
            _controls.asset.RemoveAllBindingOverrides();
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
        //if (!isGrounded) Debug.Log("空中にいます");
        //if (isGrounded)
        //{
        // 足元に地面があるか確認
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,new Vector2(0, -1), groundCheckDistance,groundLayer);
        if(hit.collider != null) { 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            //DebugLogger.Log("ジャンプ");
        }
    }
}
