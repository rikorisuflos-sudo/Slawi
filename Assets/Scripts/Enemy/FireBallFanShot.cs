using System;
using UnityEngine;

public class FireBallFanShot : MonoBehaviour
{
    [SerializeField] Enemy enemyStatus;//エネミーのスクリプタブルオブジェクト
    [SerializeField] Transform parentTr;//親のトランスフォーム
    [SerializeField] Animator animator;
    [SerializeField] GameObject move;

    [SerializeField] GameObject fireBall;//ファイアボールプレハブ
    [SerializeField] FireBall fireBallSc;//ファイアボールスクリプト
    float angle = 50.0f;
    float quantity;

    [SerializeField] Transform playerTr;//プレイヤートランスフォーム
    [SerializeField] LayerMask tagetLayer;

    float attackDistance;//攻撃範囲
    float attackAngle;//攻撃範囲

    float enemyDir;//エネミーの向き
    [SerializeField] LayerMask groundLayer;//地面レイヤー

    private void OnEnable()
    {
        //発射方向を取得
        enemyDir = Mathf.Sign(parentTr.localScale.x);

        animator.SetBool("Shot", true);
    }

    void Start()
    {
        //初期化
        if (enemyStatus != null)
        {
            this.attackDistance = enemyStatus.AttackDistance;
            this.attackAngle = enemyStatus.AttackAngle;
        }
    }

    private void OnDisable()
    {
        animator.SetBool("Shot", false);
        move.SetActive(true);
    }

    void FixedUpdate()
    {
        //アニメーションの再生位置を示す（開始0,終了１）
       　float broadcastTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (broadcastTime >= 1.0f)
        {
            //Debug.Log("攻撃終了");
            bool callAttack = CallAttack();
            if (!callAttack)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Shot()
    {
        int count = 0;
        while (count < 5)
        {
            //FireBall生成
            Debug.Log("火の玉" + count);
            GameObject newFireBoal = Instantiate(fireBall, new Vector3(parentTr.position.x + (0.2f * enemyDir), parentTr.position.y, -1), Quaternion.Euler(-enemyDir, 0, angle * enemyDir));

            //FireBallSc初期化
            fireBallSc = newFireBoal.GetComponent<FireBall>();
            fireBallSc.SetParameter(enemyStatus.AttackPower, angle, enemyDir, tagetLayer);
            count++;
            angle -= 25.0f;
        }
        count = 0;
        angle = 30;
    }

    bool CallAttack()
    {
        //プレイヤーの方向を取得
        Vector2 direction = (playerTr.position - parentTr.position);
        //プレイヤーまでの距離を算出
        float distance = direction.magnitude;
        //  距離チェック
        if (distance > attackDistance)
        {
            return false;
        }

        //エネミーの方向取得
        Vector2 facingDir = new Vector2(enemyDir, 0);
        //  プレイヤーの位置とエネミーの位置の角度差
        float angle = Vector2.Angle(facingDir, direction);
        //angleが視野角内に収まるかどうか
        if (angle > attackAngle / 2f)
        {
            return false;
        }

        return true;
    }
}