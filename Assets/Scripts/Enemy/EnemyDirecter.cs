using UnityEngine;

public class EnemyDirecter : MonoBehaviour
{
    [SerializeField] Enemy enemyStatus;
    int enemyHp;

    void Start()
    {
        enemyHp = enemyStatus.Hp;
        Debug.Log(enemyHp);
    }

    public int Damage(int attackPower)
    {
        //ダメージ計算：アルテリオス計算式*0.8~1.2の乱数
        enemyHp -= Mathf.FloorToInt((attackPower - enemyStatus.DefensePower) * Random.Range(0.8f, 1.1f));
        Debug.Log(enemyHp);
        return enemyHp;
    }

    public void Death()
    {
        Debug.Log("倒された");
        gameObject.SetActive(false);
    }
}
