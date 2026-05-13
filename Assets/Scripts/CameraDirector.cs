using UnityEngine;

public class CameraDirector : MonoBehaviour
{
    [SerializeField]GameObject player;

    [Header("追従スピード")]
    [SerializeField, Range(0, 1)] private float smoothSpeed;

    [Header("上下操作の設定 (W/S)")]
    [SerializeField] private float verticalSpeed = 10f;
    [SerializeField] private float maxManualYOffset;

    private float manualYOffset = 0f; // 手動Y移動のオフセット


    void LateUpdate()
    {
        if (player == null) return;

        //プレイヤー追従
        float targetX = Mathf.Lerp(
            transform.position.x,
            player.transform.position.x,
            smoothSpeed
        );

        float targetY = Mathf.Lerp(
            transform.position.y,
            player.transform.position.y,
            smoothSpeed
        );

        // W/S入力
        //float vInput = Input.GetAxis("Vertical");

        //if (vInput != 0)
        //{
        //    manualYOffset += vInput * verticalSpeed * Time.deltaTime;
        //    manualYOffset = Mathf.Clamp(manualYOffset, -maxManualYOffset, maxManualYOffset);
        //}
        //else
        //{
        //    manualYOffset = 0;
        //}

        //  最終位置
        transform.position = new Vector3(
            targetX,
            targetY + manualYOffset,
            -10f
        );
    }
}