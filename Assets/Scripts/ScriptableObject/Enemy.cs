using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject
{
    //インスペクターから値の設定
    //基本情報
    [SerializeField] string enemyName;//エネミーの名前
    [SerializeField] bool repop;//リポップする、しない
    [SerializeField] int hp;//最大Hp
    [SerializeField] int attackPower;//攻撃力
    [SerializeField] int defensePower;
    [SerializeField] float walkForce;//移動速度

    //プレイヤー検知のための値
    [SerializeField] float viewDistance;//プレイヤー検知範囲（視界距離）
    [SerializeField] float viewAngle;//プレイヤーの検知角度（視野角）

    //攻撃が当たる距離までプレイヤー近づいたか調べるための値
    [SerializeField] float attackDistance;
    [SerializeField] float attackAngle;

    [SerializeField] int exp;
    //[SerializeField] int[] dropItems;

    //get-only
    public string EnemyName => enemyName;
    public bool Repop => repop;
    public int Hp => hp;
    public int AttackPower => attackPower;
    public int DefensePower => defensePower;
    public float WalkForce => walkForce;
    public float ViewDistance => viewDistance;
    public float ViewAngle => viewAngle;
    public float AttackDistance => attackDistance;
    public float AttackAngle => attackAngle;
}
