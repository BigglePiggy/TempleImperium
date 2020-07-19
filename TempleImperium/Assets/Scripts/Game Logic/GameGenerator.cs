using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase


public class GameGenerator : MonoBehaviour
{
    //generator object
    /*
    responsible for:
    - visual in-level indicator for wave element
    */

    public GameObject oCrystal;

    //material references
    //TODO: cleanup / auto references (is that needed?)
    public Material matFire;                
    public Material matWater;
    public Material matElectricity;
    public Material matDarkness;
    public Material matInert;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// set crystal material based on element
    /// //todo: move me to GenericFunctions? how would genericfunctions get the mat refs?
    /// </summary>
    /// <param name="input_starstoneElement"></param>
    public void SetElement(GameLogic.StarstoneElement input_starstoneElement)
    {

        switch (input_starstoneElement)
        {
            case GameLogic.StarstoneElement.Fire:
                oCrystal.GetComponent<MeshRenderer>().material = matFire;
                break;
            case GameLogic.StarstoneElement.Water:
                oCrystal.GetComponent<MeshRenderer>().material = matWater;
                break;
            case GameLogic.StarstoneElement.Electricity:
                oCrystal.GetComponent<MeshRenderer>().material = matElectricity;
                break;
            case GameLogic.StarstoneElement.Darkness:
                oCrystal.GetComponent<MeshRenderer>().material = matDarkness;
                break;
        }
    }
    /// <summary>
    /// go to inert material
    /// </summary>
    public void GoInert()
    {
        oCrystal.GetComponent<MeshRenderer>().material = matInert;
    }

    public void GoCritical()
    {
        //TODO
        //cool generator explosion here
        Debug.LogWarning("GameGenerator has nothing cool for GoCritical() yet");
    }
}
