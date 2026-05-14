using System.Xml.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FireBall : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;

    [SerializeField] float movwForce; // オブジェクトの移動速度
    [SerializeField] float smooth;//ヌルっと感を出す
    //[SerializeField] float moveDistance = 20f;//オブジェクトの移動距離上限
    //Vector2 moveStartPos;
    float dir = 1;
    [SerializeField] LayerMask groundLayer; // 地面レイヤー
    [SerializeField] LayerMask playerLayer; // 地面レイヤー
    [SerializeField] float checkDistance;

    int attackPower;

    void Start()
    {
        // 重力の影響を無効化
        rb.gravityScale = 0f;
    }

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

        //プレイヤーチェック
        RaycastHit2D hitPlayer = Physics2D.Raycast(rb.position, new Vector2(dir, 0), checkDistance, playerLayer);
        Debug.DrawRay(rb.position, new Vector2(dir, 0), Color.red);
        if (hitPlayer.collider != null)
        {
            this.gameObject.SetActive(false);
        }

        //// 距離チェック（Xだけ）
        //float distance = Mathf.Abs(rb.position.x - dashStartPos.x);
        //if (distance >= rushDistance)
        //{
        //    this.gameObject.SetActive(false);
        //}
    }

}
