using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class SlimeNormalAttack : MonoBehaviour
{
    private Rigidbody2D parentRd;//親オブジェクトのものを取得
    private Transform parentTr;
    private SlimeController slimeController;
    [SerializeField] private GameObject player;//プレイヤー
    [SerializeField] private float smooth = 5f;           // ぬるっと具合

    [SerializeField] private float rushDistance = 20f;//攻撃距離
    [SerializeField] private float rushSpeed = 20f;//攻撃速度
    float dashDir = 0;//ダッシュ方向

    [SerializeField] private float chargeTime = 0.3f;   // 溜め時間
    [SerializeField] private float recoveryTime = 0.3f; // 硬直時間


    private Vector2 dashStartPos; // 突進開始位置

    [SerializeField] private float wallCheckDistance = 0.5f;//壁の探索範囲
    [SerializeField] private LayerMask groundLayer; // 地面レイヤー

    [SerializeField] GameObject playerTracking;
    private float timer = 0;
    enum State
    {
        Idle,//待機
        Charge,//攻撃前の溜め
        Dash,//突進
        Recovery//硬直
    }

    State currentState = State.Idle;

    void Awake()
    {
        parentRd = GetComponentInParent<Rigidbody2D>();
        parentTr = transform.parent;
        slimeController = FindFirstObjectByType<SlimeController>();
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
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
        Vector2 warpPos = player.transform.position + new Vector3(dashDir * 1.5f, 0, 0);

        //ワープしたときに突進方向を向くようにする
        parentTr.localScale = new Vector3(dashDir, 1, 1);

        parentRd.position = warpPos;
        dashStartPos = parentRd.position;
    }

    //プレイヤー追従処理OFF
    //突進方向の確定、タイマーの初期化
    public void StartAttack()
    {
        playerTracking.SetActive(false);
        slimeController.enabled = false;
        dashDir = Mathf.Sign(player.transform.localScale.x);
        if (dashDir == 0) dashDir = 1;

        timer = 0;
        currentState = State.Charge;
    }
    //攻撃前の溜め
    void Charge()
    {
        timer += Time.fixedDeltaTime;

        parentRd.linearVelocity = Vector2.zero;

        if (timer >= chargeTime)
        {
            timer = 0;
            Warp();
            currentState = State.Dash;
        }
    }
    //突進
    void Dash()
    {
        parentRd.linearVelocity = new Vector2(dashDir * rushSpeed, parentRd.linearVelocity.y);

        // 壁チェック
        RaycastHit2D hit = Physics2D.Raycast(parentRd.position, new Vector2(dashDir, 0), wallCheckDistance, groundLayer);
        Debug.DrawRay(parentRd.position, new Vector2(dashDir, 0), Color.red);
        if (hit.collider != null)
        {
            EndDash();
            return;
        }

        // 距離チェック（Xだけ）
        float distance = Mathf.Abs(parentRd.position.x - dashStartPos.x);
        if (distance >= rushDistance)
        {
            EndDash();
        }
    }

    //突進終了
    void EndDash()
    {
        timer = 0;
        currentState = State.Recovery;
    }

    //攻撃後の硬直
    void Recovery()
    {
        timer += Time.fixedDeltaTime;

        parentRd.linearVelocity = Vector2.zero;

        if (timer >= recoveryTime)
        {
            currentState = State.Idle;
            slimeController.enabled = true;
            playerTracking.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
