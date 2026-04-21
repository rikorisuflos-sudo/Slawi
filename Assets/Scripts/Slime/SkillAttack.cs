using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class SkillAttack : MonoBehaviour
{
    Rigidbody2D parentRd;//親オブジェクトのものを取得
    //private Transform parentTr;
    SlimeController slimeController;
    [SerializeField] Animator animator;
    [SerializeField] Transform player;//プレイヤー

    [SerializeField] float stompSpeed = 20f;//攻撃速度
    [SerializeField] float stompDistance = 30f;//プレイヤーの位置よりどれだけ上にワープするか

    //[SerializeField] float chargeTime = 0.3f;   // 溜め時間
    //[SerializeField] float recoveryTime = 0.3f; // 硬直時間

    //[SerializeField] float wallCheckDistance = 0.5f;//壁の探索範囲
    [SerializeField] LayerMask groundLayer; // 地面レイヤー

    [SerializeField] GameObject playerTracking;

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
        Debug.Log(currentState);
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
                break;

            case State.Recovery:
                Recovery();
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)//接触点の取得
        {
            // 下から接触 → 地面
            if (contact.normal.y > 0.5f && currentState == State.Stomp)
            {
                EndStomp();
            }
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
        animator.SetBool("EndStomp", true);
        currentState = State.Recovery;
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
            tmpDis = Vector3.Distance(obs.transform.position, player.position);

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

