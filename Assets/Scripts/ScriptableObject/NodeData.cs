using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class NodeData : ScriptableObject
{
    [SerializeField] int skillId;
    public StatusUp skillTopic;
    [SerializeField] float parameter;
    [SerializeField] List<NodeData> prerequisites;
    [SerializeField] List<NodeData> nextSkills;

    public int SkillId => skillId;
    public float Parameter => parameter;
    public IReadOnlyList<NodeData> Prerequisites => prerequisites;
    public IReadOnlyList<NodeData> NextSkills => nextSkills;
}
