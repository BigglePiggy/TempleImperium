using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//written by Eddie

public class Grenade : MonoBehaviour
{
    ////Declarations
    //Public
    public float m_initalForce;
    public float m_explosionTimer;
    public float m_explosionPower;
    public float m_explosionRadius;

    //Private
    private float m_currentTimer;
    private bool m_timerRunning;
    private Rigidbody m_grenadeRb;

    //Initalization
    void Start()
    {
        m_currentTimer = 0;
        m_timerRunning = false;
        m_grenadeRb = GetComponent<Rigidbody>();
        m_grenadeRb.AddForce(transform.forward * m_initalForce, ForceMode.Force);
    }

    //Called per frame
    void Update()
    {
        if (m_timerRunning) 
        {
            m_currentTimer += Time.deltaTime;

            if(m_currentTimer >= m_explosionTimer) 
            { _explode(); }
        }
    }

    //Explode
    public void _explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_explosionRadius);

        foreach (Collider closeObject in colliders)
        {
            if (closeObject.transform.root.CompareTag("Enemy")) 
            {
                Rigidbody rb = closeObject.GetComponent<Rigidbody>();

                if (rb != null) 
                {closeObject.GetComponent<Rigidbody>().AddExplosionForce(m_explosionPower, transform.position, m_explosionRadius);}
            }
        }

        Destroy(this.gameObject);
    }

    ////Collision detection
    private void OnCollisionEnter(Collision collision)
    {
        if (m_timerRunning == false)
        {
            if (collision.collider.transform.root.CompareTag("Enemy"))
            { _explode(); }

            if (collision.collider.transform.root.CompareTag("Player") == false)
            { m_timerRunning = true; }
        }   
    }
}
