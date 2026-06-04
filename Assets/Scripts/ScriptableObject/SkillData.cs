using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    public enum StatusUp
    {
        AtkUp,
        AtkMul,
        DefUp,
        DefMul,
        SkillLv
    }

    [SerializeField] int skillId;
    [SerializeField] StatusUp skillTopic;
    [SerializeField] float parameter;
    [SerializeField] List<int> prerequisites;
    [SerializeField] List<int> nextSkills;

    public int SkillId => SkillId;
    public StatusUp SkillTopic => skillTopic;
    public float Parameter => parameter;
    public List<int> Prerequisites => prerequisites;
    public List<int> NextSkills => nextSkills;
}
