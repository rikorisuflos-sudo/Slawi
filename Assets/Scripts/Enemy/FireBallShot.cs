using System;
using UnityEngine;

public class FireBallShot : MonoBehaviour
{
    [SerializeField] Enemy enemyStatus;//エネミーのスクリプタブルオブジェクト
    [SerializeField] Transform parentTr;//親のトランスフォーム
    [SerializeField] Animator animator;
    [SerializeField] GameObject move;

    [SerializeField] GameObject fireBall;//ファイアボールプレハブ
    [SerializeField] FireBall fireBallSc;//ファイアボールスクリプト

    [SerializeField] Transform playerTr;//プレイヤートランスフォーム

    float attackDistance;//攻撃範囲
    float attackAngle;//攻撃範囲

    float enemyDir;//エネミーの向き
    [SerializeField] LayerMask groundLayer;//地面レイヤー
    bool withinRange = false;//攻撃範囲に入ったか

    bool shot = false;

    ////攻撃間隔
    //float timer = 0;
    //[SerializeField] float interval;

    void Start()
    {
        //発射方向を取得
        enemyDir = Mathf.Sign(parentTr.localScale.x);


        //初期化
        if (enemyStatus != null)
        {
            this.attackDistance = enemyStatus.AttackDistance;
            this.attackAngle = enemyStatus.AttackAngle;
        }
    }

    void FixedUpdate()
    {
        //攻撃範囲に入ったか
        withinRange = PlayerSearch();

        //攻撃間隔
        //timer += Time.deltaTime;
        //if (withinRange && timer >= interval)
        if (withinRange)
        {

            animator.SetBool("Shot", true);
            move.SetActive(false);

            Shot();

        }
        else
        {
            animator.SetBool("Shot", false);
            move.SetActive(true);
        }
    }

    void Shot()
    {

        if (shot)
        {
            //ファイアボールの生成
            Debug.Log("火の玉");
            GameObject newFireBoal = Instantiate(fireBall, new Vector3(parentTr.position.x + (0.2f * enemyDir), parentTr.position.y, -1), Quaternion.identity);

            //ダメージ処理
            fireBallSc = newFireBoal.GetComponent<FireBall>();
            fireBallSc.SetParameter(enemyStatus.AttackPower, enemyDir);

            shot = false;
        }
    }

    bool PlayerSearch()
    {
        if (playerTr == null)
            return false;

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

        //  Raycast（壁チェック）
        RaycastHit2D hit = Physics2D.Raycast(parentTr.position, direction.normalized, distance, groundLayer);
        Debug.DrawRay(transform.position, direction.normalized * distance, Color.red);

        // 何かに当たったら＝壁に遮られている
        if (hit.collider != null)
        {
            return false;

        }
        // すべてクリア → 見えてる
        return true;
    }

    public void CastBool(int input)
    {
        shot = Convert.ToBoolean(input);
    }
}
