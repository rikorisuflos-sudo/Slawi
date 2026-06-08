using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class NodeData : ScriptableObject
{
    [SerializeField] int skillId;
    public StatusUp skillTopic;
    [SerializeField] float parameter;
    [SerializeField] List<int> prerequisites;
    [SerializeField] List<int> nextSkills;

    public int SkillId => skillId;
    public float Parameter => parameter;
    public IReadOnlyList<int> Prerequisites => prerequisites;
    public IReadOnlyList<int> NextSkills => nextSkills;
}
