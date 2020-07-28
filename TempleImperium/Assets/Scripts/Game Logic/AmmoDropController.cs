using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase


public class AmmoDropController : MonoBehaviour
{
    //this class manages ammo drop chances and spawning


    public GameObject oAmmoBoxPrefab;
    [Tooltip("Ammo boxes will reward one magazine by default.")]
    public float m_fAmmoPerBoxMultiplierPrimary = 1.5f;
    public float m_fAmmoPerBoxMultiplierSecondary = 1;

    GameObject oPlayer; //player obj ref


    void Start()
    {
        oPlayer = GameObject.Find("Player");    //establish reference to Player
    }

    void Update()
    {
        
    }

    public void RollDropChance(Vector3 input_spawnPosition)
    {
        //get ammo counts via Player tuple(item1 current reserve, max)
        int m_iPrimaryCurrent;

    }
}
