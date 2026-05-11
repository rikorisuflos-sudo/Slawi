using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerTracking : MonoBehaviour
{
    Rigidbody2D parentRd;//親オブジェクトのものを取得
    //Transform parentTr;
    [SerializeField] Transform playerTr;//追いかける対象
    [SerializeField] PlayerController playerController;

    [SerializeField] float walkForce;//速度の上昇値
    [SerializeField] float jumpForce;//ジャンプの高さ
    [SerializeField] float jampDelay;
    float nextJumpTime = 0f;
    [SerializeField] float followDistance; // この距離より離れたら追う
    [SerializeField] float stopDistance;   // この距離まで来たら止まる
    [SerializeField] float warpDistance; // この距離より離れたら追う
    [SerializeField] float smooth = 5f;           // ぬるっと具合

    const float GroundCatch = 0.5f;
    private bool isGrounded = true;//地面に触れているかいないか

    float targetX = 0f;//目標座標
    float heightDiff;

    bool canMove = true;//追従処理のON.OFF
    public bool CanMove
    {
        set { canMove = value; }
    }
    private void Start()
    {
        parentRd = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!canMove) return;

        //自分とプレイヤーがどれだけ離れているか
        float distance = Vector2.Distance(transform.position, playerTr.position);
        //自分とプレイヤー高さの差
        heightDiff = playerTr.position.y - transform.position.y;


        //一定距離離れたら追いかける
        if (distance > followDistance)
        {
            //プレイヤーが走っているか歩いているか
            float move = Mathf.Abs(playerController.Move);
            //プレイヤーが動いていなくても、最低速度は保証する
            float minimumSpeed = Mathf.Max(move, 0.5f);
            //プレイヤーの位置をもとに移動方向を決める
            float dir = Mathf.Sign(playerTr.position.x - transform.position.x);

            targetX = dir * walkForce * minimumSpeed;

            //オブジェクト反転
            if (dir == 0) dir = 1;
            //parentTr.localScale = new Vector3(dir, 1, 1);
            transform.localScale = new Vector3(dir, 1, 1);
            //DebugLogger.Log(distance);
        }
        else if (distance < stopDistance)
        {
            targetX = 0f;
            //DebugLogger.Log(distance);
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        // 今のY速度はそのまま維持！
        float newX = Mathf.Lerp(parentRd.linearVelocity.x, targetX, smooth * Time.fixedDeltaTime);
        parentRd.linearVelocity = new Vector2(newX, parentRd.linearVelocity.y);



        //一定距離離れたらプレイヤーのところにワープする
        if (Vector2.Distance(transform.position, playerTr.position) > warpDistance)
        {
            float dir = Mathf.Sign(playerTr.position.x - transform.position.x);
            Vector2 warpPos = playerTr.position + new Vector3(-dir * 1.5f, 0, 0);

            parentRd.position = warpPos;
            parentRd.linearVelocity = Vector2.zero;
        }

        // 壁に当たってたらジャンプ
        if (isGrounded && heightDiff > 1f && Time.time >= nextJumpTime)
        {
            parentRd.linearVelocity = new Vector2(parentRd.linearVelocity.x, jumpForce);
            nextJumpTime = Time.time + jampDelay;
        }
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)//接触点の取得
        {
            // 下から接触 → 地面
            if (contact.normal.y > GroundCatch)
            {
                isGrounded = true;
            }
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //地面から離れたかどうか
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

