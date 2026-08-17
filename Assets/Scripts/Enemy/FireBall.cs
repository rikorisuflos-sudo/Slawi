using System.Xml.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FireBall : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;

    [SerializeField] float movwForce; // オブジェクトの移動速度
    [SerializeField] float smooth;//ヌルっと感を出す
    float dir;//発射方向
    Vector2 direction;//発射角度

    [SerializeField] float moveDistance = 20f;//オブジェクトの移動距離上限
    Vector2 moveStartPos;//発射位置

    [SerializeField] LayerMask groundLayer; // 地面レイヤー
    LayerMask tagetLayer; // 攻撃対象レイヤー
    [SerializeField] float checkDistance;//検知範囲

    int attackPower;//攻撃力

    bool isHit = false;//当たったかどうか

    void Start()
    {
        // 重力の影響を無効化
        rb.gravityScale = 0f;
    }

    //初期化
    public void SetParameter(int attackPower, float angle , float dir,LayerMask tagetLayer)
    {
        //ダメージ設定
        this.attackPower = attackPower;

        //発射角度設定
        // 角度をラジアンに変換
        float rad = angle * Mathf.Deg2Rad;

        // 角度から進行方向を取得
        this.direction = new Vector2(
            Mathf.Cos(rad) * dir,
            Mathf.Sin(rad)
        );

        //スプライトの向きの設定
        transform.localScale = new Vector3(dir, transform.localScale.y, transform.localScale.z); 
        this.tagetLayer = tagetLayer;
        //transform.localScale = new Vector3(dir * 1, 1, -1);
        moveStartPos = transform.position;
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
        if (((1 << collision.gameObject.layer) & tagetLayer) != 0)
        {
            //Debug.Log("対象レイヤーにヒット");
            isHit = true;
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(attackPower);
            Destroy(gameObject);
        }
    }

    void Move()
    {
        


        // 目標速度を計算
        Vector2 targetVelocity = direction * movwForce;

        // 現在速度から目標速度へ滑らかに近づける
        Vector2 newVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            targetVelocity,
            smooth * Time.fixedDeltaTime
        );


        //移動

        rb.linearVelocity = newVelocity;

       
    }

    void EndMove()
    {
        // 壁チェック
        RaycastHit2D hitWall = Physics2D.Raycast(rb.position,direction.normalized,checkDistance,groundLayer);
        Debug.DrawRay(rb.position,direction.normalized * checkDistance,Color.red);
        if (hitWall.collider != null)
        {
            Destroy(gameObject);
        }

        // 距離チェック（Xだけ）
        float distance = Mathf.Abs(rb.position.x - moveStartPos.x);
        if (distance >= moveDistance)
        {
            Destroy(gameObject);
        }
    }

}
