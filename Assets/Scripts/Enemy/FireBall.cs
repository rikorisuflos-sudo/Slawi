using UnityEngine;

public class FireBall : MonoBehaviour
{
    //親オブジェクトのものを取得
    Rigidbody2D parentRd;

    [SerializeField] Transform playerTr;//プレイヤー
    //[SerializeField] Animator animator;

    [SerializeField] float rushDistance = 20f;//攻撃距離
    [SerializeField] LayerMask groundLayer; // 地面レイヤー
}
