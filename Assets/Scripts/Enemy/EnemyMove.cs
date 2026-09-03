using System;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] Rigidbody2D parentRd;
    [SerializeField] Transform parentTr;
    [SerializeField] Enemy enemyStatus;
    [SerializeField] GameObject attack;
    [SerializeField] Animator animator;
    [SerializeField] Canvas hpBar;

    [Header("移動範囲")]
    [SerializeField] BoxCollider2D moveArea; // Cinemachine ConfinerのBoxCollider2D

    [SerializeField] float enemyMileage;
    float enemyDir;
    float walkForce;

    [SerializeField] float smooth;

    [SerializeField] float checkDistance;
    [SerializeField] float wallCheckDistance;
    [SerializeField] LayerMask groundLayer;

    bool turnAround = false;

    Vector2 startPosition;
    Vector2 movPosition;

    [SerializeField] Transform playerTr;

    float viewDistance = 5f;
    float viewAngle = 60f;
    bool playerFind = false;

    float attackDistance;
    float attackAngle;

    bool stop = false;

    void OnEnable()
    {
        enemyDir = Mathf.Sign(parentTr.localScale.x);
    }

    void Start()
    {
        enemyDir = Mathf.Sign(parentTr.localScale.x);

        if (transform != null)
        {
            startPosition = parentTr.position;
            movPosition = startPosition;
        }

        if (enemyStatus != null)
        {
            walkForce = enemyStatus.WalkForce;
            viewDistance = enemyStatus.ViewDistance;
            viewAngle = enemyStatus.ViewAngle;
            attackDistance = enemyStatus.AttackDistance;
            attackAngle = enemyStatus.AttackAngle;
        }
    }

    void FixedUpdate()
    {
        bool callAttack = CallAttack();

        float broadcastTime =
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (callAttack && broadcastTime >= 1.0f)
        {
            attack.SetActive(true);
            gameObject.SetActive(false);
            return;
        }

        Walk();
    }

    void Walk()
    {
        // アニメーションなどから停止を指定されている場合
        if (stop)
        {
            parentRd.linearVelocity =
                new Vector2(0.0f, parentRd.linearVelocity.y);
            return;
        }

        playerFind = PlayerSearch();

        if (!playerFind)
        {
            turnAround = CheckTurn();

            float currentDistance =
                Vector3.Distance(movPosition, parentTr.position);

            if (currentDistance >= enemyMileage || turnAround)
            {
                enemyDir *= -1;

                hpBar.transform.localScale =
                    new Vector3(enemyDir * 0.01f, 0.01f, 1);

                movPosition = transform.position;

                parentTr.localScale =
                    new Vector3(
                        transform.parent.lossyScale.x * enemyDir,
                        transform.parent.lossyScale.y,
                        1
                    );
            }
        }

        float targetX = enemyDir * walkForce;

        // 移動
        parentRd.linearVelocity =
            new Vector2(targetX, parentRd.linearVelocity.y);
    }

    bool CheckTurn()
    {
        // =========================
        // 移動範囲チェック
        // =========================
        if (moveArea != null)
        {
            Bounds bounds = moveArea.bounds;

            // エネミーの現在位置
            float currentX = parentRd.position.x;

            // 移動範囲の端にいるか
            if (currentX <= bounds.min.x && enemyDir < 0)
            {
                return true;
            }

            if (currentX >= bounds.max.x && enemyDir > 0)
            {
                return true;
            }
        }


        // =========================
        // 壁チェック
        // =========================
        RaycastHit2D wallFind = Physics2D.Raycast(
            parentRd.position,
            new Vector2(enemyDir, 0),
            wallCheckDistance,
            groundLayer
        );

        Debug.DrawRay(
            parentRd.position,
            new Vector2(enemyDir, 0),
            Color.red
        );

        if (wallFind.collider != null)
        {
            return true;
        }


        // =========================
        // 地面検知
        // =========================
        Vector2 offset = new Vector2(enemyDir * 0.5f, 0);

        RaycastHit2D holeFind = Physics2D.Raycast(
            parentRd.position + offset,
            Vector2.down,
            checkDistance,
            groundLayer
        );

        if (holeFind.collider == null)
        {
            return true;
        }

        return false;
    }

    bool PlayerSearch()
    {
        if (playerTr == null)
            return false;

        Vector2 direction =
            playerTr.position - parentTr.position;

        float distance = direction.magnitude;

        if (distance > viewDistance)
            return false;

        Vector2 facingDir =
            new Vector2(enemyDir, 0);

        float angle =
            Vector2.Angle(facingDir, direction);

        if (angle > viewAngle / 2f)
            return false;

        RaycastHit2D hit =
            Physics2D.Raycast(
                parentTr.position,
                direction.normalized,
                distance,
                groundLayer
            );

        if (hit.collider != null)
            return false;

        return true;
    }

    bool CallAttack()
    {
        if (playerTr == null)
            return false;

        Vector2 direction =
            playerTr.position - parentTr.position;

        float distance = direction.magnitude;

        if (distance > attackDistance)
            return false;

        Vector2 facingDir =
            new Vector2(enemyDir, 0);

        float angle =
            Vector2.Angle(facingDir, direction);

        if (angle > attackAngle / 2f)
            return false;

        return true;
    }

    public void CastBool(int input)
    {
        stop = Convert.ToBoolean(input);
    }
}