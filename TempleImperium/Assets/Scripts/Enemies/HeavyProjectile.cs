using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//written by Eddie

public class HeavyProjectile : MonoBehaviour
{
    [Header("Projectile Configuration")]
    [Tooltip("Amount of time the projectile exists for")]
    public float m_startLifetime;
    [Tooltip("Damage applied to the player when hit by this projectile")]
    public float m_damage;
    [Tooltip("Amount of force applied to pushback the player when under the Arc starstone")]
    public float m_powerPushback;

    float m_lifetime;           //Lifetime tracker
    bool m_isLethal;            //Determines if damage should be applied on collison
    Rigidbody m_projectileRb;   //Rigidbody reference

    GameLogic.StarstoneElement m_starstone;     //The startsone of the enemy that created this projectile

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
            //Applied damage to player and revoes lethality
            m_isLethal = false;
            collision.transform.GetComponent<PlayerController>().TakeDamage(m_damage);

            //Applies extra force to the player if created by an enemy under the arc startsone
            if (m_starstone == GameLogic.StarstoneElement.Arc)
            {
                collision.transform.GetComponent<PlayerController>().ReduceDrag();
                collision.transform.GetComponent<Rigidbody>().AddForce(new Vector3(m_projectileRb.velocity.x, 0, m_projectileRb.velocity.z).normalized * m_powerPushback, ForceMode.Acceleration);
            }

            Destroy(gameObject);
        }
    }
}
