using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject
{
    //基本情報
    [SerializeField] string enemyName;//エネミーの名前
    [SerializeField] int Hp;//最大Hp
    [SerializeField] int attackPower;//攻撃力
    [SerializeField] float walkForce;//移動速度

    //プレイヤー検知のための値
    [SerializeField] float viewDistance;//プレイヤー検知範囲（視界距離）
    [SerializeField] float viewAngle;//プレイヤーの検知角度（視野角）

    //攻撃が当たる距離までプレイヤー近づいたか調べるための値
    [SerializeField] float attackDistance;
    [SerializeField] float attackAngle;
}
