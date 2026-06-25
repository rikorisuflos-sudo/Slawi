using UnityEngine;
using UnityEngine.UIElements;

public class EnemyDirecter : MonoBehaviour, IDamageable, IAbsorble
{
    [SerializeField] Enemy enemyStatus;//エネミーのスクリプタブルオブジェクト
    public int enemyHp { get; set;}//エネミーの体力

    void Start()
    {
        //スクリプタブルオブジェクトで初期化
        enemyHp = enemyStatus.Hp;
        Debug.Log(enemyHp);
    }

    public void Damage(int attackPower)
    {
        //ダメージ計算：アルテリオス計算式
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

    public void Absorb()
    {
        int random = Random.Range(1, 100);

        int chances;//確率

        float hpRatio = (float)enemyHp / enemyStatus.Hp;
        Debug.Log("Hp割合" + hpRatio);

        switch (hpRatio)
        {
            case >=1.0f:
                chances = 1;
                Debug.Log(chances);
                break;
            case >=0.5f:
                chances = 5;
                Debug.Log(chances);
                break;
            case >=0.2f:
                chances = 35;
                Debug.Log(chances);
                break;
            case >=0.05f:
                chances = 60;
                Debug.Log(chances);
                break;
            default:
                chances = 1000;
                Debug.Log(chances);
                break;
        }

        Debug.Log("確率" + chances + ",乱数" + random);

        if (random <= chances)
        {
            Destroy(gameObject);
            Debug.Log("吸収された");
        }
    }

    //public void Death()
    //{
    //    ////オブジェクト削除
    //    //Debug.Log("倒された");
    //    //Destroy(gameObject);
    //}
}
