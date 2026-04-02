using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class SlimeNormalAttack : MonoBehaviour
{
    private Rigidbody2D parentRd;//親オブジェクトのものを取得
    private Transform parentTr;
    [SerializeField] private GameObject player;//プレイヤー
    [SerializeField] private float smooth = 5f;           // ぬるっと具合

    [SerializeField] private float dashDistance = 20f;//攻撃距離
    [SerializeField] private float dashSpeed = 20f;//攻撃速度
    float dashDir = 0;//ダッシュ方向

    [SerializeField] private float chargeTime = 0.3f;   // 溜め時間
    [SerializeField] private float recoveryTime = 0.3f; // 硬直時間

    bool enable = false;//有効になったか
    bool hasWarped = false;//プレイヤーの前にワープしたか
    private Vector2 dashStartPos; // 突進開始位置
    private bool isDashing = false;//突進してもいいのか

    [SerializeField] private float wallCheckDistance = 0.5f;//壁の探索範囲
    [SerializeField] private LayerMask groundLayer; // 地面レイヤー

    [SerializeField] GameObject playerTracking;
    void OnEnable()
    {
        playerTracking.SetActive(false);
        enable = true;
    }

    private void Awake()
    {
        parentRd = GetComponentInParent<Rigidbody2D>();
        parentTr = transform.parent;
    }

    private void FixedUpdate()
    {
        if (enable)
        {
            Attack();
        }
    }

    public void StartAttack()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        enable = false;
        isDashing = false;

        Warp();

        //溜め
        parentRd.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(chargeTime);


        // 突進（終了待ち）
        while (isDashing)
        {
            Attack();
            yield return null;
        }

        // 硬直
        parentRd.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(recoveryTime);

        // 終了
        gameObject.SetActive(false);
        playerTracking.SetActive(true);
    }
    private void Attack()
    {
        //突進
        if (isDashing)
        {
            float targetX = dashDir * dashSpeed;

            float newX = Mathf.Lerp(parentRd.linearVelocity.x, targetX, smooth * Time.fixedDeltaTime);
            parentRd.linearVelocity = new Vector2(newX, parentRd.linearVelocity.y);

            // 壁チェック（進行方向にRay）
            Vector2 origin = parentRd.position;
            Vector2 direction = new Vector2(dashDir, 0);

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, wallCheckDistance, groundLayer);
            Debug.DrawRay(origin, direction, Color.red);


            if (hit.collider != null)
            {
                DebugLogger.Log("壁に当たった");
                EndAttack();
            }
            // 移動距離チェック
            float distance = Vector2.Distance(parentRd.position, dashStartPos);

            if (distance >= dashDistance)
            {
                DebugLogger.Log("移動距離が一定に達した");
                EndAttack();
            }
        }
    }

    private void Warp()
    {
        //ワープ（1度まで）
        if (!hasWarped)
        {
            // プレイヤーの向き（右=1、左=-1）
            dashDir = Mathf.Sign(player.transform.localScale.x);
            if (dashDir == 0) dashDir = 1;

            // プレイヤーの前にワープ
            Vector2 warpPos = player.transform.position + new Vector3(dashDir * 1.5f, 0, 0);
            //ワープしたときに突進方向を向くようにする
            parentTr.localScale = new Vector3(dashDir, 1, 1);


            parentRd.position = warpPos;
            parentRd.linearVelocity = Vector2.zero;

            hasWarped = true;
            isDashing = true;
            dashStartPos = parentRd.position;
            return; // このフレームはここで終了
        }
    }

    void EndAttack()
    {
        hasWarped = false;
        isDashing = false;
        enable = false;
        parentRd.linearVelocity = Vector2.zero;

    }
}
