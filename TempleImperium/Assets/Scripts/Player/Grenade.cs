using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    ////Declarations
    //Public
    public float initalForce;
    public float explosionTimer;
    public float explosionPower;
    public float explosionRadius;

    //Private
    private float currentTimer;
    private bool timerRunning;
    private Rigidbody grenadeRb;

    //Initalization
    void Start()
    {
        currentTimer = 0;
        timerRunning = false;
        grenadeRb = GetComponent<Rigidbody>();
        grenadeRb.AddForce(transform.forward * initalForce, ForceMode.Force);
    }

    //Called per frame
    void Update()
    {
        if (timerRunning) 
        {
            currentTimer += Time.deltaTime;

            if(currentTimer >= explosionTimer) 
            { _explode(); }
        }
    }

    //Explode
    public void _explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider closeObject in colliders)
        {
            if (closeObject.transform.root.CompareTag("Enemy")) 
            {
                Rigidbody rb = closeObject.GetComponent<Rigidbody>();

                if (rb != null) 
                {closeObject.GetComponent<Rigidbody>().AddExplosionForce(explosionPower, transform.position, explosionRadius);}
            }
        }

        Destroy(this.gameObject);
    }

    ////Collision detection
    private void OnCollisionEnter(Collision collision)
    {
        if (timerRunning == false)
        {
            if (collision.collider.transform.root.CompareTag("Enemy"))
            { _explode(); }

            if (collision.collider.transform.root.CompareTag("Player") == false)
            { timerRunning = true; }
        }   
    }
}
