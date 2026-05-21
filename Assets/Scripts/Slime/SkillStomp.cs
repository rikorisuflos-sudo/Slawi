using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class SkillStomp : MonoBehaviour
{
    Rigidbody2D parentRd;//親オブジェクトのものを取得
    //private Transform parentTr;
    SlimeController slimeController;
    [SerializeField] Animator animator;
    [SerializeField] Transform player;//プレイヤー
    [SerializeField] Player playerStatus;

    [SerializeField] float stompSpeed = 20f;//攻撃速度
    [SerializeField] float stompDistance = 30f;//プレイヤーの位置よりどれだけ上にワープするか

    //[SerializeField] float chargeTime = 0.3f;   // 溜め時間
    //[SerializeField] float recoveryTime = 0.3f; // 硬直時間

    //[SerializeField] float wallCheckDistance = 0.5f;//壁の探索範囲
    [SerializeField] LayerMask groundLayer; // 地面レイヤー

    [SerializeField] float groundCheckDistance;

    [SerializeField] GameObject playerTracking;

    [SerializeField] float searchRange = 10f;
    [SerializeField] LayerMask enemyLayer;

    bool isHit = false;

    enum State
    {
        Idle,//待機
        Warp,//ワープ
        Charge,//攻撃前の溜め
        Stomp,//突進
        Recovery//硬直
    }

    State currentState = State.Idle;

    void Awake()
    {
        parentRd = GetComponentInParent<Rigidbody2D>();
        //parentTr = transform.parent;
        slimeController = FindFirstObjectByType<SlimeController>();
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

            case State.Stomp:
                Stomp();
                EndStomp();
                break;

            case State.Recovery:
                Recovery();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHit) return;

        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            isHit = true;
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(playerStatus.AttackPower);
        }
    }

    //プレイヤー追従処理OFF
    public void StartAttack()
    {
        animator.SetBool("StartStomp", true);
        playerTracking.SetActive(false);
        slimeController.enabled = false;
        currentState = State.Warp;
    }

    //プレイヤーの前に移動
    void Warp()
    {
        GameObject tagetEnemy = serchTag();
        //ワープさせる
        Vector2 warpPos = tagetEnemy.transform.position + new Vector3(0, stompDistance, 0);

        parentRd.position = warpPos;
        //dropStartPos = parentRd.position;
        currentState = State.Charge;
    }

    //攻撃前の溜め
    void Charge()
    {
        parentRd.linearVelocity = Vector2.zero;
    }

    //突進
    void Stomp()
    {
        parentRd.linearVelocity = new Vector2(parentRd.linearVelocityX, -stompSpeed);
    }

    //  落下終了
    void EndStomp()
    {
        // 床チェック
        RaycastHit2D hit = Physics2D.Raycast(parentRd.position, new Vector2(0, -1), groundCheckDistance, groundLayer);
        Debug.DrawRay(parentRd.position, new Vector2(0, -1), Color.red);
        if (hit.collider != null)
        {
            animator.SetBool("EndStomp", true);
            currentState = State.Recovery;
            return;
        }
    }

    //攻撃後の硬直
    void Recovery()
    {
        //アニメーションの再生位置を示す（開始0,終了１）
        float broadcastTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        parentRd.linearVelocity = Vector2.zero;

        if (broadcastTime >= 1)
        {
            currentState = State.Idle;
            slimeController.enabled = true;
            playerTracking.SetActive(true);
            gameObject.SetActive(false);
            isHit = false;
        }
    }

    //指定されたタグの中で最も近いものを取得
    GameObject serchTag()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.position, searchRange, enemyLayer);

        float nearDis = Mathf.Infinity;
        GameObject targetObj = null;

        foreach (Collider2D hit in hits)
        {
            float tmpDis = Vector2.Distance(hit.transform.position, player.position);

            if (tmpDis < nearDis)
            {
                nearDis = tmpDis;
                targetObj = hit.gameObject;
            }
        }

        return targetObj;
    }

    public void CastEnum(string input)
    {
        Debug.Log("CastEnum呼ばれた: " + input);

        if (Enum.TryParse(input, out State state))
        {
            currentState = state;
            Debug.Log(currentState);
        }
    }
}

