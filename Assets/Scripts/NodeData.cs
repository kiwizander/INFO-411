using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeData", menuName = "ScriptableObjects/NodeScriptableObject")]
public class NodeData : ScriptableObject
{
    public string nodeName;

    public int versionCounts;
   
    public ScriptableObject[] dependencies;
}
