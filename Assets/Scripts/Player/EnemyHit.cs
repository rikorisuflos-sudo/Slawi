using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer; // 地面レイヤー 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 接触したオブジェクトのレイヤーが、設定したレイヤーに含まれているか確認
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("特定のレイヤーのオブジェクトに接触しました: " + collision.gameObject.name);
        }
    }
}
