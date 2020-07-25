using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeWeapon : MonoBehaviour
{
    public float m_chargeUpTime;

    float m_chargeUpProgress;
    bool m_isHeld;

    MeshRenderer m_meshRenderer;            //Mesh renderer component - used to make gun invisible when not in use


    //Initalization
    public void Initalization()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();

        m_isHeld = false;
        m_meshRenderer.enabled = false;
    }

    //Called per frame
    void Update()
    {
        if (m_isHeld) 
        {
            Inputs();
        }
    }


    private void Inputs()
    {
        //Charge to fire
        if (Input.GetKey(GlobalValues.g_settings.m_kcKeyFire))
        {
            if (m_chargeUpProgress < m_chargeUpTime)
            { m_chargeUpProgress += Time.deltaTime; }

        }
        else if (m_chargeUpProgress > 0)
        {
            m_chargeUpProgress -= Time.deltaTime;
        }
    }


    #region Is Held State Management
    public void StartHolding()
    {
        m_isHeld = true;
        m_meshRenderer.enabled = true;
    }

    public void StopHolding()
    {
        m_isHeld = false;
        m_meshRenderer.enabled = false;
    }

    public bool GetIsHeld()
    {
        return m_isHeld;
    }
    #endregion
}
