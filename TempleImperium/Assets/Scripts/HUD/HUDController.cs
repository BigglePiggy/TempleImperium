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
    //this probably isn't needed unless design/techart specify they want complete control over things, though


    //declarations w/ setter funcs, since this script will be sent information constantly
    #region declarations
    /*      template
        private string m_sPrivateVar;
        public string PublicVar { set { m_sPrivateVar = value; } }
        public Text oTextObject;
    */

    #region debug readout
    private string m_sDebugReadout;
    public string DebugReadout { set { m_sDebugReadout = value; } }
    public Text oDebugReadout;
    #endregion //debug readout end

    #endregion //declarations end

    void Start()
    {
    }

    void Update()
    {
        oDebugReadout.text = m_sDebugReadout;       //write debug readout
    }
}
