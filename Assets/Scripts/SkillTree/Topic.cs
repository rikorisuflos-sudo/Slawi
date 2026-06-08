using System;
using UnityEngine;

public class Topic : MonoBehaviour
{
    [SerializeField]TreeData treeData;
    NodeData nodeData;

    int attackPoint;
    float attackMul;
    int defensePoint;
    float defenseMul;
    int skillLv =1;

    int AttackUp(float upPoint)
    {
        Debug.Log("UŒ‚—Í‚ªã‚ª‚Á‚½");
        attackPoint += (int)upPoint;
        return attackPoint;
    }

    float AttackMul(float upMul)
    {
        Debug.Log("UŒ‚%‚ªã‚ª‚Á‚½");
        attackMul += upMul;
        return attackMul;
    }

    int DefenseUp(float upPoint)
    {
        Debug.Log("–hŒä—Í‚ªã‚ª‚Á‚½");
        defensePoint += (int)upPoint;
        return defensePoint;
    }

    float DefenseMul(float upMul)
    {
        Debug.Log("UŒ‚“‚ªã‚ª‚Á‚½");
        defenseMul += upMul;
        return defenseMul;
    }

    int SkillLv()
    {
        Debug.Log("ƒXƒLƒ‹ƒŒƒxƒ‹‚ªã‚ª‚Á‚½");
        skillLv++;
        return skillLv;
    }

    public void CallStatusUp(int num)
    {
        nodeData = treeData.TreeTopic[num];
        Debug.Log(nodeData);
        switch (nodeData.skillTopic)
        {
            case StatusUp.AtkUp:
                AttackUp(nodeData.Parameter);
                break;
            case StatusUp.AtkMul:
                AttackMul(nodeData.Parameter);
                break;
            case StatusUp.DefUp:
                DefenseUp(nodeData.Parameter);
                break;
            case StatusUp.DefMul:
                DefenseMul(nodeData.Parameter);
                break;
            case StatusUp.SkillLv:
                SkillLv();
                break;
        }
    }
}
