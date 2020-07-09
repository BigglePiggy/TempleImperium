using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//written by Ase


public class GenericFunctions
{
    //generic-use funcions
    //i couldn't figure out how to make this static. sorry!

    public int ConvertSecondsToTicks (float input_seconds, int input_tickrate = 60)
    {
        return Mathf.FloorToInt(input_seconds * input_tickrate);    //return floor(seconds*tickrate) as int
    }

    public float ConvertTickstoSeconds (float input_ticks, int input_tickrate = 60)
    {
        return input_ticks / input_tickrate;  //return ticks/tickrate
    }


#if UNITY_EDITOR
    void Start()
    {
        //layer 8 error catcher
        Debug.LogError("GenericFunctions script attached to an object! Remove me, I do nothing on my own!");
    }
#endif
}
