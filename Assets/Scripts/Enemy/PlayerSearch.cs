using UnityEngine;

public class EnemySearch : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float viewDistance = 5f;
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] private LayerMask obstacleLayer; // 壁用レイヤー

    private void Update()
    {
        bool get = IsInView();
        if (get)
        {
            DebugLogger.Log("プレイヤーを見つけた");
        }
    }

    bool IsInView()
    {
        Vector2 direction = (target.position - transform.position);
        float distance = direction.magnitude;
        //  距離チェック
        if (distance > viewDistance)
        {
            DebugLogger.Log("探索距離外にいる");
            return false;
        }

        //  角度チェック
        float angle = Vector2.Angle(transform.right, direction);
        if (angle > viewAngle / 2f)
        {
            DebugLogger.Log("探索角度が適正じゃなかった");
            return false;
        }

        //  Raycast（壁チェック）
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, obstacleLayer);
        Debug.DrawRay(transform.position, direction.normalized, Color.red);

        // 何かに当たったら＝壁に遮られている
        if (hit.collider != null)
        {
            DebugLogger.Log("障害物を見つけた");
            return false;

        }
            // すべてクリア → 見えてる
            return true;
    }
    
    //検知範囲を表示
    private void OnDrawGizmos()
    {
        // 色設定
        Gizmos.color = Color.yellow;

        // 扇形の中心（自分の位置）
        Vector3 pos = transform.position;

        // 左右の角度
        float halfAngle = viewAngle / 2f;

        // forward（2Dなら右方向）を基準に回転
        Vector3 leftDir = Quaternion.Euler(0, 0, halfAngle) * transform.right;
        Vector3 rightDir = Quaternion.Euler(0, 0, -halfAngle) * transform.right;

        // 扇の線（左右）
        Gizmos.DrawLine(pos, pos + leftDir * viewDistance);
        Gizmos.DrawLine(pos, pos + rightDir * viewDistance);

        // 円弧っぽくする（細かく線をつなぐ）
        int segments = 20;
        Vector3 prevPoint = pos + rightDir * viewDistance;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfAngle + (viewAngle / segments) * i;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * transform.right;
            Vector3 nextPoint = pos + dir * viewDistance;

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
