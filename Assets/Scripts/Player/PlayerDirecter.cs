using UnityEngine;

public class PlayerDirecter : MonoBehaviour, IDamageable, IStatusble
{
    [SerializeField] Player playerStatus;
    int playerHp;
    int attackPower;
    int defensePower;
    public int AttackPower => attackPower;

    void Start()
    {
        playerHp = playerStatus.HP;
        attackPower = playerStatus.AttackPower;
        Debug.Log(playerHp);
    }

    public void Damage(int attackPower)
    {
        //ダメージ計算：アルテリオス計算式*0.8~1.2の乱数
        playerHp -= Mathf.FloorToInt((attackPower - playerStatus.DefensePower)* Random.Range(0.8f,1.1f));
        Debug.Log(playerHp);

        //死亡判定
        if (playerHp <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Debug.Log("倒された");
    }

    public void StatusReset()
    {
        attackPower = playerStatus.AttackPower;
        defensePower = playerStatus.DefensePower;
    }

    public void Buff(int plus)
    {
        
    }

    public void Debuff(int minus)
    {
       
    }
}
