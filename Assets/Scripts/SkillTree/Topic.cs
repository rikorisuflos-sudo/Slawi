using System;
using UnityEngine;

public class Topic : MonoBehaviour
{

    int attackPoint;
    float attackMul;
    int defensePoint;
    float defenseMul;
    int skillLv;

    enum StatusUp
    {
        AtkUp,
        AtkMul,
        DefUp,
        DefMul,
        SkillLv
    }

    void Start()
    {
            
    }

    int AttacUp(float upPoint)
    {
        attackPoint += (int)upPoint;
        return attackPoint;
    }

    float AttacMul(float upMul)
    {
        attackMul += upMul;
        return attackMul;
    }

    int DefenseUp(float upPoint)
    {
        defensePoint += (int)upPoint;
        return defensePoint;
    }

    float DefenseMul(float upMul)
    {
        defenseMul += upMul;
        return defenseMul;
    }

    int SkillLv()
    {
        skillLv++;
        return skillLv;
    }

    //public void CastState(string input)
    //{
    //    if (Enum.TryParse(input, out StatusUp statusUp))
    //    {
    //        currentStatusUp = statusUp;
    //    }
    //}
}
