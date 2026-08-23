using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
//ゲッタン修正中
public class EnemyMove : MonoBehaviour
{
    [SerializeField] Rigidbody2D parentRd; // エネミーのRigidbody2Dを取得 
    [SerializeField] Transform parentTr;
    [SerializeField] Enemy enemyStatus;//エネミーのScriptableObject
    [SerializeField] GameObject attack;
    [SerializeField] Animator animator;
    [SerializeField] Canvas hpBar;

    [SerializeField] float enemyMileage; // エネミーの移動範囲 
    float enemyDir; // エネミーの向き (初期値1) 
    float walkForce; // エネミーの移動速度 
    [SerializeField] float smooth;// ぬるっと具合
    //[SerializeField] float minimumSpeed;//最低速度保障

    [SerializeField] float checkDistance; // 地面検知範囲 
    [SerializeField] float wallCheckDistance;//壁検知範囲
    [SerializeField] LayerMask groundLayer; // 地面レイヤー 
    bool turnAround = false; //待機移動中に方向転換するか

    Vector2 startPosition;//エネミー初期位置
    Vector2 movPosition;//待機状態時の移動用変数

    [SerializeField] Transform playerTr;//検知対象（プレイヤー）
    float viewDistance = 5f;//視界距離（扇形の半径）
    float viewAngle = 60f;//視野角(見える角度）
    bool playerFind = false; // プレイヤーを見つけたか (初期値false) 

    float attackDistance;//攻撃範囲
    float attackAngle;//攻撃範囲

    bool stop = false;

    void OnEnable()
    {
        //playerFind = false;
        enemyDir = Mathf.Sign(parentTr.localScale.x);
    }

    void Start()
    {
        //エネミーの方向を取得
        enemyDir = Mathf.Sign(parentTr.localScale.x);
        //エネミーの初期位置を取得
        if (transform != null)
        {
            startPosition = parentTr.position;
            movPosition = startPosition;
        }

        //ScriptableObjectの値を変数に格納
        if (enemyStatus != null)
        {
            walkForce = enemyStatus.WalkForce;
            viewDistance = enemyStatus.ViewDistance;
            viewAngle = enemyStatus.ViewAngle;
            attackDistance = enemyStatus.AttackDistance;
            attackAngle = enemyStatus.AttackAngle;
        }
        //Debug.Log(walkForce+","+enemyStatus.WalkForce);
    }

    // 移動処理を呼び出す 
    void FixedUpdate()
    {
        bool callAttack = CallAttack();

        //アニメーションの再生位置を示す（開始0,終了１）
       　float broadcastTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        // プレイヤーを発見したら、待機状態を解除する（＝移動を停止する） 
        if (callAttack && broadcastTime >= 1.0f)
        {
            attack.SetActive(true);
            gameObject.SetActive(false);
            return;
        }

        Walk();
    }

    // 移動処理やそれに関連する処理を記述 
    void Walk()
    {
        if (stop)
        {
            parentRd.linearVelocity = new Vector2(0.0f, parentRd.linearVelocity.y);
            return;
        }

        playerFind = PlayerSearch();

        if (!playerFind)
        {
            turnAround = CheckTurn();
            // 現在の移動距離を計算
            float currentDistance = Vector3.Distance(movPosition, parentTr.position);

            // 往復および落下防止の判定
            // 一定範囲（enemyMileage）を超えたか、段差（stepFind）があった場合に反転
            if (currentDistance >= enemyMileage || turnAround)
            {
                //Vector2 turn = transform.parent.lossyScale;

                //turn.x = turn.x * -1;

                ////Debug.Log("方向転換");
                enemyDir *= -1;

                hpBar.transform.localScale = new Vector3(enemyDir * 0.01f, 0.01f, 1);

                movPosition = transform.position; // 反転地点を起点に更新

                // 見た目の向きを反転させる処理
                parentTr.localScale = new Vector3(transform.parent.lossyScale.x * enemyDir, transform.parent.lossyScale.y, 1);
                //transform.parent.lossyScale = new Vector3(turn.x,turn.y,1);
            }
        }
        else
        {

            //追跡中はプレイヤーの位置をもとに移動方向を決める
            enemyDir = Mathf.Sign(playerTr.position.x - parentTr.position.x);
        }


        float targetX = enemyDir * walkForce;

        // linearVelocityによる移動処理 
        parentRd.linearVelocity = new Vector2(targetX, parentRd.linearVelocity.y);
    }

    // 段差検知と壁検知のための処理を記述 (Raycastを用いる) 
    bool CheckTurn()
    {
        // 壁チェック
        RaycastHit2D wallFind = Physics2D.Raycast(parentRd.position, new Vector2(enemyDir, 0), wallCheckDistance, groundLayer);
        Debug.DrawRay(parentRd.position, new Vector2(enemyDir, 0), Color.red);
        if (wallFind.collider != null)
        {
            return true;
        }

        // 地面検知
        // エネミーの少し前方から下向きにRayを飛ばす
        Vector2 offset = new Vector2(enemyDir * 0.5f, 0); // 前方オフセット
        RaycastHit2D holeFind = Physics2D.Raycast(parentRd.position + offset, Vector2.down, checkDistance, groundLayer);
        if (holeFind.collider == null)
        {
            return true;
        }
        return false;
    }

    //プレイヤーを探索する
    bool PlayerSearch()
    {
        if (playerTr == null) return false;

        //プレイヤーの方向を取得
        Vector2 direction = (playerTr.position - parentTr.position);
        //プレイヤーまでの距離を算出
        float distance = direction.magnitude;
        //  距離チェック
        if (distance > viewDistance)
        {
            return false;
        }

        //エネミーの方向取得
        Vector2 facingDir = new Vector2(enemyDir, 0);
        //  プレイヤーの位置とエネミーの位置の角度差
        float angle = Vector2.Angle(facingDir, direction);
        //angleが視野角内に収まるかどうか
        if (angle > viewAngle / 2f)
        {
            return false;
        }

        //  Raycast（壁チェック）
        RaycastHit2D hit = Physics2D.Raycast(parentTr.position, direction.normalized, distance, groundLayer);
        //Debug.DrawRay(transform.position, direction.normalized * distance, Color.red);

        // 何かに当たったら＝壁に遮られている
        if (hit.collider != null)
        {
            return false;

        }
        // すべてクリア → 見えてる
        return true;
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

    public void CastBool(int input)
    {
        stop = Convert.ToBoolean(input);
    }
}
