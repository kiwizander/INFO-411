using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class GameManager : MonoBehaviour
{
    public SpawnManager spawnManager;
    public CSVFileWriter cSVFileWriter;
    public List<GameObject> nodes;
    public float infectionNumber = 0.5f;

    public float TimeToInfectX;
    // Start is called before the first frame update
    public List<float> SimulationTimes;
    public List<float> edgeDependentNumber;
    public static float edgePDependent = 0.3f;
    public int counter = 0;

    void Start()
    {
        StartCoroutine(Infect());
    }

    //Continously add to 'infectionNumber'. This will reset when an infection occurs
    void Update()
    {
        TimeToInfectX += Time.deltaTime;
    }

    public void ResetSimulation()
    {

        SimulationTimes.Add(TimeToInfectX);
        TimeToInfectX = 0;
        foreach(var simulatedNodes in nodes)
        {
            simulatedNodes.GetComponent<Node>().infected = false;
        }
        infectionNumber = 0.5f;
        if(counter % 50 == 0)
        {
            // Different experiment where we change the dependency between nodes
            edgePDependent += 0.2f;
        }
        counter++;
        edgeDependentNumber.Add(edgePDependent);        
        OutputCsv();
    }

    void OutputCsv()
    {
        if(SimulationTimes.Count == 150)
        {
            Debug.Log("Writing to CSV");
            cSVFileWriter.WriteCSV(edgeDependentNumber, SimulationTimes);
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    //The inevitable counter of infection. The variable infectionNumber increases over time. At each timestep it randomly
    //selects a node. If the infectionNumber is greater than that node, the node becomes infected. If the infectionNumber
    //is greater than that node, and the node is not infected it resets the infection number. 
    
    IEnumerator Infect()
    {
        if(nodes.Count > 0)
        {
            int infected = Random.Range(0, nodes.Count);
            GameObject infectedNode = nodes[infected];
            if(infectionNumber > infectedNode.GetComponent<Node>().versionCount )
            {
                if(!infectedNode.GetComponent<Node>().infected)
                {
                    Debug.Log("Infection!: " + infectedNode.name);
                    infectedNode.GetComponent<Node>().infected = true;
                    infectedNode.GetComponent<Node>().ChangeMaterial("red");
                    infectedNode.GetComponent<Node>().StartRecover();
                    infectionNumber = 0.5f; 
                }

                Start();
                yield break;
            }
        }
        infectionNumber += 0.1f;
        float timeToInfection = 1f;
        yield return new WaitForSeconds(timeToInfection);
        StartCoroutine(Infect());
    }

}
