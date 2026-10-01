using System.Collections.Generic;
using UnityEngine;

public class BossActionLottery : MonoBehaviour
{
    [SerializeField] List<MonoBehaviour> scriptList = new List<MonoBehaviour>();

    [SerializeField] Enemy enemyStatus;//�G�l�~�[�̃X�N���v�^�u���I�u�W�F�N�g
    float attackDistance;//�U���͈�
    float attackAngle;//�U���͈�

    float enemyDir;//�G�l�~�[�̌���

    [SerializeField] Transform playerTr;//�v���C���[�g�����X�t�H�[��

    bool isAttacking = false;
    public bool IsAttacking
    {
        get => isAttacking;
        set
        {
            Debug.Log($"IsAttacking: {isAttacking} → {value}");
            isAttacking = value;
        }
    }
    void Start()
    {
        //������
        if (enemyStatus != null)
        {
            this.attackDistance = enemyStatus.AttackDistance;
            this.attackAngle = enemyStatus.AttackAngle;
        }
    }
    private void OnEnable()
    {
        //エネミーが今向いている方向
        enemyDir = Mathf.Sign(transform.parent.localScale.x);
      
        if (!HasActiveAttack())
        {
            IsAttacking = false;
            ActivateRandomScript();
        }
    }

    private void FixedUpdate()
    {
        bool callAttack = CallAttack();

        if (!callAttack)
        {
            Debug.Log("攻撃範囲外 → BossActionLottery停止");
            gameObject.SetActive(false);
            return;
        }

        if (!isAttacking)
        {
            Debug.Log("攻撃可能 → ランダム攻撃選択");
            ActivateRandomScript();
        }
    }

    bool HasActiveAttack()
    {
        foreach (MonoBehaviour script in scriptList)
        {
            if (script != null && script.enabled)
            {
                return true;
            }
        }

        return false;
    }

    void ActivateRandomScript()
    {
        if (scriptList == null || scriptList.Count == 0)
            return;

        // まず全攻撃を無効化
        //foreach (MonoBehaviour script in scriptList)
        //{
        //    if (script != null)
        //    {
        //        script.enabled = false;
        //    }
        //}

        // ランダムに1つ選択
        int randomIndex = Random.Range(0, scriptList.Count);

        MonoBehaviour chosenScript = scriptList[randomIndex];

        if (chosenScript != null)
        {
            isAttacking = true;
            chosenScript.enabled = true;

            Debug.Log($"{chosenScript.GetType().Name} を有効にしました。");
        }
    }

    bool CallAttack()
    {
        Vector2 direction = playerTr.position - transform.parent.position;
        float distance = direction.magnitude;

        if (distance > attackDistance)
        {
            return false;
        }

        Vector2 facingDir = new Vector2(enemyDir, 0);
        float angle = Vector2.Angle(facingDir, direction);

        if (angle > attackAngle / 2f)
        {
            return false;
        }

        return true;
    }
}