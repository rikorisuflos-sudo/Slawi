using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreeData", menuName = "Scriptable Objects/TreeData")]
public class TreeData : ScriptableObject
{
    [SerializeField] List<NodeData> treeData;

    public IReadOnlyList<NodeData> TreeTopic => treeData;
}
