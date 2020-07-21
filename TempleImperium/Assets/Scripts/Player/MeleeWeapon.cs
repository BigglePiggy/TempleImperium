using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    #region Declarations
    [Header("Melee Configuration")]
    [Tooltip("Speed of movement")]
    public float m_speed;
    [Tooltip("Local start position of weapon")]
    public Vector3 m_startPosition;
    [Tooltip("Local end position of weapon")]
    public Vector3 m_endPosition;

    float m_startTime;      //Time that journey was started
    float m_journeyLength;  //Length of entire journey
    bool m_attacking;       //True when weapon is traveling from start to end
    #endregion


    //Called per frame
    private void Update()
    {
        WeaponMovement();
    }


    private void WeaponMovement()
    {
        if (m_attacking)
        {
            //If at endpoint
            if (transform.position == m_endPosition)
            {
                m_attacking = false;
            }

            //If travling to endpoint
            else 
            {
                float distCovered = (Time.time - m_startTime) * m_speed;
                float fractionOfJourney = distCovered / m_journeyLength;

                transform.localPosition = Vector3.Slerp(m_startPosition, m_endPosition, fractionOfJourney);
            }
        }
    }

    public void StartAttacking()
    {
        if (m_attacking == false)
        {
            //Starts new journey from start to end
            m_startTime = Time.time;
            transform.localPosition = m_startPosition;
            m_journeyLength = Vector3.Distance(m_startPosition, m_endPosition);
            m_attacking = true;
        }
    }
}
