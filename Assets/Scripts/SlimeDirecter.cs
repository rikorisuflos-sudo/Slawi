using UnityEngine;

public class SlimeDirecter : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject player;//追いかける対象

    [SerializeField] private float walkForce;//速度の上昇値
    [SerializeField] private float jumpForce;//ジャンプの高さ
    [SerializeField] private float followDistance = 2.5f; // この距離より離れたら追う
    [SerializeField] private float stopDistance = 1.5f;   // この距離まで来たら止まる
    [SerializeField] private float warpDistance; // この距離より離れたら追う
    [SerializeField] private float smooth = 5f;           // ぬるっと具合

    private bool isGrounded = true;//地面に触れているかいないか
    private bool isWall= false;//地面に触れているかいないか

    private void FixedUpdate()
    {
        //自分とプレイヤーがどれだけ離れているか
        float distance = Vector2.Distance(transform.position, player.transform.position);

        float targetX = 0f;//移動距離

        if (distance > followDistance)
        {
            float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
            targetX = dir * walkForce;
        }
        else if (distance < stopDistance)
        {
            targetX = 0f;
        }
        // 今のY速度はそのまま維持！
        float newX = Mathf.Lerp(rb.linearVelocity.x, targetX, smooth * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);


        //一定距離離れたらプレイヤーのところにワープする
        if (Vector2.Distance(transform.position, player.transform.position) > warpDistance)
        {
            transform.position = player.transform.position;
        }

        // 壁に当たってたらジャンプ
        if (isGrounded && isWall)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)//接触点の取得
        {
            // 下から接触 → 地面
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                DebugLogger.Log("地面についた");
            }

            // 横から接触 → 壁
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                isWall = true;
            }
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //地面・壁から離れたかどうか
        if (collision.gameObject.CompareTag("Ground"))
        {           
                isGrounded = false;          
                isWall = false;

        }
    }
}

