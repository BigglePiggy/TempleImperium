using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//written by Eddie

public class MeleeWeapon : MonoBehaviour
{
    //Melee weapon script
    //What this script does:
    /*
        - Animates a melee weapon
        - Damages enemies it hits
    */

    #region Declarations
    [Header("Melee Configuration")]
    [Tooltip("Speed of movement")]
    public float m_speed;
    [Tooltip("Damage applied on hit")]
    public float m_damage;
    [Tooltip("Local start position of weapon")]
    public Vector3 m_startPosition;
    [Tooltip("Local end position of weapon")]
    public Vector3 m_endPosition;

    float m_startTime;      //Time that journey was started
    float m_journeyLength;  //Length of entire journey
    bool m_attacking;       //True when weapon is traveling from start to end
    bool m_lethal;          //True when weapon is traveling from start to end

    MeshRenderer m_meshRenderer;
    #endregion

    //Initalization
    private void Start()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshRenderer.enabled = false;
    }

    //Called per frame
    private void Update()
    {
        if (m_attacking)
        {
            //If at endpoint
            if (transform.localPosition == m_endPosition)
            {
                m_attacking = false;
                m_meshRenderer.enabled = false;
            }

            //If travling to endpoint
            else 
            {
                float distCovered = (Time.time - m_startTime) * m_speed;
                float fractionOfJourney = distCovered / m_journeyLength;

                transform.localPosition = Vector3.Slerp(m_startPosition, m_endPosition, fractionOfJourney);
                //transform.localPosition = Vector3.Lerp(m_startPosition, m_endPosition, fractionOfJourney);
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
            m_meshRenderer.enabled = true;
            m_lethal = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_lethal) 
        {
            if (other.transform.CompareTag("Enemy"))
            {
                m_lethal = false;
                other.transform.SendMessage("TakeDamage", m_damage);
            }

            else if (other.transform.root.CompareTag("Enemy"))
            {
                m_lethal = false;
                other.transform.root.SendMessage("TakeDamage", m_damage);
            }
        }
    }
}