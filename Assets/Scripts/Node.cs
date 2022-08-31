using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public NodeData nodeData;    
    public ScriptableObject[] dependencies;
    public List<GameObject> dependentGameObjects;
    public List<GameObject> dependentEdges;
    public GameObject edge;
    public bool infected;
    public int versionCount;
    public Material[] materials;
    private Renderer objectRenderer;
    private int timeToRecover;
    public delegate void XInfected();
    public static event XInfected OnXInfected;
    GameManager gameManager;

    //Subscribe to the event that all nodes are finished spawning, so can create our edges
    void OnEnable()
    {
        SpawnManager.OnSpawned += CreateEdges;
    }


    void OnDisable()
    {
        SpawnManager.OnSpawned -= CreateEdges;
    }

    //Called before edge creation (which relies on finding other nodes by their name in the scene)
    public void AccessNodeInformation()
    {
        name = nodeData.nodeName;
        dependencies = nodeData.dependencies;
        versionCount = nodeData.versionCounts;

    }

    public void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        ChangeMaterial("yellow");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    //For each dependency the node has, create an edge pointing to it
    public void CreateEdges()
    {
        foreach(var dep in dependencies)
        {
         
            GameObject dependentNode = GameObject.Find(dep.name);
            dependentGameObjects.Add(dependentNode);
            Vector3 direction = dependentNode.transform.position - gameObject.transform.position;
            Vector3 startPosition  = gameObject.transform.position + direction * 0.5f;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.Euler(0f, 0f, -angle);
            GameObject newEdge = Instantiate(edge, startPosition, q);
            newEdge.name = gameObject.name + " - " + dependentNode.name + " Edge";
            newEdge.GetComponent<Edge>().UpdateStartEndNodes(this.gameObject, dependentNode);
            dependentEdges.Add(newEdge);
        }
        StartCoroutine(AccessInfection());
    }

    //Based on whether the node is infected or not, change the colour of the node
    public void ChangeMaterial(string colour)
    {
        if(colour == "red")
        {
            objectRenderer.material.color = Color.red;            
        }
        else if(colour == "yellow")
        {
            objectRenderer.material.color = Color.yellow;
        }

    }

    //Once the node is infected, start a recovery countdown process
    public void StartRecover()
    {
        timeToRecover = 10;
        StartCoroutine(Recover()); 
    }

    //The recovery countdown process. Based upon the version count of the node. In this simulation nodes with 
    //higher version counts will be updated faster and therefore have a shorter time to recovery
    IEnumerator Recover()
    {
        if(timeToRecover <= versionCount)
        {
            infected = false;
            Debug.Log(this.gameObject.name + " has recovered!");
            yield break;
        }
        timeToRecover -= 1;
        yield return new WaitForSeconds(1f);  
        StartCoroutine(Recover());      
    }

    //At each given timestep a node will pick a random number for each nodes it is dependent on. If that number is less than the 
    //pDependent, then it has 'accessed' then node. IE if a node is 100% dependent on another node, then at any given timestep
    //it will be 100% reliant on if it has an infection.

    //Once it accesses the node, it then checks if it is infected. If so, then the node received the infection from its dependency

    //If the dependency itself is not infected, it will search through each of the dependency node dependencies. THis is recursive so
    //will keep going down each connection at each given timestep. If A is dependent on B and B dependent on C, then A can come into
    //contact with the infection through B 

    //Package X is our package we are simulating, therefore printing out the time it takes to reach X in red
    IEnumerator AccessInfection()
    {
        SearchEdges(this.gameObject);
        yield return new WaitForSeconds(1f);
        StartCoroutine(AccessInfection());
    }

    //Searches through the edges (ie what packages this node is dependent on) to try and access them. 
    //If those nodes are not infected, then it will search through their dependencies etc.
    //This is recursive so works for any length of branch 
    void SearchEdges (GameObject currentNode)
    {
        List<GameObject> currentNodeDependentEdges = currentNode.GetComponent<Node>().dependentEdges;
        foreach(var edges in currentNodeDependentEdges)
        {
            float chance = Random.Range(0f, 1f);
            if(chance < Edge.pDependent)
            {                    
                GameObject targetNode = edges.GetComponent<Edge>().endNode;

                if(targetNode.GetComponent<Node>().infected)
                {
                    if(this.name == "Package X")
                    {
                        Debug.LogError("Accessed infection! From " + this.name + " to " + targetNode.name + " time: " + gameManager.TimeToInfectX.ToString() + "with dependency " + Edge.pDependent);
                        gameManager.ResetSimulation();
                    }
                    else{
                    Debug.Log("Accessed infection! From " + this.name + " to " + targetNode.name);
                    }
                }
                else if (targetNode.GetComponent<Node>().dependencies.Length > 0)
                {
                    SearchEdges(targetNode);
                }
            }
        }    

    }
}
