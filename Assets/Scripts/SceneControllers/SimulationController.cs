using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelGen;

public class SimulationController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Run() {
        LevelGen.LevelGen.GenerateGrid();
    }
}
