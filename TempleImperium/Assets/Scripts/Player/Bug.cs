using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : MonoBehaviour
{
    ////Declarations
    //Public
    public float m_initalForce;
    public Vector2 m_initalOffset;
    public float m_lifetime;
    public float m_stunTime;
    public float m_chaseForce;

    //Private
    private float m_lifetimeTimer;
    private float m_stunTimer;
    private bool m_attached;
    private bool m_chasing;
    private Transform m_targetEnemy;
    private Rigidbody m_bugRb;

    // Start is called before the first frame update
    void Start()
    {
        m_lifetimeTimer = m_lifetime;
        m_stunTimer = m_stunTime;
        m_attached = false;

        m_bugRb = GetComponent<Rigidbody>();
        m_bugRb.AddForce(transform.forward * m_initalForce);
        m_bugRb.AddRelativeForce(new Vector3(Random.Range(-m_initalOffset.x, m_initalOffset.x), Random.Range(-m_initalOffset.y / 2, m_initalOffset.y), 0));
    }

    // Update is called once per frame
    void Update()
    {
        

        if(m_chasing == false && m_attached == false) 
        {
            m_lifetimeTimer -= Time.deltaTime;

            if(m_lifetimeTimer <= 0) 
            { Destroy(gameObject); }
        }

        try
        {
            if (m_chasing && m_attached == false)
            {
                m_bugRb.AddForce((m_targetEnemy.position - transform.position).normalized * (m_chaseForce * Time.deltaTime * 100));
            }
        }
        catch (MissingReferenceException e)
        { Destroy(gameObject); }


        if (m_attached) 
        {
            m_stunTimer -= Time.deltaTime;

            if (m_stunTimer <= 0)
            { Destroy(gameObject); }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Enemy") && m_chasing == false) 
        {
            m_targetEnemy = other.transform;
            m_chasing = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(m_chasing && m_attached == false && collision.transform.CompareTag("Enemy")) 
        {
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

            if (attach)
            {
                m_attached = true;
                transform.parent = collision.transform;
                collision.transform.SendMessage("Stun", m_stunTime);
                Destroy(m_bugRb);
            }
        }
    }
}
