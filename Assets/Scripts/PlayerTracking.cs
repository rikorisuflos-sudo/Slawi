using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerTracking : MonoBehaviour
{
    private Rigidbody2D parentRd;//親オブジェクトのものを取得
    [SerializeField] private GameObject player;//追いかける対象
    private PlayerController playerController;

    [SerializeField] private float walkForce;//速度の上昇値
    [SerializeField] private float jumpForce;//ジャンプの高さ
    [SerializeField] private float followDistance = 2.5f; // この距離より離れたら追う
    [SerializeField] private float stopDistance = 1.5f;   // この距離まで来たら止まる
    [SerializeField] private float warpDistance; // この距離より離れたら追う
    [SerializeField] private float smooth = 5f;           // ぬるっと具合

    private bool isGrounded = true;//地面に触れているかいないか

    float targetX = 0f;//目標座標
    float heightDiff;
    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        parentRd = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        //自分とプレイヤーがどれだけ離れているか
        float distance = Vector2.Distance(transform.position, player.transform.position);
        //自分とプレイヤー高さの差
        heightDiff = player.transform.position.y - transform.position.y;

        //一定距離離れたら追いかける
        if (distance > followDistance)
        {
            float move = Mathf.Abs(playerController.Move);
            float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
            targetX = dir * walkForce * move;
            
        }
        else if (distance < stopDistance)
        {
            targetX = 0f;
        }
    }

    private void FixedUpdate()
    {
      
        // 今のY速度はそのまま維持！
        float newX = Mathf.Lerp(parentRd.linearVelocity.x, targetX, smooth * Time.fixedDeltaTime);
        parentRd.linearVelocity = new Vector2(newX, parentRd.linearVelocity.y);


        //一定距離離れたらプレイヤーのところにワープする
        if (Vector2.Distance(transform.position, player.transform.position) > warpDistance)
        {
            float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
            Vector2 warpPos = player.transform.position + new Vector3(-dir * 1.5f, 0, 0);

            parentRd.position = warpPos;
            parentRd.linearVelocity = Vector2.zero;
        }

        // 壁に当たってたらジャンプ
        if (isGrounded && heightDiff > 0.5f)
        {
            parentRd.linearVelocity = new Vector2(parentRd.linearVelocity.x, jumpForce);
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

