using UnityEngine;

public class BossFireBallShot : MonoBehaviour
{
    private enum AttackType
    {
        FanShot,
        RandomShot
    }

    [Header("敵ステータス")]
    [SerializeField] private Enemy enemyStatus;

    [Header("アニメーション")]
    [SerializeField] private Animator animator;

    [Header("移動")]
    [SerializeField] private GameObject move;

    [Header("ファイアボール")]
    [SerializeField] private GameObject fireBall;

    [Header("プレイヤー")]
    [SerializeField] private Transform playerTr;

    [Header("レイヤー")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask groundLayer;

    [Header("攻撃設定")]
    private float attackDistance;
    private float attackAngle;

    private AttackType attackType;

    // 敵の向き
    private float enemyDir;

    // FanShot用
    private float angle = 50f;

    // RandomShot用
    private int count = 0;
    private float timer = 0f;
    private bool isShooting = false;

    // 1回の攻撃でShot()を1回だけ実行する
    private bool hasShot = false;

    // 現在攻撃中か
    private bool isAttacking = false;


    private void Start()
    {
        if (enemyStatus != null)
        {
            attackDistance = enemyStatus.AttackDistance;
            attackAngle = enemyStatus.AttackAngle;
        }
    }


    private void OnEnable()
    {
        Debug.Log(
            $"========== BossFireBallShot OnEnable ==========\n" +
            $"frame={Time.frameCount}\n" +
            $"animator.speed={animator.speed}"
        );

        StartAttack();
    }


    private void OnDisable()
    {
        Debug.Log(
            $"========== BossFireBallShot OnDisable ==========\n" +
            $"frame={Time.frameCount}"
        );

        count = 0;
        timer = 0f;
        isShooting = false;
        hasShot = false;
        isAttacking = false;

        if (animator != null)
        {
            animator.speed = 1f;
        }
    }


    /*
     * Animatorは通常Updateで更新されるため、
     * アニメーション終了監視もUpdateで行う。
     */
    private void Update()
    {
        if (!isAttacking)
        {
            return;
        }

        /*
         * RandomShot中
         *
         * Animatorを停止しているため、
         * AnimationEvent後の5発を時間経過で発射する。
         */
        if (isShooting)
        {
            timer += Time.deltaTime;

            if (timer >= 0.5f)
            {
                timer = 0f;

                ShootRandomFireBall();
            }

            return;
        }


        /*
         * FanShotの場合はこちら。
         *
         * RandomShotが終了した後もここに入る。
         */
        AnimatorStateInfo stateInfo =
            animator.GetCurrentAnimatorStateInfo(0);

        /*
         * 現在のAnimator状態を確認
         */
        if (stateInfo.normalizedTime >= 1f)
        {
            Debug.Log(
                $"【アニメーション終了判定】 " +
                $"frame={Time.frameCount} " +
                $"normalizedTime={stateInfo.normalizedTime} " +
                $"speed={animator.speed}"
            );

            FinishAttack();
        }
    }


    /*
     * 攻撃開始
     */
    public void StartAttack()
    {
        Debug.Log(
            $"========== StartAttack ==========\n" +
            $"frame={Time.frameCount}"
        );

        if (animator == null)
        {
            Debug.LogError("【StartAttack】Animatorが設定されていません");
            return;
        }

        if (enemyStatus == null)
        {
            Debug.LogError("【StartAttack】enemyStatusが設定されていません");
            return;
        }

        /*
         * 敵の向き
         */
        if (transform.parent != null)
        {
            enemyDir = Mathf.Sign(transform.parent.localScale.x);
        }
        else
        {
            enemyDir = Mathf.Sign(transform.localScale.x);
        }

        /*
         * 攻撃状態を初期化
         */
        count = 0;
        timer = 0f;
        angle = 50f;

        isShooting = false;
        hasShot = false;
        isAttacking = true;

        /*
         * 攻撃開始時は必ずAnimatorを動かす
         */
        animator.speed = 1f;

        Debug.Log(
            $"【StartAttack直後】 " +
            $"frame={Time.frameCount} " +
            $"animator.speed={animator.speed}"
        );

        /*
         * 移動停止
         */
        if (move != null)
        {
            move.SetActive(false);
        }

        /*
         * Shotアニメーション開始
         */
        animator.SetBool("Shot", true);

        Debug.Log(
            $"【StartAttack終了】 " +
            $"frame={Time.frameCount} " +
            $"animator.speed={animator.speed}"
        );
    }


    /*
     * AnimationEventから呼ばれる
     */
    public void Shot()
    {
        Debug.Log(
            $"========== Shot ==========\n" +
            $"frame={Time.frameCount}\n" +
            $"animator.speed={animator.speed}\n" +
            $"hasShot={hasShot}\n" +
            $"isAttacking={isAttacking}"
        );

        /*
         * すでにShot済みなら何もしない
         */
        if (hasShot)
        {
            Debug.Log("【Shot】すでに実行済みのため無視");
            return;
        }

        /*
         * 攻撃中でなければ無視
         */
        if (!isAttacking)
        {
            Debug.Log("【Shot】攻撃中ではないため無視");
            return;
        }

        hasShot = true;

        /*
         * FanShot / RandomShotをランダム選択
         */
        int randomIndex = Random.Range(0, 2);

        attackType = (AttackType)randomIndex;

        Debug.Log(
            $"【Shot】attackType={attackType} " +
            $"speed={animator.speed}"
        );


        switch (attackType)
        {
            case AttackType.FanShot:

                Debug.Log("【Shot】FanShot開始");

                /*
                 * FanShotはAnimatorを停止しない
                 */
                animator.speed = 1f;

                ShotFan();

                break;


            case AttackType.RandomShot:

                Debug.Log("【Shot】RandomShot開始");

                ShotRandomStart();

                break;
        }
    }


    /*
     * FanShot
     *
     * 50 / 25 / 0 / -25 / -50
     * の5発を一度に発射
     */
    private void ShotFan()
    {
        Debug.Log(
            $"【ShotFan】開始 " +
            $"frame={Time.frameCount} " +
            $"speed={animator.speed}"
        );

        float fanAngle = 50f;

        for (int i = 0; i < 5; i++)
        {
            CreateFireBall(fanAngle);

            fanAngle -= 25f;
        }

        Debug.Log("【ShotFan】5発発射完了");
    }


    /*
     * RandomShot開始
     */
    private void ShotRandomStart()
    {
        Debug.Log(
            $"【ShotRandomStart】開始 " +
            $"frame={Time.frameCount}"
        );

        /*
         * Animationを一時停止
         */
        animator.speed = 0f;

        count = 0;
        timer = 0f;
        isShooting = true;

        /*
         * 1発目は即時発射
         */
        ShootRandomFireBall();
    }


    /*
     * RandomShot
     *
     * 0.5秒ごとに1発
     */
    private void ShootRandomFireBall()
    {
        if (!isAttacking)
        {
            return;
        }

        float randomAngle =
            Random.Range(-5, 6) * 5f;

        Debug.Log(
            $"【RandomFireBall】 " +
            $"count={count + 1}/5 " +
            $"angle={randomAngle}"
        );

        CreateFireBall(randomAngle);

        count++;

        /*
         * 5発終了
         */
        if (count >= 5)
        {
            isShooting = false;
            count = 0;
            timer = 0f;

            /*
             * アニメーション再開
             */
            animator.speed = 1f;

            Debug.Log(
                $"【RandomFireBall】5発完了 → " +
                $"Animator再開 speed={animator.speed}"
            );
        }
    }


    /*
     * FireBall生成
     */
    private void CreateFireBall(float shotAngle)
    {
        if (fireBall == null)
        {
            Debug.LogError(
                "【CreateFireBall】fireBallプレハブが設定されていません"
            );

            return;
        }

        if (enemyStatus == null)
        {
            Debug.LogError(
                "【CreateFireBall】enemyStatusが設定されていません"
            );

            return;
        }

        if (transform.parent == null)
        {
            Debug.LogError(
                "【CreateFireBall】親Transformがありません"
            );

            return;
        }


        Vector3 spawnPosition =
            transform.parent.position +
            new Vector3(
                0.2f * enemyDir,
                0f,
                -1f
            );


        Quaternion rotation =
            Quaternion.Euler(
                -enemyDir,
                0f,
                shotAngle * enemyDir
            );


        GameObject newFireBall =
            Instantiate(
                fireBall,
                spawnPosition,
                rotation
            );


        if (newFireBall == null)
        {
            Debug.LogError(
                "【CreateFireBall】Instantiateに失敗しました"
            );

            return;
        }


        FireBall fireBallScript =
            newFireBall.GetComponent<FireBall>();


        if (fireBallScript == null)
        {
            Debug.LogError(
                "【CreateFireBall】FireBallコンポーネントがありません"
            );

            Destroy(newFireBall);

            return;
        }


        fireBallScript.SetParameter(
            enemyStatus.AttackPower,
            shotAngle,
            enemyDir,
            targetLayer
        );
    }


    /*
     * 攻撃終了
     */
    private void FinishAttack()
    {
        Debug.Log(
            $"★★ FinishAttack実行 " +
            $"frame={Time.frameCount} " +
            $"speed={animator.speed} " +
            $"normalizedTime={animator.GetCurrentAnimatorStateInfo(0).normalizedTime}"
        );


        /*
         * 攻撃状態を終了
         */
        isAttacking = false;
        isShooting = false;

        count = 0;
        timer = 0f;
        hasShot = false;


        /*
         * Animatorは必ず通常速度に戻す
         */
        animator.speed = 1f;


        /*
         * プレイヤーが攻撃範囲内か確認
         */
        bool callAttack = CallAttack();


        if (!callAttack)
        {
            Debug.Log("攻撃範囲外 → BossFireBallShot停止");

            animator.SetBool("Shot", false);

            /*
             * OnDisableを発生させる
             */
            gameObject.SetActive(false);

            return;
        }


        /*
         * 攻撃範囲内の場合
         *
         * ここでは移動を有効化しない。
         * また、自分自身もDisableしない。
         *
         * 次の攻撃開始時には
         * StartAttack()を呼ぶ。
         */
        Debug.Log("【FinishAttack】攻撃範囲内 → 待機状態");
    }


    /*
     * プレイヤーが攻撃範囲内か確認
     */
    private bool CallAttack()
    {
        if (playerTr == null)
        {
            Debug.LogWarning(
                "【CallAttack】playerTrがありません"
            );

            return false;
        }


        Vector2 direction =
            playerTr.position -
            transform.position;


        float distance =
            direction.magnitude;


        /*
         * 距離判定
         */
        if (distance > attackDistance)
        {
            Debug.Log(
                $"【CallAttack】距離外 " +
                $"distance={distance} / " +
                $"attackDistance={attackDistance}"
            );

            return false;
        }


        /*
         * 攻撃方向
         */
        Vector2 forward;

        if (transform.parent != null)
        {
            forward =
                transform.parent.right *
                enemyDir;
        }
        else
        {
            forward =
                transform.right *
                enemyDir;
        }


        /*
         * 角度判定
         */
        float angle =
            Vector2.Angle(
                forward,
                direction
            );


        if (angle > attackAngle / 2f)
        {
            Debug.Log(
                $"【CallAttack】角度外 " +
                $"angle={angle} / " +
                $"limit={attackAngle / 2f}"
            );

            return false;
        }


        Debug.Log("【CallAttack】攻撃範囲内");

        return true;
    }
}