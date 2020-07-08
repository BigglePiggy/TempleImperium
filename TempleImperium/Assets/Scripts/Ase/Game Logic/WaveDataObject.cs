﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase


public class WaveDataObject
{
    //wave data container (C# object, NOT a Unity GameObject)


    public float m_iWaveDuration;
    public float m_iSubWaveRestDuration;
    public int m_iSubWaveKillLenience;
    public GameLogic.StarstoneElement m_eStarstoneElement;
    public int[,] m_iSubWavesArray;

    public WaveDataObject(float input_WaveDuration, float input_SubWaveRestDuration, int input_SubWaveKillLenience,
        GameLogic.StarstoneElement input_StarstoneElement,int[,] input_SubWavesArray)
    {
        m_iWaveDuration             = input_WaveDuration;
        m_iSubWaveRestDuration      = input_SubWaveRestDuration;
        m_iSubWaveKillLenience      = input_SubWaveKillLenience;
        m_eStarstoneElement         = input_StarstoneElement;
        m_iSubWavesArray            = input_SubWavesArray;
    }

#if UNITY_EDITOR
    void Start()
    {
        Debug.LogError("WaveDataObject script attached to an object! Use WaveData instead!");
    }
#endif

}