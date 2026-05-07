using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] Rigidbody2D parentRd; // エネミーのRigidbody2Dを取得 
    //[SerializeField] private new Transform transform; // エネミーのTransformを取得
    //[SerializeField] private Transform enemyTr; 
    [SerializeField] float enemyMileage; // エネミーの移動範囲 
    float enemyDir = 1f; // エネミーの向き (初期値1) 
    [SerializeField] float walkForce; // エネミーの移動速度 
    [SerializeField] float smooth = 5f;           // ぬるっと具合
    [SerializeField]float minimumSpeed = 0.5f;//最低速度保障
    bool playerFind = false; // プレイヤーを見つけたか (初期値false) 
    bool turnAround;
    [SerializeField] float checkDistance; // 地面検知範囲 
    [SerializeField] float wallCheckDistance;//壁検知範囲
    [SerializeField] LayerMask groundLayer; // 地面レイヤー 

    Vector2 startPosition;//エネミー初期位置
    Vector2 movPosition;//待機状態時の移動用変数

    [SerializeField] Transform playerTr;//検知対象（プレイヤー）
    [SerializeField] float viewDistance = 5f;//視界距離（扇形の半径）
    [SerializeField] float viewAngle = 60f;//視野角(見える角度）

    
    private void OnEnable()
    {
        playerFind = false;
        enemyDir = 1f;
    }

    private void Start()
    {
        enemyDir = Mathf.Sign(transform.localScale.x);
        //エネミーの初期位置を取得
        if (transform != null)
        {
            startPosition = transform.position;
        }
        movPosition = startPosition;
    }

    // 移動処理を呼び出す 
    private void FixedUpdate()
    {
        playerFind = PlayerSearch();
        // プレイヤーを発見したら、待機状態を解除する（＝移動を停止する） 
        if (!playerFind)
        {
            Walk();
        }
        else
        {
            //移動先をエネミーの初期位置に
            if (movPosition != startPosition ) movPosition = startPosition;
            //プレイヤーの位置をもとに移動方向を決める
            float dir = Mathf.Sign(playerTr.position.x - transform.position.x);

            float targetX = dir * walkForce * minimumSpeed;
            float newX = Mathf.MoveTowards(parentRd.linearVelocity.x, targetX, smooth * Time.fixedDeltaTime);
            parentRd.linearVelocity = new Vector2(newX, parentRd.linearVelocity.y);
        }
    }

    // 移動処理やそれに関連する処理を記述 
    private void Walk()
    {
        turnAround = CheckTurn();
        // 現在の移動距離を計算
        float currentDistance = Vector3.Distance(movPosition, transform.position);

        // 往復および落下防止の判定
        // 一定範囲（enemyMileage）を超えたか、段差（stepFind）があった場合に反転
        if (currentDistance >= enemyMileage || turnAround)
        {
            enemyDir *= -1;
            movPosition = transform.position; // 反転地点を起点に更新

            // 見た目の向きを反転させる処理
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * enemyDir;
            transform.localScale = scale;
        }

        // linearVelocityによる移動処理 
        parentRd.linearVelocity = new Vector2(enemyDir * walkForce, parentRd.linearVelocity.y);
    }

    // 段差検知のための処理を記述 (Raycastを用いる) 
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

    bool PlayerSearch()
    {
        if (playerTr == null) return false;

        //プレイヤーの方向を取得
        Vector2 direction = (playerTr.position - transform.position);
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, groundLayer);
        Debug.DrawRay(transform.position, direction.normalized * distance, Color.red);

        // 何かに当たったら＝壁に遮られている
        if (hit.collider != null)
        {
            return false;

        }
        // すべてクリア → 見えてる
        return true;
    }

}
