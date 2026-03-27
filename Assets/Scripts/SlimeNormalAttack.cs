using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class SlimeNormalAttack : MonoBehaviour
{
    private Rigidbody2D parentRd;//親オブジェクトのものを取得
    [SerializeField] private GameObject player;//プレイヤー
    [SerializeField] private float smooth = 5f;           // ぬるっと具合

    [SerializeField] private float dashDistance = 20f;//攻撃距離
    [SerializeField] private float dashSpeed = 20f;//攻撃速度

    bool enable = false;//有効になったか
    bool hasWarped = false;//プレイヤーの前にワープしたか
    private Vector2 dashStartPos; // 突進開始位置
    private bool isDashing = false;//突進してもいいのか

    void OnEnable()
    {
        enable = true;
    }

    private void OnDisable()
    {
        enable = false;
        hasWarped = false;
    }

    private void Awake()
    {
        parentRd = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (enable)
        {
            Attack();
        }
    }

    public void Attack()
    {
            float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
        //ワープは1度まで
        if (!hasWarped)
        {
            // プレイヤーの前にワープ
            Vector2 warpPos = player.transform.position + new Vector3(dir * 1.5f, 0, 0);

            parentRd.position = warpPos;
            parentRd.linearVelocity = Vector2.zero;

            hasWarped = true;
            isDashing = true;
            dashStartPos = parentRd.position;
            return; // このフレームはここで終了
        }

        //突進
        if (isDashing)
        {
            float targetX = dir * dashSpeed;

            float newX = Mathf.Lerp(parentRd.linearVelocity.x, targetX, smooth * Time.fixedDeltaTime);
            parentRd.linearVelocity = new Vector2(newX, parentRd.linearVelocity.y);

            // 移動距離チェック
            float distance = Vector2.Distance(parentRd.position, dashStartPos);

            if (distance >= dashDistance)
            {
                isDashing = false;
                parentRd.linearVelocity = Vector2.zero;
                this.gameObject.SetActive(false);
            }
        }
    }
}
