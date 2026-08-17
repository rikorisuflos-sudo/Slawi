using System;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NormalAttack : MonoBehaviour
{
    //親オブジェクトのものを取得
    [SerializeField] Rigidbody2D parentRd;
    [SerializeField] SlimeController slimeController;
    [SerializeField] Animator animator;

    [SerializeField] Collider2D attackCollider;

    [SerializeField] Transform playerTr;//プレイヤー
    [SerializeField] Player playerStatus;

    [SerializeField] float rushSpeed;//攻撃速度
    float dashDir = 0;//ダッシュ方向
    [SerializeField] float walkForce;
    [SerializeField] float smooth;

    [SerializeField] float searchRange;//エネミー探索範囲
    [SerializeField] float stopDistance;

    Vector2 dashStartPos; // 突進開始位置

    [SerializeField] float wallCheckDistance = 0.5f;//壁の探索範囲
    [SerializeField] LayerMask groundLayer; // 地面レイヤー

    [SerializeField] LayerMask enemyLayer; // エネミーレイヤー

    [SerializeField] GameObject playerTracking;//プレイヤー追従

    bool isHit = false;//攻撃が当たったか
    enum State
    {
        Idle,//待機
        Tracking,//追尾
        Charge,//攻撃前の溜め
        Attack,//突進
        Recovery//硬直
    }

    State currentState = State.Idle;

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Tracking:
                Tracking();
                break;
            case State.Attack:
                Attack();
                EndAttack();
                break;

            case State.Recovery:
                Recovery();
                break;
        }
        Debug.Log(currentState);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHit) return;//攻撃が複数回当たるのを防ぐ

        //エネミーに当たったらダメージ処理
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            isHit = true;
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(playerStatus.AttackPower);
            currentState = State.Recovery;

            Debug.Log("エネミーに衝突した");
        }
    }

    //プレイヤー追従処理OFF
    //突進方向の確定、タイマーの初期化
    public void StartAttack()
    {
        if (currentState != State.Idle) return;

       

        playerTracking.SetActive(false);
        slimeController.IsAttacking = true;
        dashDir = Mathf.Sign(playerTr.transform.localScale.x);
        animator.SetBool("Attack", true);
        if (dashDir == 0) dashDir = 1;
        //オブジェクト反転
        if (dashDir == 0) dashDir = 1;
        transform.localScale = new Vector3(dashDir, 1, 1);

        currentState = State.Tracking;
    }

    void Tracking()
    {
        GameObject target = serchTag();

        if (target == null)
        {
            dashStartPos = parentRd.transform.position;
            currentState = State.Attack;
            return;
        }
        ////自分とエネミーがどれだけ離れているか
        float distance = Vector2.Distance(parentRd.transform.position, target.transform.position);

        if (distance <= stopDistance)
        {
            dashStartPos = parentRd.transform.position;
            currentState = State.Attack;
            return;
        }

        float targetX = dashDir * walkForce;
        float newX = Mathf.Lerp(parentRd.linearVelocity.x, targetX, smooth * Time.fixedDeltaTime);
        parentRd.linearVelocity = new Vector2(newX, parentRd.linearVelocity.y);

    }

    //攻撃
    void Attack()
    {
        parentRd.linearVelocity = new Vector2(dashDir * rushSpeed, parentRd.linearVelocity.y);

    }

    //突進終了
    void EndAttack()
    {
        // 壁チェック
        RaycastHit2D hit = Physics2D.Raycast(parentRd.position, new Vector2(dashDir, 0), wallCheckDistance, groundLayer);
        Debug.DrawRay(parentRd.position, new Vector2(dashDir, 0), Color.red);
        if (hit.collider != null)
        {
            //animator.SetBool("EndRush", true);
            currentState = State.Recovery;
            return;
        }

        ////アニメーションの再生位置を示す（開始0,終了１）
        float broadcastTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (broadcastTime >= 1.0f)
        {
                currentState = State.Recovery;
                return;
        }
    }

    //攻撃後の硬直
    void Recovery()
    {
        ////移動を止める
        parentRd.linearVelocity = Vector2.zero;

        //初期化
        
            animator.SetBool("Attack", false);

            currentState = State.Idle;

            slimeController.IsAttacking = false;
            playerTracking.SetActive(true);
            gameObject.SetActive(false);
            isHit = false;

    }

    //指定されたタグの中で最も近いものを取得
    GameObject serchTag()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(playerTr.position, searchRange, enemyLayer);

        float nearDis = Mathf.Infinity;
        GameObject targetObj = null;

        // プレイヤーが向いている方向
        Vector2 facingDir = new Vector2(dashDir, 0);   // 右:1 左:-1

        foreach (Collider2D hit in hits)
        {
            // プレイヤー→敵へのベクトル
            Vector2 direction = hit.transform.position - playerTr.position;

            // プレイヤーとの距離
            float tmpDis = direction.magnitude;

            // プレイヤーの向きと敵への方向の角度
            float angle = Vector2.Angle(facingDir, direction);

            // 前方90°(左右45°)だけを対象にする
            if (angle > 45f)
                continue;

            // 一番近い敵を選択
            if (tmpDis < nearDis)
            {
                nearDis = tmpDis;
                targetObj = hit.gameObject;
            }
        }

        return targetObj;
    }
}
