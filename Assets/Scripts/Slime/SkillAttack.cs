using UnityEngine;

public class SkillAttack : MonoBehaviour
{
    private Rigidbody2D parentRd;//親オブジェクトのものを取得
    private Transform parentTr;
    [SerializeField] private GameObject player;//プレイヤー
    [SerializeField] private float smooth = 5f;           // ぬるっと具合
    bool enable = false;//有効になったか
    bool hasWarped = false;//プレイヤーの前にワープしたか
    float dashDir = 0;//ダッシュ方向
    [SerializeField] GameObject playerTracking;
    [SerializeField] private float chargeTime = 0.3f;   // 溜め時間
    [SerializeField] private float recoveryTime = 0.3f; // 硬直時間
    private Vector2 dashStartPos; // 突進開始位置
    private bool isDashing = false;//突進してもいいのか
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
