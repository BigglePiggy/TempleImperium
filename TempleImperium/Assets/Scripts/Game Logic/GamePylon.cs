using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase


public class GamePylon : MonoBehaviour
{
    //pylon object. controlled from GameLogic
    //this script should be attached to the base!

    [Header("add a child GameObject called Core, starting in the raised position.")]  
    [Tooltip("vertical offset of destination (Core will move its own height by default)")]
    public int m_iCoreDestinationYOffset = -5;
    [Tooltip("is the Core raised (up/disengaged) or lowered (down/engaged)?")]
    public bool m_bIsRaised = false;
    [Tooltip("speed of Core movement - every tick, move the distance between current and target position divided by this value." +
        "\neg. current pos = y2, target pos = y10. the distance between these is 8. assuming this value = 2, the Core will move by 4 (to y6) this tick." +
        "\nthe next tick, it will move by 2 (10-6=4 -> 4/2 = 2) to y8. then y9, then y9.5, etc.")]
    public int m_iCoreMoveEaseDivider = 2;
    [Tooltip("this is the point where we snap to the target position." +
        "\nfollowing from the previous parameter's tooltip, if this value is 0.5, instead of continuing to ease the movement after y9.5, the current position is set to the end position instead.")]
    public float m_fMoveSnapThreshold = 0.5f;    //at what delta do we snap to the target position?

    bool m_bIsActuating = false;        //currently moving between raised/lowered?

    GameObject oCore;               //"core" part of pylon to move up/down
    Vector3 m_vCorePosRaised;       //core start pos
    Vector3 m_vCorePosLowered;      //core end pos
    float m_fCoreHeight;            //core height

    void Start()
    {
        oCore = gameObject.GetComponent("Core").gameObject;     //establish reference to Core child object
        m_vCorePosRaised = oCore.transform.position;            //retrieve start position
        m_fCoreHeight = oCore.transform.lossyScale.y;           //retrieve core (bbox) height

        //calculate end position
        //end pos = orig pos + (down * height) + (down * offset)
        m_vCorePosLowered = oCore.transform.position + (Vector3.down * m_fCoreHeight) + (Vector3.down*m_iCoreDestinationYOffset); 
    }

    void FixedUpdate()
    {
        if (m_bIsActuating) //if actuating
        {
            //assume we're moving down for now.
            //calc distance between current position and raised position; our MoveDelta is currently the upper portion of
            //the whole Core actuation distance.
            float m_fMoveDelta = m_vCorePosLowered.y - oCore.transform.position.y;

            if (m_bIsRaised) //if IsRaised=true, we're currently moving toward the raised position and should invert our delta
            {
                //height - delta = the other side of the delta
                //eg height 2 - delta 0.5 = 1.5
                m_fMoveDelta = m_fCoreHeight - m_fMoveDelta;
            }

            //affect by easing divider
            m_fMoveDelta /= m_iCoreMoveEaseDivider;


            if (!m_bIsRaised)   //if moving toward lowered position, *-1 delta so we actually move the pylon down
            {
                m_fMoveDelta *= -1;
            }

            //write to position
            Vector3 m_vOutPos = oCore.transform.position;   //get pos
            m_vOutPos.y = m_fMoveDelta;                     //write y
            oCore.transform.position = m_vOutPos;           //overwrite pos

            //threshold check (prevent wasting cycles easing to some stupid tiny float value forever)
            //lowering
            if(m_vOutPos.y < (m_vCorePosLowered.y + m_fMoveSnapThreshold))  //if below (low bound + threshold)
            {
                oCore.transform.position = m_vCorePosLowered;   //overwrite pos
                m_bIsActuating = false;                         //stop moving
            }
            //raising
            if(m_vOutPos.y > (m_vCorePosRaised.y - m_fMoveSnapThreshold))   //if above (high bound - threshold)
            {
                oCore.transform.position = m_vCorePosRaised;    //overwrite pos
                m_bIsActuating = false;                         //stop moving
            }
        }
    }


    /// <summary>
    /// get the pylon's state
    /// </summary>
    /// <returns>returns bIsRaised, bIsActuating</returns>
    public (bool,bool) GetState()
    {
        return (m_bIsRaised, m_bIsActuating);
    }

    #region inputs
    /// <summary>
    /// invert the pylon's state - if it's raised, it'll lower, and vice versa.
    /// </summary>
    public void Toggle()
    {
        m_bIsRaised ^= true;    //flip bool (^ is bool operator XOR - this like is doing an XOR with m_bIsRaised and a true literal.)
        m_bIsActuating = true;  //tell pylon to start moving
    }
    /// <summary>
    /// raise the pylon out of the ground.
    /// </summary>
    public void Raise()
    {
        if (!m_bIsRaised)   //if not already raised,
        {
            m_bIsRaised = true;         //raise
            m_bIsActuating = true;      //tell to move
        }
    }
    /// <summary>
    /// lower the pylon into the ground.
    /// </summary>
    public void Lower()
    {
        if (m_bIsRaised)    //if raised,
        {
            m_bIsRaised = false;        //lower
            m_bIsActuating = true;      //tell to move
        }

    }
    /// <summary>
    /// set the pylon's state with a boolean input.
    /// </summary>
    /// <param name="input_isRaised"></param>
    public void SetRaised(bool input_isRaised)
    {
        if(!m_bIsRaised == input_isRaised)  //if input doesn't match...
        {
            m_bIsRaised = input_isRaised;   //write to state
            m_bIsActuating = true;          //tell to move
        }

    }
    #endregion
}
