using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class SkillAttack : MonoBehaviour
{
    private Rigidbody2D parentRd;//親オブジェクトのものを取得
    //private Transform parentTr;
    private SlimeController slimeController;
    [SerializeField] private GameObject player;//プレイヤー
    [SerializeField] private float smooth = 5f;           // ぬるっと具合

    [SerializeField] private float dropSpeed = 20f;//攻撃速度
    [SerializeField] private float stompDistance= 30f;//プレイヤーの位置よりどれだけ上にワープするか
    float dashDir = 0;//ダッシュ方向

    [SerializeField] private float chargeTime = 0.3f;   // 溜め時間
    [SerializeField] private float recoveryTime = 0.3f; // 硬直時間

    private bool isGrounded = false;
    private Vector2 dropStartPos; // 突進開始位置

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
        //parentTr = transform.parent;
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
                Drop();
                break;

            case State.Recovery:
                Recovery();
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)//接触点の取得
        {
            // 下から接触 → 地面
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
            }
        }
    }
    //プレイヤーの前に移動
    void Warp()
    {
        GameObject tagetEnemy = serchTag();
        //ワープさせる
        Vector2 warpPos = tagetEnemy.transform.position + new Vector3(0, stompDistance, 0);

        parentRd.position = warpPos;
        dropStartPos = parentRd.position;
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
    void Drop()
    {
        parentRd.linearVelocity = new Vector2(parentRd.linearVelocityX, -dropSpeed);

        //// 壁チェック
        //RaycastHit2D hit = Physics2D.Raycast(parentRd.position, new Vector2(dashDir, 0), wallCheckDistance, groundLayer);
        //Debug.DrawRay(parentRd.position, new Vector2(dashDir, 0), Color.red);
        //if (hit.collider != null)
        //{
        //    EndDash();
        //    return;
        //}
        if (isGrounded) EndDrop();
    }

    //  落下終了
    void EndDrop()
    {
        timer = 0;
        isGrounded = false;
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

//指定されたタグの中で最も近いものを取得
GameObject serchTag()
    {
        float tmpDis = 0;           //距離用一時変数
        float nearDis = 0;          //最も近いオブジェクトの距離
        GameObject targetObj = null; //オブジェクト

        //タグ指定されたオブジェクトを配列で取得する
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            //自身と取得したオブジェクトの距離を取得
            tmpDis = Vector3.Distance(obs.transform.position, player.transform.position);

            //オブジェクトの距離が近いか、距離0であればオブジェクト名を取得
            //一時変数に距離を格納
            if (nearDis == 0 || nearDis > tmpDis)
            {
                nearDis = tmpDis;
                targetObj = obs;
            }

        }
        //最も近かったオブジェクトを返す
        return targetObj;
    }
}
