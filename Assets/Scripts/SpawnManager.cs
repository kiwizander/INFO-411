using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<NodeData> nodes;
    public GameObject nodePrefab;
    public GameManager gameManager;
    public GameObject spawnPrefab;
    public List<Transform> spawnPositions;
    public delegate void FinishedSpawning();
    public static event FinishedSpawning OnSpawned;


    //Spawn the nodes at random positions in the grid
    void Start()
    {
        for(int i = -10; i < 10; i ++)
        {
            for(int j = 4; j > -4; j --)
            {
                Vector3 spawnPosition = new Vector3(i, j, 0);
                GameObject pos = Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);
                pos.name = "Spawn Position " + i + j;
                spawnPositions.Add(pos.transform);
                pos.transform.SetParent(this.transform);
            }
        }
        foreach(var node in nodes)
        {
            NodeData nodeInformation = node;
            GameObject nextNode;
            int randomSpot = Random.Range(0, spawnPositions.Count);
            nextNode = Instantiate(nodePrefab, spawnPositions[randomSpot].position, Quaternion.identity);
            nextNode.GetComponent<Node>().nodeData = nodeInformation;
            nextNode.GetComponent<Node>().AccessNodeInformation();
            gameManager.GetComponent<GameManager>().nodes.Add(nextNode);
            spawnPositions.RemoveAt(randomSpot);
        }
        if(OnSpawned != null)
        {
            OnSpawned();
        }
    }
}
