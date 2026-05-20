using System;
using System.Collections;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class SkillRush : MonoBehaviour
{
    //親オブジェクトのものを取得
    Rigidbody2D parentRd;

    [SerializeField] SlimeController slimeController;
    [SerializeField] Transform playerTr;//プレイヤー
    [SerializeField] Animator animator;
    [SerializeField] Player playerStatus;

    [SerializeField] float rushDistance = 20f;//攻撃距離
    [SerializeField] float rushSpeed;//攻撃速度
    float dashDir = 0;//ダッシュ方向

    //[SerializeField]  float chargeTime = 0.3f;   // 溜め時間
    //[SerializeField]  float recoveryTime = 0.3f; // 硬直時間
    float broadcastTime = 0f;

    Vector2 dashStartPos; // 突進開始位置

    [SerializeField] float wallCheckDistance = 0.5f;//壁の探索範囲
    [SerializeField] LayerMask groundLayer; // 地面レイヤー

    [SerializeField] LayerMask enemyLayer; // 地面レイヤー

    [SerializeField] GameObject playerTracking;
    bool hasWarped = false;
    //float timer = 0;
    bool isHit;
    enum State
    {
        Idle,//待機
        Warp,//ワープ
        Charge,//攻撃前の溜め
        Rush,//突進
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
        if (isHit) return;

        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            isHit = true;
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(playerStatus.AttackPower);
            gameObject.SetActive(false);
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
        currentState = State.Warp;
    }

    //プレイヤーの前に移動
    void Warp()
    {
        if (hasWarped) return;

        hasWarped = true;
        //ワープさせる
        Vector2 warpPos = playerTr.transform.position + new Vector3(dashDir * 1.5f, 0, 0);

        //ワープしたときに突進方向を向くようにする
        transform.localScale = new Vector3(dashDir, 1, 1);

        parentRd.position = warpPos;
        dashStartPos = parentRd.position;
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
        broadcastTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        parentRd.linearVelocity = Vector2.zero;

        if (broadcastTime >= 1.0f)
        {
            animator.SetBool("EndRush", false);
            animator.SetBool("StartRush", false);
            hasWarped = false;
            slimeController.enabled = true;
            playerTracking.SetActive(true);
            gameObject.SetActive(false);
            Debug.Log("owari");
        }
    }

    public void CastEnum(string input)
    {
        if (Enum.TryParse(input, out State state))
        {
            currentState = state;
        }
    }
}
