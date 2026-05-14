using UnityEngine;

public class FireBallShot : MonoBehaviour
{
    [SerializeField] Enemy enemyStatus;
    [SerializeField] Transform parentTr;

    [SerializeField] GameObject fireBall;
    [SerializeField] FireBall fireBallSc;
    [SerializeField] Transform enemyTr;
    [SerializeField] Transform playerTr;
    float attackDistance;
    float attackAngle;
    float enemyDir;
    [SerializeField] LayerMask groundLayer;
    bool withinRange = false;
    float timer = 0;
    [SerializeField] float interval;

    void Start()
    {
        enemyDir = enemyDir = Mathf.Sign(parentTr.localScale.x);
        if (enemyStatus != null)
        {
            this.attackDistance = enemyStatus.AttackDistance;
            this.attackAngle = enemyStatus.AttackAngle;
        }
    }

    void Update()
    {
        withinRange = PlayerSearch();
        timer += Time.deltaTime;
        if (withinRange && timer >= interval)
        {
            Debug.Log("射程内に入った");
            GameObject newFireBoal = Instantiate(fireBall, enemyTr.position, Quaternion.identity);

            fireBallSc = newFireBoal.GetComponent<FireBall>();
            fireBallSc.SetParameter(enemyStatus.AttackPower, enemyDir);

            timer = 0;
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
            Debug.Log(attackAngle);
            return false;
        }

        //  Raycast（壁チェック）
        RaycastHit2D hit = Physics2D.Raycast(parentTr.position, direction.normalized, distance, groundLayer);
        Debug.DrawRay(transform.position, direction.normalized * distance, Color.red);

        // 何かに当たったら＝壁に遮られている
        if (hit.collider != null)
        {
            Debug.Log("壁がある");
            return false;

        }
        // すべてクリア → 見えてる
        return true;
    }
}
