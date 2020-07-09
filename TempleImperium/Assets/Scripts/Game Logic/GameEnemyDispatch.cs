using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase


public class GameEnemyDispatch : MonoBehaviour
{
    //this script manages instantiating enemies.

    //private List<Vector3> m_spawnpoints  

    void Start()
    {
        //TODO
        //acquire list of all nav node transforms on start
        //acquire object reference for player

        Debug.LogError("GameEnemyDispatch.Start() still needs to be programmed! waiting on build merge.");
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


        Debug.LogError("GameEnemyDispatch.DispatchSubwave() still needs to be programmed! waiting on build merge.");
    }
}
