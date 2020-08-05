using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//written by Eddie

public class HeavyProjectile : MonoBehaviour
{
    public float m_startLifetime;
    public float m_damage;
    public float m_powerPushback;

    float m_lifetime;
    bool m_isLethal;
    Rigidbody m_projectileRb;

    GameLogic.StarstoneElement m_starstone;

    //Initalization
    public void Initalization(GameLogic.StarstoneElement input_starstone)
    {
        m_starstone = input_starstone;
        m_lifetime = m_startLifetime;
        m_isLethal = true;
        m_projectileRb = GetComponent<Rigidbody>();
    }

    //Called per frame
    void Update()
    {
        m_lifetime -= Time.deltaTime;

        if(m_lifetime < 0) 
        { Destroy(gameObject); }
    }

    //Collision detection
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && m_isLethal)
        {
            m_isLethal = false;
            collision.transform.GetComponent<PlayerController>().TakeDamage(m_damage);

            if (m_starstone == GameLogic.StarstoneElement.Arc)
            {
                collision.transform.GetComponent<PlayerController>().ReduceDrag();
                collision.transform.GetComponent<Rigidbody>().AddForce(new Vector3(m_projectileRb.velocity.x, 0, m_projectileRb.velocity.z).normalized * m_powerPushback, ForceMode.Acceleration);
            }

            Destroy(gameObject);
        }
    }
}
