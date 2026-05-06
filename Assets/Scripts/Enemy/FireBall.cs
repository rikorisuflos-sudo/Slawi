using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FireBall : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float movwForce; // オブジェクトの移動速度
    [SerializeField] float smooth;//ヌルっと感を出す
    [SerializeField] float rushDistance = 20f;//攻撃距離
    float dir = 1;
    [SerializeField] LayerMask groundLayer; // 地面レイヤー

    private void Start()
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2Dが付いてない！");
        }
        // 重力の影響を無効化
        rb.gravityScale = 0f;
    }
    private void FixedUpdate()
    {
        float targetX = movwForce * dir;
        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetX, smooth * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
    }


}
