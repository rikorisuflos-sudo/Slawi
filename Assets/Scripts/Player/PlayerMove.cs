using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Xml.Linq;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    const float MoveRun = 1.0f;//歩いているか
    const float MoveWalk = 0.5f;//走っているか

    float move = 0f;//上の二つの変数を受け渡すための変数
    public float Move
    {
        get { return move; } // getterの部分
    }

    [SerializeField] float jumpForce;//ジャンプの高さ
    [SerializeField] float walkForce;//速度の上昇値
    bool isGrounded = true;//地面に触れているかいないか

    const float GroundCatch = 0.5f;
    [SerializeField] LayerMask groundLayer;

    Keyboard keyboard;

    void Awake()
    {
        //初期化
        Application.targetFrameRate = 60;
        keyboard = Keyboard.current;
    }

    void FixedUpdate()
    {
        if (keyboard == null) return;
        move = 0f;

        ////Aが押されたとき左に移動
        if (keyboard.aKey.isPressed)
        {
            move = keyboard.shiftKey.isPressed ? -MoveRun : -MoveWalk;
            transform.localScale = new Vector3(-1, 1, 0);
        }
        // //Dが押されたとき右に移動
        if (keyboard.dKey.isPressed)
        {
            move = keyboard.shiftKey.isPressed ? MoveRun : MoveWalk;
            transform.localScale = new Vector3(1, 1, 0);
        }

        Walk();

        //Spaceが押されたとき
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }
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

    /// <summary>
    /// 歩く
    /// </summary>
    void Walk()
    {
        rb.linearVelocity = new Vector2(move * walkForce, rb.linearVelocity.y);
    }

    /// <summary>
    /// ジャンプする
    /// </summary>
    void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            //DebugLogger.Log("ジャンプ");
        }
    }
}
