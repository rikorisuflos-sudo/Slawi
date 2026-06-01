using UnityEngine;

public class Skill : MonoBehaviour
{

    private int attacPoint;
   enum StatusUp
    {
        AttacUp1,
        AttacUp2,
        AttacMul,
        DefenseUp1,
        DefenseUp2,
        DefenseMul
    }

    enum SkillUp
    {
        Skill1,
        Skill2,
        Skill3
    }

    int AttacUp(int attac)
    {

        return attac;
    }

    int AttacMul()
    {
        return attacPoint;
    }

    int DefenseUp (int defense)
    {
        return defense;
    }
}
