using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    float move = 0f;

    [SerializeField] private float jumpForce;//ジャンプの高さ
    [SerializeField] private float walkForce;//速度の上昇値
    private bool isGrounded = true;//地面に触れているかいないか

    private void Awake()
    {
        //初期化
        Application.targetFrameRate = 60;
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;


        if (keyboard.aKey.isPressed)
        {
            move = -1f;
        }
        if (keyboard.dKey.isPressed)
        {
            move = 1f;
        }
        //Spaceが押されたとき
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
            Debug.Log("ジャンプ");
        }
        // //Dが押されたとき右に移動
        //if (Input.GetKey(KeyCode.D))
        //{
        //    Walk(this.walkForce);
        //    Debug.Log("右");
        //}

        ////Aが押されたとき左に移動
        //if (aKey.isPressed)
        //{
        //    Walk(-this.walkForce);
        //    Debug.Log("左");
        //}
        Walk();
        move = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //地面と接触しているかいないか
        if (collision.gameObject.CompareTag("Ground"))
        {
            DebugLogger.Log("地面についた");
            isGrounded = true;
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //地面から離れたかどうか
        if (collision.gameObject.CompareTag("Ground"))
        {
            DebugLogger.Log("地面から離れた");
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
            //this.rb.AddForce(transform.up * this.jumpForce);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            DebugLogger.Log("ジャンプ");
        }
    }

   
}
