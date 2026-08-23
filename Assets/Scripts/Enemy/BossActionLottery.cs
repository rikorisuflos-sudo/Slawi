using System.Collections.Generic;
using UnityEngine;

public class BossActionLottery : MonoBehaviour
{
    [SerializeField] List<MonoBehaviour> scriptList = new List<MonoBehaviour>();

    [SerializeField] Enemy enemyStatus;//エネミーのスクリプタブルオブジェクト
    float attackDistance;//攻撃範囲
    float attackAngle;//攻撃範囲

    float enemyDir;//エネミーの向き

    [SerializeField] Transform playerTr;//プレイヤートランスフォーム

    bool isAttacking = false;
    public bool IsAttacking
    {
        set { isAttacking = value; }
    }
    void Start()
    {
        //初期化
        if (enemyStatus != null)
        {
            this.attackDistance = enemyStatus.AttackDistance;
            this.attackAngle = enemyStatus.AttackAngle;
        }
    }
    private void OnEnable()
    {
        //発射方向を取得
        enemyDir = Mathf.Sign(transform.parent.localScale.x);
        isAttacking = false;

        ActivateRandomScript();
    }

    private void FixedUpdate()
    {
        bool callAttack = CallAttack();

        if (!callAttack)
        {
            gameObject.SetActive(false);
            return;
        }
        if (!isAttacking)
        {
            ActivateRandomScript();
        }
    }

    void ActivateRandomScript()
    {
        // リストが空の場合は処理しない
        if (scriptList == null || scriptList.Count == 0) return;

        // 2. ランダムなインデックスを1つ決定
        int randomIndex = Random.Range(0, scriptList.Count);

        // 3. 選ばれたスクリプトを取り出して有効化
        MonoBehaviour chosenScript = scriptList[randomIndex];
        if (chosenScript != null)
        {
            isAttacking = true;
            chosenScript.enabled = true;
            Debug.Log($"{chosenScript.GetType().Name} を有効化しました。");
        }
    }

    bool CallAttack()
    {
        //プレイヤーの方向を取得
        Vector2 direction = (playerTr.position - transform.parent.position);
        //プレイヤーまでの距離を算出
        float distance = direction.magnitude;
        //  距離チェック
        if (distance > attackDistance)
        {
            return false;
        }

        //エネミーの方向取得
        Vector2 facingDir = new Vector2(enemyDir, 0);
        //  プレイヤーの位置とエネミーの位置の角度差
        float angle = Vector2.Angle(facingDir, direction);
        //angleが視野角内に収まるかどうか
        if (angle > attackAngle / 2f)
        {
            return false;
        }

        return true;
    }
}
