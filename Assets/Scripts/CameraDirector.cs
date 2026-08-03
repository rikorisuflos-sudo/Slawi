using UnityEngine;


public class CameraDirector : MonoBehaviour
{
    [SerializeField] Transform player;//プレイヤーのトランスフォーム

    [Header("追従スピード")]
    [SerializeField, Range(0, 1)] float smoothSpeed;

    //デッドゾーンの基準
    [SerializeField, Range(0, 1)] float referenceLineTop;//上
    [SerializeField, Range(0, 1)] float referenceLineBotom;//下
    [SerializeField, Range(0, 1)] float referenceLineLeft;//左
    [SerializeField, Range(0, 1)] float referenceLineRight;//右

    Vector3 playerViewport;//プレイヤーのビューポート座標

    Vector3 cameraPs = new Vector3(1, 1, -20);//カメラの位置

    void LateUpdate()
    {
        //プレイヤーがいなかったら
        if (player == null) return;

        //プレイヤーのワールド座標をビューポート座標に変換
        playerViewport = Camera.main.WorldToViewportPoint(player.position);

        //基準より左or右にいたらx軸方向に追従する
        if (playerViewport.x < referenceLineLeft || playerViewport.x > referenceLineRight)
        {
            cameraPs.x = Mathf.SmoothDamp(transform.position.x, player.position.x, ref smoothSpeed, 1.2f);
        }

        //基準より上or下にいたらy軸方向に追従する
        if (playerViewport.y < referenceLineBotom || playerViewport.y > referenceLineTop)
        {
            cameraPs.y = Mathf.SmoothDamp(transform.position.y, player.position.y, ref smoothSpeed, 1.2f);
        }

        //移動処理
        transform.position = cameraPs;
    }

    //ギズモ
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(Camera.main.ViewportToWorldPoint(new Vector3(referenceLineLeft, 0, 0)), Camera.main.ViewportToWorldPoint(new Vector3(referenceLineLeft, 1, 0)));
        Gizmos.DrawLine(Camera.main.ViewportToWorldPoint(new Vector3(referenceLineRight, 0, 0)), Camera.main.ViewportToWorldPoint(new Vector3(referenceLineRight, 1, 0)));
        Gizmos.DrawLine(Camera.main.ViewportToWorldPoint(new Vector3(0, referenceLineTop, 0)), Camera.main.ViewportToWorldPoint(new Vector3(1, referenceLineTop, 0)));
        Gizmos.DrawLine(Camera.main.ViewportToWorldPoint(new Vector3(0, referenceLineBotom, 0)), Camera.main.ViewportToWorldPoint(new Vector3(1, referenceLineBotom, 0)));
    }
}