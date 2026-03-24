using UnityEngine;

public class CameraDirector : MonoBehaviour
{
    [Header("追いかける対象")]
    public Transform player;

    [Header("追従スピード (X軸)")]
    [Range(0, 1)]
    public float smoothSpeed = 0.125f;

    [Header("上下操作の設定 (W/S)")]
    public float verticalSpeed = 10f; // 上下移動の速さ
    private float currentY;           // 現在のカメラの高さ

    void Start()
    {
        // 起動時のカメラのY座標を初期位置として保存
        currentY = transform.position.y;
    }

    void LateUpdate()
    {
        // 1. プレイヤーのX軸追従
        float targetX = transform.position.x;
        if (player != null)
        {
            targetX = Mathf.Lerp(transform.position.x, player.position.x, smoothSpeed);
        }

        // 2. W/Sキーによる上下操作 (Vertical = W/S または 上下矢印キー)
        float vInput = Input.GetAxis("Vertical");
        currentY += vInput * verticalSpeed * Time.deltaTime;

        // 3. 最終的な位置を適用 (Zは現在の値を維持)
        transform.position = new Vector3(targetX, currentY, transform.position.z);
    }
}