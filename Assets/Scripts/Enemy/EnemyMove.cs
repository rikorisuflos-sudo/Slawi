using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid2D; // エネミーのRigidbody2Dを取得 
    //[SerializeField] private new Transform transform; // エネミーのTransformを取得
    //[SerializeField] private Transform enemyTr; 
    [SerializeField] private float enemyMileage; // エネミーの移動範囲 
    private float enemyDir = 1f; // エネミーの向き (初期値1) 
    [SerializeField] private float walkForce; // エネミーの移動速度 
    private bool playerFind = false; // プレイヤーを見つけたか (初期値false) 
    private bool stepFind = false; // 段差があるか (初期値false) 
    [SerializeField] private float checkDistance; // 地面検知範囲 
    [SerializeField] private LayerMask groundLayer; // 地面レイヤー 

    // 移動範囲制御のための内部変数 
    private Vector3 startPosition;

    [SerializeField] Transform player;//検知対象（プレイヤー）
    [SerializeField] float viewDistance = 5f;//視界距離（扇形の半径）
    [SerializeField] float viewAngle = 60f;//視野角(見える角度）

    private void OnEnable()
    {
        playerFind = false;
        stepFind = false;
        enemyDir = 1f;
    }

    private void Start()
    {
        //エネミーの初期位置を取得
        if (transform != null)
        {
            startPosition = transform.position;
        }
        
    }

    // 移動処理を呼び出す 
    private void FixedUpdate()
    {
        enemyDir = Mathf.Sign(transform.localScale.x);
        playerFind = PlayerSearch();
        // プレイヤーを発見したら、待機状態を解除する（＝移動を停止する） 
        if (!playerFind)
        {
            Walk();
        }
        else
        {
            // 発見時はその場に留まる
            rigid2D.linearVelocity = new Vector2(0, rigid2D.linearVelocity.y);
        }
    }

    // 移動処理やそれに関連する処理を記述 
    private void Walk()
    {
        // 段差検知の更新 
        SearchStep();

        // 現在の移動距離を計算
        float currentDistance = Vector3.Distance(startPosition, transform.position);

        // 往復および落下防止の判定
        // 一定範囲（enemyMileage）を超えたか、段差（stepFind）があった場合に反転
        if (currentDistance >= enemyMileage || stepFind)
        {
            enemyDir *= -1;
            startPosition = transform.position; // 反転地点を起点に更新

            // 見た目の向きを反転させる処理
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * enemyDir;
            transform.localScale = scale;
        }

        // linearVelocityによる移動処理 
        rigid2D.linearVelocity = new Vector2(enemyDir * walkForce, rigid2D.linearVelocity.y);
    }

    // 段差検知のための処理を記述 (Raycastを用いる) 
    private void SearchStep()
    {
        // エネミーの少し前方から下向きにRayを飛ばす
        Vector2 rayOrigin = transform.position;
        Vector2 offset = new Vector2(enemyDir * 0.5f, 0); // 前方オフセット

        // 地面検知の実行
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin + offset, Vector2.down, checkDistance, groundLayer);

        // Rayが地面に当たっていない（＝穴がある）場合は段差ありと判定 
        if (hit.collider == null)
        {
            stepFind = true;
        }
        else
        {
            stepFind = false;
        }
    }

    void OnDrawGizmos()
    {
        // 色設定
        Gizmos.color = Color.yellow;

        // 扇形の中心（自分の位置）
        Vector3 pos = transform.position;

        // 左右の角度
        float halfAngle = viewAngle / 2f;

        //エネミーの向き
        Vector3 facingDir = new Vector3(enemyDir, 0, 0);

        // forward（2Dなら右方向）を基準に回転
        Vector3 leftDir = Quaternion.Euler(0, 0, halfAngle) * facingDir;
        Vector3 rightDir = Quaternion.Euler(0, 0, -halfAngle) * facingDir;

        // 扇の線（左右）
        Gizmos.DrawLine(pos, pos + leftDir * viewDistance);
        Gizmos.DrawLine(pos, pos + rightDir * viewDistance);

        // 円弧っぽくする（細かく線をつなぐ）
        int segments = 20;
        Vector3 prevPoint = pos + rightDir * viewDistance;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfAngle + (viewAngle / segments) * i;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * facingDir;
            Vector3 nextPoint = pos + dir * viewDistance;

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
    bool PlayerSearch()
    {
        if (player == null) return false;

        //プレイヤーの方向を取得
        Vector2 direction = (player.position - transform.position);
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
