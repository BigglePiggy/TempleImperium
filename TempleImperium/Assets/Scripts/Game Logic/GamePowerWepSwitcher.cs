using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase
//all **ten** lines of it. yeah baby


public class GamePowerWepSwitcher : MonoBehaviour
{
    //player can go up to objects with these to change the element of their power weapon

    public GameLogic.StarstoneElement m_eStarstoneElement;
    public float m_fRadius = 5;


    public float GetRadius()
    {
        return m_fRadius;
    }
    public GameLogic.StarstoneElement GetElement()
    {
        return m_eStarstoneElement;
    }

    //visualise radius via editor gizmos
    void OnDrawGizmosSelected()
    {
            Gizmos.DrawWireSphere(gameObject.transform.position, m_fRadius);
    }
}
