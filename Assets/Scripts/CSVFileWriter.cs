using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVFileWriter : MonoBehaviour
{
    string filename = "";

    List<float> simulationTimes;
    void Start()
    {
        filename = Application.dataPath + "/simulation_pDependent_peripheral_B_D_final.csv";        
    }

    public void WriteCSV(List<float> pDependent, List<float> simulationTimes)
    {
        if(simulationTimes.Count > 0)
        {
            TextWriter tw = new StreamWriter(filename, false); // Using false to overrite initially
            //Headings "," for each column
            tw.WriteLine("Dependency percentage", "Simulation Time");
            tw.Close();

            tw = new StreamWriter(filename, true);
            for (int i = 0; i < simulationTimes.Count; i++)
            {
                tw.WriteLine(pDependent[i] + "," + simulationTimes[i]);
            }
            tw.Close();
            Application.Quit();
        }
    }
}
