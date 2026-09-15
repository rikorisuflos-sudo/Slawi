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
        set { isAttacking = value; }
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
        //���˕������擾
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
        // ���X�g����̏ꍇ�͏������Ȃ�
        if (scriptList == null || scriptList.Count == 0) return;

        // 2. �����_���ȃC���f�b�N�X��1����
        int randomIndex = Random.Range(0, scriptList.Count);

        // 3. �I�΂ꂽ�X�N���v�g�����o���ėL����
        MonoBehaviour chosenScript = scriptList[randomIndex];
        if (chosenScript != null)
        {
            isAttacking = true;
            chosenScript.enabled = true;
            Debug.Log($"{chosenScript.GetType().Name} ��L�������܂����B");
        }
    }

    bool CallAttack()
    {
        //�v���C���[�̕������擾
        Vector2 direction = (playerTr.position - transform.parent.position);
        //�v���C���[�܂ł̋������Z�o
        float distance = direction.magnitude;
        //  �����`�F�b�N
        if (distance > attackDistance)
        {
            return false;
        }

        //�G�l�~�[�̕����擾
        Vector2 facingDir = new Vector2(enemyDir, 0);
        //  �v���C���[�̈ʒu�ƃG�l�~�[�̈ʒu�̊p�x��
        float angle = Vector2.Angle(facingDir, direction);
        //angle������p���Ɏ��܂邩�ǂ���
        if (angle > attackAngle / 2f)
        {
            return false;
        }

        return true;
    }
}