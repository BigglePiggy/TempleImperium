using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//written by Eddie

public class JumpPad : MonoBehaviour
{
    public float jumpForce;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player")) 
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(0, jumpForce, 0);       
        }
    }
}
