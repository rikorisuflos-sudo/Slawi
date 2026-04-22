using UnityEngine;


// 敵エネミーが待機状態で一定範囲を巡回するスクリプト

public class EnemyIdleMove : MonoBehaviour
{
    // --- 変数定義  ---
    [SerializeField] private Rigidbody2D rigid2D; // エネミーのRigidbody2Dを取得 
    //[SerializeField] private new Transform transform; // エネミーのTransformを取得
    //[SerializeField] private Transform enemyTr; 
    [SerializeField] private float enemyMileage; // エネミーの移動距離 
    private float enemyDir = 1f; // エネミーの向き (初期値1) 
    [SerializeField] private float walkForce; // エネミーの移動速度 
    private bool playerFind = false; // プレイヤーを見つけたか (初期値false) 
    private bool stepFind = false; // 段差があるか (初期値false) 
    [SerializeField] private float checkDistance; // 地面検知範囲 
    [SerializeField] private LayerMask groundLayer; // 地面レイヤー 

    // 移動範囲制御のための内部変数 
    private Vector3 startPosition;

    // --- メソッド定義  ---

    // playerFindやenemyDirなどの初期化を行なう 
    private void Awake()
    {
        playerFind = false;
        stepFind = false;
        enemyDir = 1f;

        if (transform != null)
        {
            startPosition = transform.position;
        }
    }

    // 移動処理を呼び出す 
    private void FixedUpdate()
    {
        // プレイヤーを発見したら、待機状態を解除する（＝移動を停止する） 
        if (!playerFind)
        {
            Walk();
        }
        else
        {
            // 発見時はその場に留まる
            rigid2D.linearVelocity = new Vector2(0, rigid2D.linearVelocity.y);
        }
    }

    // 移動処理やそれに関連する処理を記述 
    private void Walk()
    {
        // 段差検知の更新 
        SearchStep();

        // 現在の移動距離を計算
        float currentDistance = Vector3.Distance(startPosition, transform.position);

        // 往復および落下防止の判定
        // 一定範囲（enemyMileage）を超えたか、段差（stepFind）があった場合に反転
        if (currentDistance >= enemyMileage || stepFind)
        {
            enemyDir *= -1;
            startPosition = transform.position; // 反転地点を起点に更新

            // 見た目の向きを反転させる処理
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * enemyDir;
            transform.localScale = scale;
        }

        // linearVelocityによる移動処理 
        rigid2D.linearVelocity = new Vector2(enemyDir * walkForce, rigid2D.linearVelocity.y);
    }

    // 段差検知のための処理を記述 (Raycastを用いる) 
    private void SearchStep()
    {
        // エネミーの少し前方から下向きにRayを飛ばす
        Vector2 rayOrigin = transform.position;
        Vector2 offset = new Vector2(enemyDir * 0.5f, 0); // 前方オフセット

        // 地面検知の実行
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin + offset, Vector2.down, checkDistance, groundLayer);

        // Rayが地面に当たっていない（＝穴がある）場合は段差ありと判定 
        if (hit.collider == null)
        {
            stepFind = true;
        }
        else
        {
            stepFind = false;
        }
    }
}