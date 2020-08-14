using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : MonoBehaviour
{
    //Player controller script - there should be only one player object with an instance of me!
    //What this script does:
    /*
        - Handles keyboard and mouse inputs
        - Handles bespoke player physics
        - Manages weapon switching
        - Manages abilities
    */

    #region Declarations
    [Header("Bug Configuration")]
    [Tooltip("Base force applied when created")]
    public float m_initalForce;
    [Tooltip("Range of force that can be added when created (randomized)")]
    public Vector2 m_initalOffset;
    [Tooltip("Amount of time before the bug is destoryed (if it does not stun an enemy)")]
    public float m_lifetime;
    [Tooltip("Amount of time an enemy will be stunned for")]
    public float m_stunTime;
    [Tooltip("Force applied to the bug to chase after it's target")]
    public float m_chaseForce;

    float m_lifetimeTimer;  //Tracks the alive time
    float m_stunTimer;      //Tracks how long the stun has lasted
    bool m_attached;        //If true then the bug is attached to a target
    bool m_chasing;         //If true then the bug is chasing to a target
    Transform m_targetEnemy;    //Target transform reference
    Rigidbody m_bugRb;  //Rigidbody reference
    #endregion

    //Initalization
    void Start()
    {
        m_lifetimeTimer = m_lifetime;
        m_stunTimer = m_stunTime;
        m_attached = false;

        m_bugRb = GetComponent<Rigidbody>();
        m_bugRb.AddForce(transform.forward * m_initalForce);
        m_bugRb.AddRelativeForce(new Vector3(Random.Range(-m_initalOffset.x, m_initalOffset.x), Random.Range(-m_initalOffset.y / 2, m_initalOffset.y), 0));
    }

    //Called per frame
    void Update()
    {
        //Traks alive time when not attached or stunning
        if(m_chasing == false && m_attached == false) 
        {
            m_lifetimeTimer -= Time.deltaTime;

            //Is destroyed when lifetime expires
            if(m_lifetimeTimer <= 0) 
            { Destroy(gameObject); }
        }

        //Applied a force to the bug that causes it to chase its target 
        try
        {
            if (m_chasing && m_attached == false)
            { m_bugRb.AddForce((m_targetEnemy.position - transform.position).normalized * (m_chaseForce * Time.deltaTime * 100), ForceMode.Acceleration); }
        }
        catch (MissingReferenceException) //Is destoryed if the enemy is it chasing is killed
        { Destroy(gameObject); }

        //Tracks the stun time
        if (m_attached) 
        {
            m_stunTimer -= Time.deltaTime;

            //Is destoryed when it has finished stunning the enemy
            if (m_stunTimer <= 0)
            { Destroy(gameObject); }
        }
    }

    //Target enemy detection - trigger collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Enemy") && m_chasing == false) 
        {
            m_targetEnemy = other.transform;
            m_chasing = true;
        }
    }

    //Target stun applciation - non-trigger collider
    private void OnCollisionEnter(Collision collision)
    {
        if(m_chasing && m_attached == false && collision.transform.CompareTag("Enemy")) 
        {
            //Is true when an enemy that is not currently stunned has passed into the trigger
            bool attach = false;

            if (collision.transform.GetComponent<LightEnemyController>() != null)
            {
                if (collision.transform.GetComponent<LightEnemyController>().GetStunnedTimer() <= 0)
                { attach = true; }
            }

            if (collision.transform.GetComponent<MediumEnemyController>() != null)
            {
                if (collision.transform.GetComponent<MediumEnemyController>().GetStunnedTimer() <= 0)
                { attach = true; }
            }

            if (collision.transform.GetComponent<HeavyEnemyController>() != null)
            { 
                if (collision.transform.GetComponent<HeavyEnemyController>().GetStunnedTimer() <= 0)
                { attach = true; }  
            }

            //If a target enemy is found 
            if (attach)
            {
                m_attached = true;
                transform.parent = collision.transform;
                collision.transform.SendMessage("Stun", m_stunTime);
                Destroy(m_bugRb);

                SphereCollider[] sphereColliders = GetComponents<SphereCollider>();
                for (int i = 0; i < sphereColliders.Length; i++)
                { Destroy(sphereColliders[i]); }
                //Attachs to the enemy and waits to be destoryed
            }

            else //if target enemy is stunned allready then bug is destoryed
            { m_chasing = false; }
        }
    }
}
