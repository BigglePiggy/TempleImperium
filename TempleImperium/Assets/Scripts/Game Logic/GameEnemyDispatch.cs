using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//written by Ase & Eddie

public class GameEnemyDispatch : MonoBehaviour
{
    //Game Enemy Dispatch Script - There should be only one insance of me!
    //What this script does:
    /*
        - Handles enemy spawning
        - Manages which spawnpoints are viable 
    */

    #region Declarations
    [Header("Enemy Prefabs")]
    [Tooltip("The prefabs for each enemy type")]
    public GameObject m_lightEnemy;
    public GameObject m_mediumEnemy;
    public GameObject m_heavyEnemy;

    Transform m_player;                 //Player transform reference
    List<Vector3> m_allSpawnpoints;   //Stores all spawnpoint transforms  
    #endregion

    //Initalization
    void Start()
    {
        Transform spawnpointsObject = GameObject.FindGameObjectWithTag("Spawnpoints").transform; //The spawnpoints root 

        m_allSpawnpoints = new List<Vector3>(); //Initalizes the list
        for (int i = 0; i < spawnpointsObject.childCount; i++)
        { m_allSpawnpoints.Add(spawnpointsObject.GetChild(i).transform.position); } //Adds each spawnpoint's position to m_allSpawnpoints

        m_player = GameObject.FindGameObjectWithTag("Player").transform; //Player transform reference
    }

    //Spawns designered number of enemies at spawnpoints out of sight from the player - Used by GameLogic 
    public void DispatchSubwave(int input_numLight, int input_numMedium, int input_numHeavy, GameLogic.StarstoneElement input_element)
    {
        //Holds viable spawnpoints
        List<Vector3> obscuredSpawnpoints = new List<Vector3>();

        //Finds spawnpoints that are not visible
        for (int i = 0; i < m_allSpawnpoints.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Linecast(m_player.position, m_allSpawnpoints[i], out hit))  //If something is between the spawnpoint and the player 
            {
                if (hit.transform.position != m_player.position)    //And it isnt the player
                { obscuredSpawnpoints.Add(m_allSpawnpoints[i]); }   //Then it is obstructed and viable to spawn at
            }
        }

        //Orders the with the closest at the top
        obscuredSpawnpoints = obscuredSpawnpoints.OrderBy(x => Vector3.Distance(x, m_player.position)).ToList();

        //Instantiates enemies at unquie spawnpoints
        for (int i = 0; i < input_numLight; i++)
        {
            Instantiate(m_lightEnemy, obscuredSpawnpoints[0], Quaternion.identity).GetComponent<LightEnemyController>().Initialize(input_element);
            if (obscuredSpawnpoints.Count > 1)      //Stops the list from being empty
            { obscuredSpawnpoints.RemoveAt(0); }
        }

        for (int i = 0; i < input_numMedium; i++)
        {
            Instantiate(m_mediumEnemy, obscuredSpawnpoints[0], Quaternion.identity).GetComponent<MediumEnemyController>().Initialize(input_element);
            if (obscuredSpawnpoints.Count > 1)      //Stops the list from being empty
            { obscuredSpawnpoints.RemoveAt(0); }
        }

        for (int i = 0; i < input_numHeavy; i++)
        {
            Instantiate(m_heavyEnemy, obscuredSpawnpoints[0], Quaternion.identity).GetComponent<HeavyEnemyController>().Initialize(input_element);
            if (obscuredSpawnpoints.Count > 1)      //Stops the list from being empty
            { obscuredSpawnpoints.RemoveAt(0); }
        }
    }
}
