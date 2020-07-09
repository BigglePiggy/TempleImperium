using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//written by Ase


public class GameEnemyDispatch : MonoBehaviour
{
    //this script manages instantiating enemies.

    private Transform player;
    private List<Transform> allSpawnpoints;

    void Start()
    {
        //Populates spawnpoints
        Transform spawnpointsObject = GameObject.FindGameObjectWithTag("Spawnpoints").transform;
        allSpawnpoints = new List<Transform>();
        for (int i = 0; i < spawnpointsObject.childCount; i++)
        { allSpawnpoints.Add(spawnpointsObject.GetChild(i).transform); }

        //Player reference
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void DispatchSubwave(int input_numLight, int input_numMedium, int input_numHeavy, GameLogic.StarstoneElement input_element)
    {
        //TODO
        //for every navnode transform saved,
        //do raycast from player transform to navnode transform
        //store list of transforms with failed raycasts (spawn obscured). these are this dispatch's valid locations
        //(throw error if list empty)

        //for i<numLight
        //spawn
        //call instantiation function on enemy, pass in element

        //repeat loops for numMed, numHeavy



        Vector3[] lightSpawnpoints = new Vector3[input_numLight];
        Vector3[] mediumSpawnpoints = new Vector3[input_numMedium];
        Vector3[] heavySpawnpoints = new Vector3[input_numHeavy];

        List<Vector3> obscuredSpawnpoints = new List<Vector3>();

        for (int i = 0; i < allSpawnpoints.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(player.position, player.position - allSpawnpoints[i].position, out hit, Vector3.Distance(player.position, allSpawnpoints[i].position)))
            { obscuredSpawnpoints.Add(allSpawnpoints[i].position); }
        }

        if (obscuredSpawnpoints != null)
        {
            for (int l = 0; l < input_numLight; l++)
            {
                lightSpawnpoints[l] = obscuredSpawnpoints[Random.Range(0, obscuredSpawnpoints.Count - 1)];
                obscuredSpawnpoints.RemoveAt(Random.Range(0, obscuredSpawnpoints.Count - 1));
            }

            for (int m = 0; m < input_numMedium; m++)
            {
                mediumSpawnpoints[m] = obscuredSpawnpoints[Random.Range(0, obscuredSpawnpoints.Count - 1)];
                obscuredSpawnpoints.RemoveAt(Random.Range(0, obscuredSpawnpoints.Count - 1));
            }

            for (int h = 0; h < input_numHeavy; h++)
            {
                heavySpawnpoints[h] = obscuredSpawnpoints[Random.Range(0, obscuredSpawnpoints.Count - 1)];
                obscuredSpawnpoints.RemoveAt(Random.Range(0, obscuredSpawnpoints.Count - 1));
            }  
        }
    }
}
