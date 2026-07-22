using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTracking : MonoBehaviour
{
    Rigidbody2D parentRd;//親オブジェクトのものを取得
    //Transform parentTr;
    [SerializeField] Transform playerTr;//追いかける対象
    [SerializeField] PlayerMove playerMove;

    [SerializeField] float walkForce;//速度の上昇値
    [SerializeField] float jumpForce;//ジャンプの高さ

    [SerializeField] float followDistance; // この距離より離れたら追う
    [SerializeField] float stopDistance;   // この距離まで来たら止まる
    [SerializeField] float warpDistance; // この距離より離れたら追う
    [SerializeField] float smooth = 5f;           // ぬるっと具合

    [SerializeField] float wallCheckDistance;//壁検知範囲
    [SerializeField] LayerMask groundLayer;

    const float GroundCatch = 0.5f;
    private bool isGrounded = true;//地面に触れているかいないか

    float enemyDir;

    float targetX = 0f;//目標座標
    float heightDiff;

    BoundedQueue<Vector3> vector3s = new BoundedQueue<Vector3>(20);
    Vector3 plaerPos = Vector3.zero;
    [SerializeField] float recordInterval = 0.2f;

    float timer;


    void Start()
    {
        parentRd = GetComponentInParent<Rigidbody2D>();

        //プレイヤーの位置を格納
        vector3s.Enqueue(playerTr.position);
    }

    void Update()
    {

        //自分とプレイヤーがどれだけ離れているか
        float distance = Vector2.Distance(transform.position, playerTr.position);


        //一定距離離れたら追いかける
        if (distance > followDistance)
        {
            //プレイヤーが走っているか歩いているか
            float move = Mathf.Abs(playerMove.Move);
            //プレイヤーが動いていなくても、最低速度は保証する
            float minimumSpeed = Mathf.Max(move, 0.5f);
            //プレイヤーの位置をもとに移動方向を決める
            enemyDir = Mathf.Sign(plaerPos.x - transform.position.x);

            targetX = enemyDir * walkForce * minimumSpeed;

            //オブジェクト反転
            if (enemyDir == 0) enemyDir = 1;
            //parentTr.localScale = new Vector3(dir, 1, 1);
            transform.localScale = new Vector3(enemyDir, 1, 1);
            //DebugLogger.Log(distance);
        }
        else if (distance < stopDistance)
        {
            targetX = 0f;
            //DebugLogger.Log(distance);
        }
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        vector3s.Enqueue(playerTr.position);

        if (vector3s.Count >= 20)
        {
            plaerPos = vector3s.Peek();
        }


        float speed = Mathf.Clamp(targetX, -walkForce, walkForce);

        parentRd.linearVelocity = new Vector2(
            speed,
            parentRd.linearVelocity.y);



        //一定距離離れたらプレイヤーのところにワープする
        if (Vector2.Distance(transform.position, playerTr.position) > warpDistance)
        {
            float dir = Mathf.Sign(playerTr.position.x - transform.position.x);
            Vector2 warpPos = playerTr.position + new Vector3(-enemyDir * 1.5f, 0, 0);

            parentRd.position = warpPos;
            parentRd.linearVelocity = Vector2.zero;
        }

        // 壁に当たってたらジャンプ
        if (isGrounded && CheckWall())
        {
            parentRd.linearVelocity = new Vector2(parentRd.linearVelocity.x, jumpForce);
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        //地面から離れたかどうか
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
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

    }
    void OnCollisionExit2D(Collision2D collision)
    {
        //地面から離れたかどうか
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }

    bool CheckWall()
    {
        RaycastHit2D wallFind = Physics2D.Raycast(
            parentRd.position,
            Vector2.right * enemyDir,
            wallCheckDistance,
            groundLayer);

        Debug.DrawRay(parentRd.position,
                      Vector2.right * enemyDir * wallCheckDistance,
                      Color.red);

        return wallFind.collider != null;
    }

}