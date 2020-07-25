using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//written by Ase



public class HUDFader : MonoBehaviour
{
    //this script makes an object - ideally a UI element - adjust its opacity linearly between fully opaque and transparent.

    //public declares
    [Tooltip("Fader panel colour/tint")]
    public Color m_cColour = Color.black;
    [Tooltip("Amount to in/decrement alpha each frame")]
    public int m_iStep = 5;
    [Tooltip("Upper bound for stepping colour to/from. Lower is 0")]
    public int m_iStepUpperBound = 300;

    //private declares
    int m_iCurrentAlpha = 0;
    int m_iTargetAlpha = 0;


    void Start()
    {

    }

    void Update()
    {
        if (m_iCurrentAlpha != m_iTargetAlpha)  //if not at target..
        {
            if(m_iCurrentAlpha > m_iTargetAlpha) { m_iCurrentAlpha -= m_iStep; }    //if greater, step down
            else { m_iCurrentAlpha += m_iStep; }                                    //else lesser, step up

            m_iCurrentAlpha = Mathf.Min(m_iCurrentAlpha, m_iStepUpperBound);  //bounds check
            m_iCurrentAlpha = Mathf.Max(m_iCurrentAlpha, 0);

            m_cColour.a = (m_iCurrentAlpha / m_iStepUpperBound);    //update local colour (unityEngine.color takes 0-1f)
            gameObject.GetComponent<Image>().color = m_cColour;     //write to gameObject (individual channels are readonly for some reason)
        }
    }

    #region inputs

    public void FadeIn(bool instant)
    {
        m_iTargetAlpha = m_iStepUpperBound;
        if (instant) { m_iCurrentAlpha = m_iTargetAlpha; }
    }
    public void FadeOut(bool instant)
    {
        m_iTargetAlpha = 0;
        if (instant) { m_iCurrentAlpha = m_iTargetAlpha; }
    }

    public void ConfigureColour(Color input_colour)
    {
        m_cColour = input_colour;
    }
    public void ConfigureStep(int input_step)
    {
        m_iStep = input_step;
    }

    #endregion inputs
}
