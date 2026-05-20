using UnityEngine;

public class PlayerDirecter : MonoBehaviour, IDamageable
{
    [SerializeField] Player playerStatus;
    int playerHp;

    void Start()
    {
        playerHp = playerStatus.HP;
        Debug.Log(playerHp);
    }

    public int Damage(int attackPower)
    {
        //ダメージ計算：アルテリオス計算式*0.8~1.2の乱数
        playerHp -= Mathf.FloorToInt((attackPower - playerStatus.DefensePower)* Random.Range(0.8f,1.1f));
        Debug.Log(playerHp);
        return playerHp;
    }

    public void Death()
    {
        Debug.Log("倒された");
    }
}
