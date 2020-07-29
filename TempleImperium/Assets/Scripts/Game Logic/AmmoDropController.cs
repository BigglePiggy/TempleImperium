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
    [Space]
    [Tooltip("drop chance calculted with:\n((gun reserve / reserve max)+flat increase)*multiplier")]
    public float m_fDropChanceMultiplier = 0.5f;
    public float m_fDropChanceFlatIncrease = 0.2f;
    [Space]
    [Tooltip("0-1. chance of which weapon's ammo should drop.\neg 0.33 with the Primary weapon equipped would give a 2/3rds chance of drops being Secondary ammo.")]
    public float m_fEquippedWeaponWeight = 0.33f;
    /*[Space(30f)]
    public*/ bool DisableBoundsWarnings = false;

    GameObject oPlayer;         //player obj ref
    (int, int) m_tiMagSizes;    //gun max mag sizes (for calc ammo box contents, * multiplier)


    void Start()
    {
        oPlayer = GameObject.Find("Player");    //establish reference to Player


        //reasonable bounds checks
        if (DisableBoundsWarnings) { return; }  //drop from Start() if not doing bounds checks

        if(m_fDropChanceMultiplier < 0.25f || m_fDropChanceMultiplier > 0.9)
        { Debug.LogWarning("AmmoDropController drop chance multiplier unreasonable value of '" + m_fDropChanceMultiplier + "'. are you sure?"); }

        if (m_fDropChanceFlatIncrease > 0.5f)
        { Debug.LogWarning("AmmoDropController drop chance flat increase unreasonable value of '" + m_fDropChanceFlatIncrease + "'. are you sure?"); }
    }

    void Update()
    {
    }

    public void RollDropChanceAtPosition(Vector3 input_spawnPosition)
    {

        //get ammo counts via Player tuple(item1 current reserve, item2 max reserve)

        (int, int) m_tiPrimary = oPlayer.GetComponent<PlayerController>().GetPrimaryGunAmmo();  //get primary

        float m_fPrimaryChance = m_tiPrimary.Item1 / (float)m_tiPrimary.Item2; //calc 0-1 % filled. float coercion bc int/int=int

        (int, int) m_tiSecondary = oPlayer.GetComponent<PlayerController>().GetSecondaryGunAmmo();  //get secondary

        float m_fSecondaryChance = m_tiSecondary.Item1 / (float)m_tiSecondary.Item2; //calc 0-1 % filled. float coercion bc int/int=int


        //do chance roll (primary)
        float m_fRoll = Random.Range(0, 1);     //roll
        bool m_bPrimaryRollPassed = false;      //declare bool
        if(m_fPrimaryChance > m_fRoll) { m_bPrimaryRollPassed = true; }  //check if passed

        //do chance roll (secondary)
        m_fRoll = Random.Range(0, 1);           //roll (reuse var)
        bool m_bSecondaryRollPassed = false;    //declare bool
        if(m_fSecondaryChance > m_fRoll) { m_bSecondaryRollPassed = true; }  //check if passed


        //states
        if (m_bPrimaryRollPassed)
        {
            if (m_bSecondaryRollPassed)
            {
                //if BOTH rolls passed

                m_fRoll = Random.Range(0, m_fPrimaryChance + m_fSecondaryChance);   //do new roll between chances

                if (m_fRoll <= m_fPrimaryChance)    //check against primary chance (lower side of chance)
                {
                    //if <= primary chance (within lower side of chance), spawn primary ammo
                    DropAtPosition(input_spawnPosition, true, false);
                }
                else
                {
                    //if > primary chance (within upper side of chance), spawn secondary ammo
                    DropAtPosition(input_spawnPosition, false, true);
                }
            }
            else
            {
                //if ONLY PRIMARY roll passed, spawn primary ammo
                DropAtPosition(input_spawnPosition, true, false);
            }
        }
        else
        {
            if (m_bSecondaryRollPassed)
            {
                //if ONLY SECONDARY roll passed, spawn secondary ammo
                DropAtPosition(input_spawnPosition, false, true);
            }
            else
            {
                //if NO ROLLS passed

                //do nothing
            }
        }
    }

    /// <summary>
    /// spawn ammo box at position
    /// </summary>
    /// <param name="input_spawnPosition"></param>
    /// <param name="input_hasPrimaryAmmo"></param>
    /// <param name="input_hasSecondaryAmmo"></param>
    public void DropAtPosition(Vector3 input_spawnPosition, bool input_hasPrimaryAmmo, bool input_hasSecondaryAmmo)
    {
        GameObject oNewAmmoBox = Instantiate(oAmmoBoxPrefab);

        int m_iNewAmmoBoxPrimary = 0;
        int m_iNewAmmoBoxSecondary = 0;

        if (input_hasPrimaryAmmo) { m_iNewAmmoBoxPrimary = }
    }
}
