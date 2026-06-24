using UnityEngine;

public class EnemyDirecter : MonoBehaviour, IDamageable
{
    [SerializeField] Enemy enemyStatus;//エネミーのスクリプタブルオブジェクト
    int enemyHp;//エネミーの体力

    void Start()
    {
        //スクリプタブルオブジェクトで初期化
        enemyHp = enemyStatus.Hp;
        Debug.Log(enemyHp);
    }

    public void Damage(int attackPower)
    {
        //ダメージ計算：アルテリオス計算式*0.8~1.2の乱数
        enemyHp -= Mathf.FloorToInt(attackPower - enemyStatus.DefensePower);
        Debug.Log("敵："+ enemyHp);

        //死亡判定
        if (enemyHp <= 0)
        {
            Debug.Log("倒された");
            Destroy(gameObject);
            //return true;
            //Death();
        }
        //return false;
    }

    //public void Death()
    //{
    //    ////オブジェクト削除
    //    //Debug.Log("倒された");
    //    //Destroy(gameObject);
    //}
}
