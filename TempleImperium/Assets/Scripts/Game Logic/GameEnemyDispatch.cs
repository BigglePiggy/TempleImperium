using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//written by Ase & Eddie
public class GameEnemyDispatch : MonoBehaviour
{
    //this script manages instantiating enemies.

    public GameObject lightEnemy, mediumEnemy, heavyEnemy;    //Enemy prefab
    private Transform player;   //Player transform 
    private List<Transform> allSpawnpoints;     //All spawnpoints

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
        //Holds viable spawnpoints
        List<Vector3> obscuredSpawnpoints = new List<Vector3>();

        //Finds & assigns spawnpoints that are not visible
        int maxRayLength = 100;
        for (int i = 0; i < allSpawnpoints.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(player.position, allSpawnpoints[i].position - player.position, out hit, maxRayLength)) 
            { obscuredSpawnpoints.Add(allSpawnpoints[i].position); }
        }

        //Errors
        if(obscuredSpawnpoints.Count - input_numLight -input_numMedium-input_numHeavy <= 0) 
        { Debug.Log("Not enough spots, only " + obscuredSpawnpoints.Count); }

        if (obscuredSpawnpoints == null || obscuredSpawnpoints.Count == 0)
        { Debug.Log("No spots"); }

        //Instantiates enemies at unquie spawnpoints
        for (int i = 0; i < input_numLight; i++)
        {
            int index = Random.Range(0, obscuredSpawnpoints.Count - 1);
            Instantiate(lightEnemy, obscuredSpawnpoints[index], Quaternion.identity);
            obscuredSpawnpoints.RemoveAt(index);
        }

        for (int i = 0; i < input_numMedium; i++)
        {
            int index = Random.Range(0, obscuredSpawnpoints.Count - 1);
            Instantiate(lightEnemy, obscuredSpawnpoints[index], Quaternion.identity);
            obscuredSpawnpoints.RemoveAt(index);
        }

        for (int i = 0; i < input_numHeavy; i++)
        {
            int index = Random.Range(0, obscuredSpawnpoints.Count - 1);
            Instantiate(lightEnemy, obscuredSpawnpoints[index], Quaternion.identity);
            obscuredSpawnpoints.RemoveAt(index);
        }
    }
}
