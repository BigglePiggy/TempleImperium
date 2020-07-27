using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyProjectile : MonoBehaviour
{
    public float m_startLifetime;
    public float m_damage;

    float m_lifetime;
    bool m_isLethal;

    //Initalization
    void Start()
    {
        m_lifetime = m_startLifetime;
        m_isLethal = true;
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
        }
    }
}
