using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//written by Ase


public class HUDController : MonoBehaviour
{
    //this script acts as a controller for various HUD elements.

    //pulling custom/arbitrary data into the HUD *has* to be done through code for now - maybe some abomination of sending EVERYTHING to this
    //script and having each data readout be a public list instead could accomplish high flexibility?
    //this probably isn't needed unless design specify they want complete control over things


    //-----------------------------------------------------
    //generic declares
    GenericFunctions cGenericFunctions = new GenericFunctions(); //instantiate a GenericFunctions for use here

    //declarations w/ setter funcs, since this script will be sent information constantly
    #region getset declarations
    /*      template
        private string m_sPrivateVar;
        public string PublicVar { set { m_sPrivateVar = value; } }
        public Text oTextObject;
    */

    //private string m_sDebugReadout;                                   //DEPRECATED!
    //public string DebugReadout { set { m_sDebugReadout = value; } }   //DEPRECATED! build string in HUDController.Update();

    //--
    private int m_sWaveCounter;
    public int WaveCounter { set { m_sWaveCounter = value; } }
    //--
    private int m_sWaveTimerTicks;
    public int WaveTimerTicks { set { m_sWaveTimerTicks = value; } }
    //--
    private int m_sPhaseTimerTicks;
    public int PhaseTimerTicks { set { m_sPhaseTimerTicks = value; } }
    //--
    private GameLogic.StarstoneElement m_eWaveStarstoneElement;
    public GameLogic.StarstoneElement WaveStarstoneElement { set { m_eWaveStarstoneElement = value; } }
    //--
    private string m_sGameplayPhase;
    public string GameplayPhase { set { m_sGameplayPhase = value; } }
    //--


    #endregion getset declarations
    //-----------------------------------------------------

    //-----------------------------------------------------
    //text object references
    #region output text objs declarations   
    public Text oTextDebugReadout;
    public Text oTextWaveCounter;
    public Text oTextWaveTimer;
    public Text oTextStarstoneElement;
    #endregion output text objs declarations
    //-----------------------------------------------------


    void Start()
    {
    }

    void Update()
    {
        //write out values every frame

        //write debug readout
        oTextDebugReadout.text = (
            m_sGameplayPhase +
            "\nphase\t" + cGenericFunctions.ConvertTickstoSeconds(m_sPhaseTimerTicks).ToString() + "s\t(" + m_sPhaseTimerTicks.ToString() +
            "t)\nwave\t" + cGenericFunctions.ConvertTickstoSeconds(m_sWaveTimerTicks).ToString() + "s\t(" + m_sWaveTimerTicks.ToString() + "t)"
            );
        
        //write wave counter
        oTextWaveCounter.text = "Wave: " + (m_sWaveCounter + 1); //(0 based)
        //write wave timer
        oTextWaveTimer.text = "Time: " + Mathf.FloorToInt(cGenericFunctions.ConvertTickstoSeconds(m_sWaveTimerTicks)); //ticks --> seconds
        //write starstone element
        oTextStarstoneElement.text = m_eWaveStarstoneElement.ToString();
    }
}
