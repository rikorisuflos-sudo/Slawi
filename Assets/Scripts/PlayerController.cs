using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Xml.Linq;

public class PlayerController : MonoBehaviour
{
    [SerializeField]private Rigidbody2D rb;
    

    float move = 0f;
    public float Move
    {
        get { return move; } // getterの部分
    }

    [SerializeField] private float jumpForce;//ジャンプの高さ
    [SerializeField] private float walkForce;//速度の上昇値
    private bool isGrounded = true;//地面に触れているかいないか

    private Keyboard keyboard;

    private void Awake()
    {
        //初期化
        Application.targetFrameRate = 60;
        keyboard = Keyboard.current;
    }


    void Update()
    {

        if (keyboard == null) return;
        move = 0f;

        ////Aが押されたとき左に移動
        if (keyboard.aKey.isPressed)
        {
            move = keyboard.shiftKey.isPressed ? -1.5f : -1f;
        }
        // //Dが押されたとき右に移動
        if (keyboard.dKey.isPressed)
        {
            move = keyboard.shiftKey.isPressed ? 1.5f : 1f;
        }
       
       
    }

    void FixedUpdate()
    {
        Walk();
        //Spaceが押されたとき
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //地面と接触しているかいないか
        if (collision.gameObject.CompareTag("Ground"))
        {
            //DebugLogger.Log("地面についた");
            isGrounded = true;
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //地面から離れたかどうか
        if (collision.gameObject.CompareTag("Ground"))
        {
            //DebugLogger.Log("地面から離れた");
            isGrounded = false;
        }
        
        
    }

    /// <summary>
    /// 歩く
    /// </summary>
    private void Walk()
    {
        rb.linearVelocity = new Vector2(move * walkForce, rb.linearVelocity.y);
    }

    /// <summary>
    /// ジャンプする
    /// </summary>
    private void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            //DebugLogger.Log("ジャンプ");
        }
    }

   
}
