﻿using System.Collections;
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

    /// <summary>
    /// get a UnityEngine Color corresponding to a starstone element
    /// </summary>
    public Color GetStarstoneElementColour(GameLogic.StarstoneElement input_element)
    {
        Color m_cOutput = Color.white;
        switch (input_element)
        {
            case GameLogic.StarstoneElement.Fire:
                m_cOutput = new Color(0.9f, 0.2f, 0.2f);    //fire red
                break;
            case GameLogic.StarstoneElement.Water:
                m_cOutput = new Color(0.2f, 0.5f, 0.9f);    //water blue
                break;
            case GameLogic.StarstoneElement.Electricity:
                m_cOutput = new Color(0.9f, 0.9f, 0.4f);    //electricity yellow
                break;
            case GameLogic.StarstoneElement.Darkness:
                m_cOutput = new Color(0.5f, 0.1f, 0.7f);    //darkness purple
                break;

            default:
                //whoops!
                Debug.LogError("GenericFunctions.GetStarstoneElementColour given unaccounted for StarstoneElement \"" + input_element + "\"!");
                break;
        }

        return m_cOutput;
    }


#if UNITY_EDITOR
    void Start()
    {
        //layer 8 error catcher
        Debug.LogError("GenericFunctions script attached to an object! Remove me, I do nothing on my own!");
    }
#endif
}
