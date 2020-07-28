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

    MeshRenderer oGlowRenderer;

    //material references
    //TODO: cleanup / auto references (is that needed?)
    public Material matSummon;                
    public Material matArc;
    public Material matHazard;
    public Material matPower;
    public Material matInert;

    GenericFunctions cGenericFunctions = new GenericFunctions(); //instantiate a GenericFunctions for use here

    // Start is called before the first frame update
    void Start()
    {
        oGlowRenderer = oCrystal.transform.Find("Outline").GetComponent<MeshRenderer>();
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
            case GameLogic.StarstoneElement.Summon:
                oCrystal.GetComponent<MeshRenderer>().material = matSummon;
                break;
            case GameLogic.StarstoneElement.Arc:
                oCrystal.GetComponent<MeshRenderer>().material = matArc;
                break;
            case GameLogic.StarstoneElement.Hazard:
                oCrystal.GetComponent<MeshRenderer>().material = matHazard;
                break;
            case GameLogic.StarstoneElement.Power:
                oCrystal.GetComponent<MeshRenderer>().material = matPower;
                break;
        }

        //enable glow
        oGlowRenderer.gameObject.SetActive(true);
        //set zIgnore glow colour
        //oGlowRenderer.material.color = cGenericFunctions.GetStarstoneElementColour(input_starstoneElement);
    }
    /// <summary>
    /// go to inert material
    /// </summary>
    public void GoInert()
    {
        oCrystal.GetComponent<MeshRenderer>().material = matInert;

        //glow -> white
        //oGlowRenderer.material.color = Color.white;
    }

    public void GoCritical()
    {
        //TODO
        //cool generator explosion here
        Debug.LogWarning("GameGenerator has nothing cool for GoCritical() yet");

        //placeholder
        oCrystal.GetComponent<MeshRenderer>().material = matSummon;
        //oGlowRenderer.material.color = Color.red;
    }
}
