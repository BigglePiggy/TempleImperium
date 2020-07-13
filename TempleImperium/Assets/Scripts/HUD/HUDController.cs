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
    private int m_iWaveCounter;
    public int WaveCounter { set { m_iWaveCounter = value; } }
    //--
    private int m_iWaveTimerTicks;
    public int WaveTimerTicks { set { m_iWaveTimerTicks = value; } }
    //--
    private int m_iPhaseTimerTicks;
    public int PhaseTimerTicks { set { m_iPhaseTimerTicks = value; } }
    //--
    private GameLogic.StarstoneElement m_eWaveStarstoneElement;
    public GameLogic.StarstoneElement WaveStarstoneElement { set { m_eWaveStarstoneElement = value; } }
    //--
    private GameLogic.GameplayPhase m_eGameplayPhase;
    public GameLogic.GameplayPhase GameplayPhase { set { m_eGameplayPhase = value; } }
    //--

    #endregion getset declarations

    //-----------------------------------------------------
    //misc public vars
    public bool m_bShowDebug = false;

    //-----------------------------------------------------
    //text object references (and their private output strings)
    #region output text obj refs declarations   
    [Header("HUD Elements")]
    public Text oTextDebugReadout;
    string m_sTextDebugReadout = "";
    public Text oTextWaveCounter;
    string m_sTextWaveCounter = "";
    public Text oTextWaveTimer;
    string m_sTextWaveTimer = "";
    public Text oTextStarstoneElement;
    string m_sTextStarstoneElement = "";
    #endregion output text obj refs declarations
    //-----------------------------------------------------


    void Start()
    {
    }

    void Update()
    {
        //write out values every frame

        //debug readout
        if (m_bShowDebug)   //(if configured to)
        {
            m_sTextDebugReadout = (
                m_eGameplayPhase +
                "\nphase\t" + cGenericFunctions.ConvertTickstoSeconds(m_iPhaseTimerTicks).ToString() + "s\t(" + m_iPhaseTimerTicks.ToString() +
                "t)\nwave\t" + cGenericFunctions.ConvertTickstoSeconds(m_iWaveTimerTicks).ToString() + "s\t(" + m_iWaveTimerTicks.ToString() + "t)"
                );
        }
        else { m_sTextDebugReadout = ""; }

        //timer
        switch (m_eGameplayPhase)
        {
            //write WAVE ticker
            case GameLogic.GameplayPhase.Subwave:
            case GameLogic.GameplayPhase.InbetweenSubwave:
                m_sTextWaveTimer = "Time: \n" + Mathf.FloorToInt(cGenericFunctions.ConvertTickstoSeconds(m_iWaveTimerTicks)); //ticks --> seconds
                break;

            //write PHASE ticker
            case GameLogic.GameplayPhase.InbetweenWave:
            case GameLogic.GameplayPhase.PreGame:
                m_sTextWaveTimer = "Prepare... \n" + Mathf.FloorToInt(cGenericFunctions.ConvertTickstoSeconds(m_iPhaseTimerTicks)); //ticks --> seconds
                break;

            //write BLANK
            default:
                m_sTextWaveTimer = "";
                break;
        }


        //wave counter
        m_sTextWaveCounter = "Wave " + (m_iWaveCounter + 1); //(0 based)

        //starstone element
        m_sTextStarstoneElement = m_eWaveStarstoneElement.ToString();

        //refresh HUD
        Write();
    }

    //write to HUD objects
    void Write()
    {
        oTextDebugReadout.text      = m_sTextDebugReadout;
        oTextWaveCounter.text       = m_sTextWaveCounter;
        oTextWaveTimer.text         = m_sTextWaveTimer;
        oTextStarstoneElement.text  = m_sTextStarstoneElement;
        
    }
}
