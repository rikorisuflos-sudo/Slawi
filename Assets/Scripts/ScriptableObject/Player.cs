using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player")]
public class Player : ScriptableObject
{
    //インスペクターから値の設定
    //基本情報
    [SerializeField] string playerName = "player";//プレイヤーの名前
    [SerializeField] int hp;//最大Hp
    [SerializeField] int attackPower;//攻撃力
    [SerializeField] int defensePower;//防御力
    [SerializeField] float walkForce;//移動速度
    [SerializeField] float jumpForce;//ジャンプ速度

    public string PlayerName => playerName;
    public int HP => hp;
    public int AttackPower => attackPower;
    public int DefensePower => defensePower;
    public float WalkForce => walkForce;
    public float JumpForce => jumpForce;

}
