using System.Collections;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class SlimeNormalAttack : MonoBehaviour
{
   //親オブジェクトのものを取得
    Rigidbody2D parentRd;

    [SerializeField] SlimeController slimeController;
    [SerializeField]  Transform playerTr;//プレイヤー
    //[SerializeField] Animator animator;

    [SerializeField]  float rushDistance = 20f;//攻撃距離
    [SerializeField]  float rushSpeed ;//攻撃速度
    float dashDir = 0;//ダッシュ方向

    //[SerializeField]  float chargeTime = 0.3f;   // 溜め時間
    //[SerializeField]  float recoveryTime = 0.3f; // 硬直時間


     Vector2 dashStartPos; // 突進開始位置

    [SerializeField]  float wallCheckDistance = 0.5f;//壁の探索範囲
    [SerializeField]  LayerMask groundLayer; // 地面レイヤー

    [SerializeField] GameObject playerTracking;
    //float timer = 0;
    bool hasEndedDash = false;
    enum State
    {
        Idle,//待機
        Warp,//ワープ
        Charge,//攻撃前の溜め
        Dash,//突進
        Recovery//硬直
    }

    State currentState = State.Idle;

    void Awake()
    {
        parentRd = GetComponentInParent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Warp:
                Warp();
                break;
            case State.Charge:
                Charge();
                break;

            case State.Dash:
                Dash();
                break;

            case State.Recovery:
                Recovery();
                break;
        }
    }
    //プレイヤーの前に移動
    void Warp()
    {
        //ワープさせる
        Vector2 warpPos = playerTr.transform.position + new Vector3(dashDir * 1.5f, 0, 0);

        //ワープしたときに突進方向を向くようにする
        transform.localScale = new Vector3(dashDir, 1, 1);

        parentRd.position = warpPos;
        dashStartPos = parentRd.position;
        //currentState = State.Charge;
    }

    //プレイヤー追従処理OFF
    //突進方向の確定、タイマーの初期化
    public void StartAttack()
    {
        playerTracking.SetActive(false);
        slimeController.enabled = false;
        dashDir = Mathf.Sign(playerTr.transform.localScale.x);
        if (dashDir == 0) dashDir = 1;

        //timer = 0;
        currentState = State.Warp;
    }
    //攻撃前の溜め
    void Charge()
    {
        //timer += Time.fixedDeltaTime;

        parentRd.linearVelocity = Vector2.zero;

        //アニメーションの再生位置を示す（開始0,終了１）
        //float broadcastTime = parentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;



        //if (broadcastTime >= 1)
        //{
        //    //timer = 0;
        //    parentAnimator.SetBool("StartRush",true);
        //    currentState = State.Dash;
        //}
    }
    //突進
    void Dash()
    {
        parentRd.linearVelocity = new Vector2(dashDir * rushSpeed, parentRd.linearVelocity.y);
        DebugLogger.Log("ダッシュ");

        //アニメーションの再生位置を示す（開始0,終了１）
        //float broadcastTime = parentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        // 壁チェック
        RaycastHit2D hit = Physics2D.Raycast(parentRd.position, new Vector2(dashDir, 0), wallCheckDistance, groundLayer);
        Debug.DrawRay(parentRd.position, new Vector2(dashDir, 0), Color.red);
        if (!hasEndedDash && hit.collider != null)
        {
            //    EndDash();
            hasEndedDash = true;
            return;
        }

        // 距離チェック（Xだけ）
        float distance = Mathf.Abs(parentRd.position.x - dashStartPos.x);
        if (!hasEndedDash && distance >= rushDistance)
        {
            EndDash();
            hasEndedDash = true;
            return;
        }
    }

    //突進終了
    void EndDash()
    {
        //parentAnimator.SetBool("EndRush",true );
        //timer = 0;
        hasEndedDash = false;
        currentState = State.Recovery;
    }

    //攻撃後の硬直
    void Recovery()
    {
        //timer += Time.fixedDeltaTime;
        //アニメーションの再生位置を示す（開始0,終了１）
        //float broadcastTime = parentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        parentRd.linearVelocity = Vector2.zero;

        //if (timer >= recoveryTime)
        //if (broadcastTime >= 1)
        //{
        //    parentAnimator.SetBool("StartRush", false);
        //    currentState = State.Idle;
        //    slimeController.enabled = true;
        //    playerTracking.SetActive(true);
        //    gameObject.SetActive(false);
        //}
    }
}
