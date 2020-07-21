using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    //Aim Down Sight
    //    if (m_left || m_right)
    //    {
    //        float distCovered = (Time.time - m_startTime) * m_adsSpeed;
    //        float fractionOfJourney = distCovered / m_journeyLength;

    //        if (m_left)
    //        { transform.localPosition = Vector3.Lerp(m_gunPosition, m_adsPos, fractionOfJourney); }

    //        if (m_right)
    //        { transform.localPosition = Vector3.Lerp(m_gunPosition, m_hipPos, fractionOfJourney); }
    //    }

    //    //Down - Go left
    //    if (Input.GetKeyDown(GlobalValues.g_settings.m_kcKeyAltFire))
    //    {
    //        m_startTime = Time.time;
    //        m_gunPosition = transform.localPosition;
    //        m_journeyLength = Vector3.Distance(m_gunPosition, m_adsPos);
    //        m_left = true;
    //        m_right = false;
    //    }
    //}

    //private void WeaponMovement() 
    //{
    //    if (m_attacking)
    //    {
    //        float distCovered = (Time.time - m_startTime) * m_speed;
    //        float fractionOfJourney = distCovered / m_journeyLength;

    //        transform.localPosition = Vector3.Slerp(m_startPosition, m_endPosition, fractionOfJourney);
    //    }
    //}

    //public void StartAttacking() 
    //{
    //    if(m_attacking == false) 
    //    {
    //        m_startTime = Time.time;
    //        m_startPosition = transform.localPosition;
    //        m_journeyLength = Vector3.Distance(m_startPosition, m_endPosition);
    //        m_attacking = true;
    //    }
    //}
}
