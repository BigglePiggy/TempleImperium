using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//written by Ase & Eddie
public class GameEnemyDispatch : MonoBehaviour
{
    //this script manages instantiating enemies.

    public GameObject lightEnemy, mediumEnemy, heavyEnemy;    //Enemy prefab
    public float m_spawnpointVariation;
    public float m_lineOfSightCheckRadius;
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
        for (int i = 0; i < allSpawnpoints.Count; i++)
        {
            if (Vector3.Distance(player.position, allSpawnpoints[i].position) < m_lineOfSightCheckRadius)
            {
                RaycastHit hit;
                if (Physics.Linecast(player.position, allSpawnpoints[i].position, out hit))
                {
                    if (hit.transform != allSpawnpoints[i])
                    { obscuredSpawnpoints.Add(allSpawnpoints[i].position); }
                }
            }
            else
            { obscuredSpawnpoints.Add(allSpawnpoints[i].position); }
        }

        //Errors
        if(obscuredSpawnpoints.Count - input_numLight -input_numMedium-input_numHeavy <= 0) 
        { Debug.Log("Not enough spots, only " + obscuredSpawnpoints.Count); }

        if (obscuredSpawnpoints == null || obscuredSpawnpoints.Count == 0)
        { Debug.Log("No spots"); }

        //Shuffles list of valid points
        obscuredSpawnpoints = obscuredSpawnpoints.OrderBy(x => Vector3.Distance(x, player.position)).ToList();

        int totalEnemies = input_numLight + input_numMedium + input_numHeavy;
        if (totalEnemies + totalEnemies * Mathf.CeilToInt(m_spawnpointVariation) > obscuredSpawnpoints.Count) 
        {
            //if total enemies + 25% extra is more than number of available points then use normal list
        }
        else 
        {
            //otherwise make the list the length of total + 25% and randomize it           
            obscuredSpawnpoints.RemoveRange(Mathf.CeilToInt(obscuredSpawnpoints.Count * m_spawnpointVariation), obscuredSpawnpoints.Count - Mathf.CeilToInt(obscuredSpawnpoints.Count * m_spawnpointVariation));
            obscuredSpawnpoints = obscuredSpawnpoints.OrderBy(x => Random.value).ToList();
        }

        //Instantiates enemies at unquie spawnpoints
        for (int i = 0; i < input_numLight; i++)
        {
            Instantiate(lightEnemy, obscuredSpawnpoints[0], Quaternion.identity).GetComponent<LightEnemyController>().Initialize(input_element);            
            obscuredSpawnpoints.RemoveAt(0);
        }

        for (int i = 0; i < input_numMedium; i++)
        {
            Instantiate(mediumEnemy, obscuredSpawnpoints[0], Quaternion.identity).GetComponent<MediumEnemyController>().Initialize(input_element);
            obscuredSpawnpoints.RemoveAt(0);
        }

        for (int i = 0; i < input_numHeavy; i++)
        {
            Instantiate(heavyEnemy, obscuredSpawnpoints[0], Quaternion.identity).GetComponent<HeavyEnemyController>().Initialize(input_element);
            obscuredSpawnpoints.RemoveAt(0);
        }
    }
}
