using UnityEngine;

public class EnemySearch : MonoBehaviour
{
    [SerializeField] Transform player;//検知対象（プレイヤー）
    [SerializeField] float viewDistance = 5f;//視界距離（扇形の半径）
    [SerializeField] float viewAngle = 60f;//視野角(見える角度）
    [SerializeField] LayerMask obstacleLayer; // 壁用レイヤー
    private float enemyDir = 1f; // エネミーの向き (初期値1) 

    void FixedUpdate()
    {
        enemyDir = Mathf.Sign(transform.localScale.x);
        bool get = IsInView();
        if (get)
        {
            DebugLogger.Log("プレイヤーを見つけた");
        }

    }

    //検知範囲を表示
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

    bool IsInView()
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, obstacleLayer);
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
