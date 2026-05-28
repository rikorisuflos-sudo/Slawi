using System.Xml.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FireBall : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;

    [SerializeField] float movwForce; // オブジェクトの移動速度
    [SerializeField] float smooth;//ヌルっと感を出す
    float dir;//発射方向

    [SerializeField] float moveDistance = 20f;//オブジェクトの移動距離上限
    Vector2 moveStartPos;//発射位置

    [SerializeField] LayerMask groundLayer; // 地面レイヤー
    [SerializeField] LayerMask playerLayer; // プレイヤーレイヤー
    [SerializeField] float checkDistance;//検知範囲

    int attackPower;//攻撃力

    bool isHit = false;//当たったかどうか

    void Start()
    {
        // 重力の影響を無効化
        rb.gravityScale = 0f;
    }

    //初期化
    public void SetParameter(int attackPower, float dir)
    {
        this.attackPower = attackPower;
        this.dir = dir;
    }
    void FixedUpdate()
    {
        Move();
        EndMove();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHit) return;//当たり判定が複数回になるのを防ぐ

        //プレイヤーに当たったらダメージ処理
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("対象レイヤーにヒット");
            isHit = true;
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(attackPower);
            gameObject.SetActive(false);
        }
    }

    void Move()
    {
        float targetX = movwForce * dir;
        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetX, smooth * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
    }

    void EndMove()
    {
        // 壁チェック
        RaycastHit2D hitWall = Physics2D.Raycast(rb.position, new Vector2(dir, 0), checkDistance, groundLayer);
        Debug.DrawRay(rb.position, new Vector2(dir, 0), Color.red);
        if (hitWall.collider != null)
        {
            this.gameObject.SetActive(false);
        }

        // 距離チェック（Xだけ）
        float distance = Mathf.Abs(rb.position.x - moveStartPos.x);
        if (distance >= moveDistance)
        {
            this.gameObject.SetActive(false);
        }
    }

}
