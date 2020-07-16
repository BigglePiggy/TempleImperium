using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase


public class SettingsManager : MonoBehaviour
{
    //stores user settings and pushes to scripts.

    [Header("Default Controls")]


    public int m_iMouseSensitivityX = 5;
    public int m_iMouseSensitivityY = 5;


    /// <summary>
    /// write current SettingsManager config to all scripts
    /// </summary>
    void RefreshControlsOnAllScripts()
    {

    }

    #region filesystem IO
    void WriteToFile()
    {
        //TODO
        Debug.LogWarning("SettingsManager.WriteToFile() needs to be programmed! bug Ase about it");
    }
    void ReadFromFile()
    {
        //TODO
        Debug.LogWarning("SettingsManager.ReadFromFile() needs to be programmed! bug Ase about it");
    }
    #endregion filesystem IO

    void Start()
    {
    }
    void Update()
    {
    }
}
