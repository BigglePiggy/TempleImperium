using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//written by Eddie and Ase

public class GamePylon : MonoBehaviour
{
    ////Declarations
    //Public
    public float speed;
    public bool startLowered;
    public Vector3 raisedPosition;
    public Vector3 loweredPosition;
    
    //Private
    private Transform core;
    private bool up, down;

    private float startTime;
    private float journeyLength;

    GameObject oGameLogic;  //gameLogic instance reference

    //Initalization
    void Start()
    {
        core = transform.Find("Core");

        if (startLowered)
        { core.localPosition = loweredPosition; }

        else
        { core.localPosition = raisedPosition; }

        //establish reference to gamelogic script
        oGameLogic = GameObject.Find("Game Logic");
    }

    //Called per frame
    void Update()
    {
        if (oGameLogic == null) //if no reference to gamelogic script
        {
        }

        if ((up && core.localPosition != raisedPosition) || (down && core.localPosition != loweredPosition)) 
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;

            if (up)
            { core.localPosition = Vector3.Lerp(core.localPosition, raisedPosition, fractionOfJourney); }

            if (down)
            { core.localPosition = Vector3.Lerp(core.localPosition, loweredPosition, fractionOfJourney); }
        }
    }

    //Raise the pylon
    public void GoUp() 
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(core.localPosition, raisedPosition);
        up = true;
        down = false;
    }

    //Lower the pylon
    public void GoDown()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(core.localPosition, loweredPosition);
        up = false;
        down = true;
    }

    //Collision detection
    private void OnTriggerEnter(Collider ForeignCollider)
    {
        if (up)
        {
            //if foreign collider is the player, this pylon goes down
            if (ForeignCollider.tag == "Player") { GoDown(); }

            //tell gamelogic to run pylon down func
            oGameLogic.GetComponent<GameLogic>().WaveEventPylonLoweredByPlayer();
        }
    }
}
