using UnityEngine;

public class AbsorbAttack : MonoBehaviour
{
    //親オブジェクトのものを取得
    [SerializeField] Rigidbody2D parentRd;
    [SerializeField] SlimeController slimeController;
    [SerializeField] Animator animator;

    //[SerializeField] Collider2D attackCollider;

    [SerializeField] Transform playerTr;//プレイヤー
    [SerializeField] Collider2D playerCol;
    [SerializeField] PlayerDirecter playerStatus;

    [SerializeField] float rushSpeed;//攻撃速度
    float dashDir = 0;//ダッシュ方向
    [SerializeField] float walkForce;
    [SerializeField] float smooth;
    [SerializeField] float angle;


    [SerializeField] float stopDistance;

    Vector2 dashStartPos; // 突進開始位置

    [SerializeField] float wallCheckDistance = 0.5f;//壁の探索範囲
    [SerializeField] LayerMask groundLayer; // 地面レイヤー

    [SerializeField] LayerMask enemyLayer; // エネミーレイヤー

    [SerializeField] GameObject playerTracking;//プレイヤー追従

    float timer = 0;
    [SerializeField] float recoveryTime;

    bool isHit = false;//攻撃が当たったか
    bool hasWarped = false;//ワープしたか
    enum State
    {
        Idle,//待機
        Warp,
        Attack,//突進
        Recovery//硬直
    }

    State currentState = State.Idle;

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Warp:
                Warp();
                break;
            case State.Attack:
                EndAttack();
                break;
            case State.Recovery:
                timer += Time.fixedDeltaTime;
                if (timer >= recoveryTime)
                {
                    Recovery();
                }
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
            IAbsorble absorble = collision.gameObject.GetComponent<IAbsorble>();
            Debug.Log("IAbsorbleを検出：" + absorble.GetType().Name);
            if (absorble != null) absorble.Absorb();
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(Mathf.FloorToInt(playerStatus.AttackPower * 0.7f));

            //enemyHp = collision.gameObject.GetComponent<EnemyDirecter>().enemyHp;


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
        //オブジェクト反転
        if (dashDir == 0) dashDir = 1;
        transform.localScale = new Vector3(dashDir, 1, 1);
        timer = 0.0f;
        currentState = State.Warp;
    }

    //プレイヤーの前に移動
    void Warp()
    {
        if (hasWarped) return;

        hasWarped = true;
        //ワープさせる
        Vector2 warpPos = new Vector2(playerTr.position.x, playerCol.bounds.min.y);

        //ワープしたときに突進方向を向くようにする
        transform.localScale = new Vector3(dashDir, 1, 1);

        parentRd.position = warpPos;
        dashStartPos = parentRd.position;
        Attack();
        currentState = State.Attack;
    }

    //攻撃
    void Attack()
    {
        // 角度をラジアンに変換
        float rad = angle * Mathf.Deg2Rad;

        // 角度から進行方向を取得
        Vector2 direction = new Vector2(
            Mathf.Cos(rad) * dashDir,
            Mathf.Sin(rad)
        );

        Vector2 targetVector = direction * rushSpeed;

        parentRd.linearVelocity = targetVector;

    }

    //突進終了
    void EndAttack()
    {
        // 壁チェック
        RaycastHit2D hit = Physics2D.Raycast(parentRd.position, new Vector2(dashDir, 0), wallCheckDistance, groundLayer);
        Debug.DrawRay(parentRd.position, new Vector2(dashDir, 0), Color.red);
        if (hit.collider != null)
        {
            ////移動を止める
            parentRd.linearVelocity = Vector2.zero;

            currentState = State.Recovery;
            return;
        }

        ////アニメーションの再生位置を示す（開始0,終了１）
        float broadcastTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (broadcastTime >= 1.0f)
        {
            ////移動を止める
            parentRd.linearVelocity = Vector2.zero;

            currentState = State.Recovery;
            return;
        }
    }

    //攻撃後の硬直
    void Recovery()
    {
       

        //初期化

        animator.SetBool("Attack", false);

        currentState = State.Idle;

        slimeController.IsAttacking = false;
        playerTracking.SetActive(true);
        gameObject.SetActive(false);
        isHit = false;
        hasWarped = false;

    }
}
