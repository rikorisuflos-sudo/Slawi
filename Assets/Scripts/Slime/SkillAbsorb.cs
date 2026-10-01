using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkillAbsorb : MonoBehaviour
{
    [SerializeField] Rigidbody2D parentRd;
    [SerializeField] Transform parentTr;//親のトランスフォーム
    [SerializeField] SlimeController slimeController;

    [SerializeField] Transform playerTr;//プレイヤー
    [SerializeField] PlayerDirecter playerStatus;
    [SerializeField] GameObject playerTracking;//プレイヤー追従

    bool hasWarped = false;//ワープしたか

    float timer = 0;
    [SerializeField] float interval;

    [SerializeField] GameObject fireBall;//ファイアボールプレハブ
    [SerializeField] FireBall fireBallSc;//ファイアボールスクリプト
    const float Angle = 0;//発射角度（前方のみのため固定）

    [SerializeField] LayerMask tagetLayer;

    float dir;
    enum State
    {
        Idle,//待機
        Warp,//ワープ
        Charge,//攻撃前の溜め
        FireBallShot,//火の玉
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
                case State.Charge:
                    Charge();
                    break;

                case State.FireBallShot:
                    FireBallShot();
                    break;

                case State.Recovery:
                    Recovery();
                    break;
            }
            Debug.Log(currentState);
    }

    public void StartAttack()
    {
        //animator.SetBool("StartRush", true);
        playerTracking.SetActive(false);
        slimeController.IsAttacking = true;
        dir = Mathf.Sign(playerTr.transform.localScale.x);
        if (dir == 0) dir = 1;
        currentState = State.Warp;
    }

    //プレイヤーの前に移動
    void Warp()
    {
        if (hasWarped) return;

        hasWarped = true;
        //ワープさせる
        Vector2 warpPos = playerTr.transform.position + new Vector3(dir * 1.5f, 0, 0);

        //ワープしたときに突進方向を向くようにする
        transform.localScale = new Vector3(dir, 1, 1);

        parentRd.position = warpPos;
        currentState = State.Charge;
    }

    void Charge()
    {
        if (currentState != State.Charge) return;
        parentRd.linearVelocity = Vector2.zero;
        currentState = State.FireBallShot;
    }

    void FireBallShot()
    {
            //ファイアボールの生成
            Debug.Log("火の玉");
            GameObject newFireBoal = Instantiate(fireBall, parentTr.position, Quaternion.identity);

            //ダメージ処理
            fireBallSc = newFireBoal.GetComponent<FireBall>();
            fireBallSc.SetParameter(Mathf.FloorToInt(playerStatus.AttackPower * 1.7f), Angle ,dir,tagetLayer);

        currentState = State.Recovery;
    }

    void Recovery()
    {
        //アニメーションの再生位置を示す（開始0,終了１）
       　//float broadcastTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        //移動を止める
        parentRd.linearVelocity = Vector2.zero;

        //初期化
        //if (broadcastTime >= 1.0f)
        //攻撃間隔
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            //animator.SetBool("EndRush", false);
            //animator.SetBool("StartRush", false);
            hasWarped = false;
            slimeController.IsAttacking = false;
            playerTracking.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
