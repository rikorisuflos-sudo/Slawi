using UnityEngine;

public class NormalAttack : MonoBehaviour
{

    //親オブジェクトのものを取得
    [SerializeField] Rigidbody2D parentRd;
    [SerializeField] SlimeController slimeController;

    [SerializeField] Transform playerTr;//プレイヤー
    [SerializeField] Player playerStatus;

    [SerializeField] float rushDistance = 20f;//攻撃距離
    [SerializeField] float rushSpeed;//攻撃速度
    float dashDir = 0;//ダッシュ方向

    Vector2 dashStartPos; // 突進開始位置

    [SerializeField] float wallCheckDistance = 0.5f;//壁の探索範囲
    [SerializeField] LayerMask groundLayer; // 地面レイヤー

    [SerializeField] LayerMask enemyLayer; // 地面レイヤー

    [SerializeField] GameObject playerTracking;//プレイヤー追従

    bool hasWarped = false;//ワープしたか
    //float timer = 0;
    bool isHit = false;//攻撃が当たったか

    void Awake()
    {
        Rush();
        EndRush();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHit) return;//攻撃が複数回当たるのを防ぐ

        //エネミーに当たったらダメージ処理
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            isHit = true;
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(playerStatus.AttackPower);
            Debug.Log("エネミーに衝突した");
        }
    }

    void Rush()
    {
        parentRd.linearVelocity = new Vector2(dashDir * rushSpeed, parentRd.linearVelocity.y);

    }

    //突進終了
    void EndRush()
    {
        // 壁チェック
        RaycastHit2D hit = Physics2D.Raycast(parentRd.position, new Vector2(dashDir, 0), wallCheckDistance, groundLayer);
        Debug.DrawRay(parentRd.position, new Vector2(dashDir, 0), Color.red);
        if (hit.collider != null)
        {
            //animator.SetBool("EndRush", true);
            //currentState = State.Recovery;
            return;
        }

        // 距離チェック（Xだけ）
        float distance = Mathf.Abs(parentRd.position.x - dashStartPos.x);
        if (distance >= rushDistance)
        {
            //animator.SetBool("EndRush", true);
            //currentState = State.Recovery;
            return;
        }
    }

    //指定されたタグの中で最も近いものを取得
    GameObject serchTag()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(parentRd.position, searchRange, enemyLayer);

        float nearDis = Mathf.Infinity;
        GameObject targetObj = null;

        foreach (Collider2D hit in hits)
        {
            float tmpDis = Vector2.Distance(hit.transform.position, parentRd.position);

            if (tmpDis < nearDis)
            {
                nearDis = tmpDis;
                targetObj = hit.gameObject;
            }
        }

        return targetObj;
    }
}
