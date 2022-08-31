using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    public static float pDependent;
    public GameObject startNode;
    public GameObject endNode;

    public void UpdateStartEndNodes(GameObject startingNode, GameObject endingNode)
    {
        startNode = startingNode;
        endNode = endingNode;
    }

    public void Update()
    {
        pDependent = GameManager.edgePDependent;
    }
}
