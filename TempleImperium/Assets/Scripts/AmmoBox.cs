using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase

public class AmmoBox : MonoBehaviour
{
    //GameObject that stores ammo for the player to pick up. not too interesting.

    public int m_iPrimary;      //primary ammo in this box
    public int m_iSecondary;    //secondary ammo in this box

    bool m_bPendingDestroy;     //destroy next Update()?

    void Update()
    {
        if (m_bPendingDestroy) { Destroy(gameObject); }
    }

    public void SetAmmo(int input_primary, int input_secondary)
    {
        m_iPrimary = input_primary;
        m_iSecondary = input_secondary;
    }

    /// <summary>
    /// retrieve ammo count from this Box
    /// </summary>
    /// <returns>Tuple(Item1 int Primary, Item2 int Secondary)</returns>
    public (int, int) GetAmmo(bool input_destroyBox = true)
    {
        m_bPendingDestroy = input_destroyBox;   //kill on next tick if told to

        return (m_iPrimary, m_iSecondary);      //return ammo counts
    }
}
