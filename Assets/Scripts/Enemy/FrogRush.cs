using System;
using System.Collections;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class FrogRush : MonoBehaviour
{
    //親オブジェクトのものを取得
    [SerializeField] Rigidbody2D parentRd;
    [SerializeField] SlimeController slimeController;
    [SerializeField] Animator animator;

    [SerializeField] Transform playerTr;//プレイヤー
    [SerializeField] Player playerStatus;

    [SerializeField] float rushDistance = 20f;//攻撃距離
    [SerializeField] float rushSpeed;//攻撃速度
    float dashDir = 0;//ダッシュ方向

    Vector2 dashStartPos; // 突進開始位置

    [SerializeField] float wallCheckDistance = 0.5f;//壁の探索範囲
    [SerializeField] LayerMask groundLayer; // 地面レイヤー

    [SerializeField] LayerMask enemyLayer; // 地面レイヤー

    [SerializeField] GameObject playerTracking;//プレイヤー追従

    bool hasWarped = false;//ワープしたか
    //float timer = 0;
    bool isHit = false;//攻撃が当たったか
    enum State
    {
        Idle,//待機
        Charge,//攻撃前の溜め
        Rush,//突進
        Recovery//硬直
    }

    State currentState = State.Idle;

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Charge:
                Charge();
                break;

            case State.Rush:
                Rush();
                EndRush();
                break;

            case State.Recovery:
                Recovery();
                break;
        }
        //Debug.Log(currentState);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHit) return;//攻撃が複数回当たるのを防ぐ

        //エネミーに当たったらダメージ処理
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            isHit = true;
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(playerStatus.AttackPower);
            Debug.Log("エネミーに衝突した");
        }
    }

    //プレイヤー追従処理OFF
    //突進方向の確定、タイマーの初期化
    public void StartAttack()
    {
        animator.SetBool("StartRush", true);
        playerTracking.SetActive(false);
        slimeController.enabled = false;
        dashDir = Mathf.Sign(playerTr.transform.localScale.x);
        if (dashDir == 0) dashDir = 1;
        currentState = State.Charge;
    }
   

    //攻撃前の溜め
    void Charge()
    {
        if (currentState != State.Charge) return;
        parentRd.linearVelocity = Vector2.zero;
    }

    //突進
    void Rush()
    {
        parentRd.linearVelocity = new Vector2(dashDir * rushSpeed, parentRd.linearVelocity.y);

    }

    //突進終了
    void EndRush()
    {
        // 壁チェック
        RaycastHit2D hit = Physics2D.Raycast(parentRd.position, new Vector2(dashDir, 0), wallCheckDistance, groundLayer);
        Debug.DrawRay(parentRd.position, new Vector2(dashDir, 0), Color.red);
        if (hit.collider != null)
        {
            animator.SetBool("EndRush", true);
            currentState = State.Recovery;
            return;
        }

        // 距離チェック（Xだけ）
        float distance = Mathf.Abs(parentRd.position.x - dashStartPos.x);
        if (distance >= rushDistance)
        {
            animator.SetBool("EndRush", true);
            currentState = State.Recovery;
            return;
        }
    }

    //攻撃後の硬直
    void Recovery()
    {
        //アニメーションの再生位置を示す（開始0,終了１）
       　float broadcastTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        //移動を止める
        parentRd.linearVelocity = Vector2.zero;

        //初期化
        if (broadcastTime >= 1.0f)
        {
            animator.SetBool("EndRush", false);
            animator.SetBool("StartRush", false);
            hasWarped = false;
            slimeController.enabled = true;
            playerTracking.SetActive(true);
            gameObject.SetActive(false);
            isHit = false;
        }
    }

    //アニメーションイベントから渡された文字列をStateに変換
    public void CastState(string input)
    {
        if (Enum.TryParse(input, out State state))
        {
            currentState = state;
        }
    }
}
