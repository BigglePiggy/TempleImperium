using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Written by Eddie

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

    //Initalization
    void Start()
    {
        core = transform.Find("Core");

        if (startLowered)
        { core.localPosition = loweredPosition; }

        else
        { core.localPosition = raisedPosition; }
    }

    //Called per frame
    void Update()
    {
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
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player") 
        { GoDown(); }
    }
}
