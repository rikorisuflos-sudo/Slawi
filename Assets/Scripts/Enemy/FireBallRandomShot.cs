using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireBallRandomShot : MonoBehaviour
{
    [SerializeField] Enemy enemyStatus;//エネミーのスクリプタブルオブジェクト
    //[SerializeField] Transform parentTr;//親のトランスフォーム
    [SerializeField] Animator animator;
    [SerializeField] GameObject move;

    [SerializeField] BossActionLottery bossActionLottery;

    [SerializeField] GameObject fireBall;//ファイアボールプレハブ
    [SerializeField] FireBall fireBallSc;//ファイアボールスクリプト
    float angle = 50.0f;
    int count = 0;
    float timer = 0.0f;
    bool isShooting = false;

    [SerializeField] Transform playerTr;//プレイヤートランスフォーム
    [SerializeField] LayerMask tagetLayer;

    float enemyDir;//エネミーの向き
    [SerializeField] LayerMask groundLayer;//地面レイヤー

    private void OnEnable()
    {
        //発射方向を取得
        enemyDir = Mathf.Sign(transform.parent.localScale.x);

        bossActionLottery.IsAttacking = true;
        animator.SetBool("Shot", true);
    }

    private void OnDisable()
    {
        move.SetActive(true);
        animator.SetBool("Shot", false);
    }

    void FixedUpdate()
    {


        //アニメーションの再生位置を示す（開始0,終了１）
        float broadcastTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (broadcastTime >= 1.0f)
        {
            bossActionLottery.IsAttacking = false;
            enabled = false;
            Debug.Log("攻撃終了");
            //bool callAttack = CallAttack();
            //if (!callAttack)
            //{
            //    gameObject.SetActive(false);
            //}
        }

        if (!isShooting) return;
        timer += Time.fixedDeltaTime;
        if (timer >= 0.5f)
        {
            timer = 0.0f;

            ShootFireBall();
        }
    }

    // アニメーションイベントから呼ぶ
    public void Shot()
    {
        animator.speed = 0;

        count = 0;
        timer = 0;
        isShooting = true;

        // 1発目をすぐ発射
        ShootFireBall();
    }

    // 実際の火の玉生成
    void ShootFireBall()
    {
        angle = Random.Range(-5, 6) * 5;

        //Debug.Log("火の玉 " + count + " 角度：" + angle);

        GameObject newFireBoal = Instantiate(
            fireBall,
            new Vector3(
                transform.parent.position.x + (0.2f * enemyDir),
                transform.parent.position.y,
                -1
            ),
            Quaternion.Euler(
                -enemyDir,
                0,
                angle * enemyDir
            )
        );

        fireBallSc = newFireBoal.GetComponent<FireBall>();

        fireBallSc.SetParameter(
            enemyStatus.AttackPower,
            angle,
            enemyDir,
            tagetLayer
        );

        count++;

        if (count >= 5)
        {
            isShooting = false;
            count = 0;
            animator.speed = 1;
        }
    }

    //    bool CallAttack()
    //{
    //    //プレイヤーの方向を取得
    //    Vector2 direction = (playerTr.position - transform.parent.position);
    //    //プレイヤーまでの距離を算出
    //    float distance = direction.magnitude;
    //    //  距離チェック
    //    if (distance > attackDistance)
    //    {
    //        return false;
    //    }

    //    //エネミーの方向取得
    //    Vector2 facingDir = new Vector2(enemyDir, 0);
    //    //  プレイヤーの位置とエネミーの位置の角度差
    //    float angle = Vector2.Angle(facingDir, direction);
    //    //angleが視野角内に収まるかどうか
    //    if (angle > attackAngle / 2f)
    //    {
    //        return false;
    //    }

    //    return true;
    //}
}