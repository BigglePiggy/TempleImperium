using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by Eddie

public class PlayerGun : MonoBehaviour
{
    //Player Gun script 
    //What this script does:
    /*
        - Handles player shooting inputs
        - Handles ADS functionality
        - Passes ammo infomation to the HUD
        - Passes recoil infomation to Player Controller
        - Manages held state
    */

    #region Declarations
    [Header("Gun Configuration")]
    [Tooltip("Toggles between the gun being automatic or semi-automatic")]
    public bool m_automatic;
    [Tooltip("Minimum time between shots in seconds")]
    public float m_fireRate;
    [Tooltip("Maximum amount of ammo that can be stored for this gun")]
    public int m_maxAmmoCount;
    [Tooltip("Amount of ammo this gun contains on start")]
    public int m_startingAmmoCount;
    [Tooltip("Number of rounds contained withtin one magazine")]
    public int m_maxMagCapacity;
    [Tooltip("Time it takes to reload in seconds")]
    public float m_reloadTime;
    [Tooltip("Maximum range of raycast shots")]
    public float m_bulletRange;
    [Tooltip("Damage applied by bullets that hit the player")]
    public float m_shotDamage;
    [Space]

    [Tooltip("Speed at which the gun moves between hip-fire and ADS")]
    public float m_adsSpeed;
    [Tooltip("Gun posiiton at hip")]
    public Vector3 m_hipPos;
    [Tooltip("Gun posiiton when aiming down sight")]
    public Vector3 m_adsPos;
    [Space]

    [Header("Recoil")]
    [Tooltip("Amount of recoil applied per shot")]
    public float m_recoil;
    [Tooltip("Rate at which the recoil reduces over time")]
    public float m_recoilDampening;
    [Tooltip("Amount of weight the player's mouse inputs have over the recoil")]
    public float m_recoilControl;
    [Space]

    [Header("Sound Effects")]
    [Tooltip("Effect played when a shot is fired")]
    public AudioClip shotEffect;
    [Tooltip("Effect played reloading")]
    public AudioClip reloadEffect;


    //ADS variables
    bool m_left;                //True when gun moving towards ADS position 
    bool m_right;               //True when gun moving back to hip position 
    float m_startTime;          //Stores a Lerp journey's start time
    float m_journeyLength;      //Stores the length of the current proposed journey
    Vector3 m_gunPosition;      //Lerp gun position

    bool m_isHeld;              //True when gun is selected by player & active   
    int m_ammoCount;            //Total ammo for this gun
    int m_currentMagCapacity;   //Ammo in the current magazine
    float m_timeSinceLastShot;  //Time since last shot in seconds
    float m_reloadProgress;     //Tracks reload progress 

    PlayerController m_playerController;    //Reference to Player Controller script - Recoil values are passed 
    HUDController m_hudController;          //Reference to Hud Controller script - Display values are passed

    AudioSource m_audioOrigin;              //Audio origin component
    MeshRenderer m_meshRenderer;            //Mesh renderer component - used to make gun invisible when not in use
    Transform m_bulletOrigin;               //Reference to editor positioned bullet origin
    ParticleSystem m_bulletParticleSystem;  //Used to emit when gun is shot
    #endregion


    //Initalization
    public void Initalization()
    {
        m_left = false;
        m_right = false;

        m_isHeld = false;

        m_ammoCount = m_startingAmmoCount;
        m_currentMagCapacity = m_maxMagCapacity;
        m_timeSinceLastShot = m_fireRate;
        m_reloadProgress = m_reloadTime;

        m_playerController = transform.root.GetComponent<PlayerController>();
        m_hudController = GameObject.Find("HUD").GetComponent<HUDController>();

        m_bulletOrigin = transform.Find("Bullet Origin");
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_audioOrigin = GetComponent<AudioSource>();
        m_bulletParticleSystem = m_bulletOrigin.GetComponent<ParticleSystem>(); 
    }

    //Called per frame
    void Update()
    {
        if (m_isHeld && Time.timeScale != 0)
        {
            Inputs();
            HudValues();
            WeaponPointingAt();
        }
    }


    #region Whilst held
    private void Inputs()
    {
        ////Shooting
        //Since last shot update
        if (m_timeSinceLastShot < m_fireRate)
        { m_timeSinceLastShot += Time.deltaTime; }

        //Reload update
        if (m_reloadProgress < m_reloadTime)
        { m_reloadProgress += Time.deltaTime; }

        //Stops shooting on an empty magazines or during a reload
        if (m_currentMagCapacity != 0 && m_reloadProgress >= m_reloadTime)
        {
            bool shoot = false;

            if (m_automatic && Input.GetKey(GlobalValues.g_settings.m_kcKeyFire) && m_timeSinceLastShot >= m_fireRate) 
            { shoot = true; }

            if (m_automatic == false && Input.GetKeyDown(GlobalValues.g_settings.m_kcKeyFire) && m_timeSinceLastShot >= m_fireRate) 
            { shoot = true; }

            if (shoot) 
            {
                m_currentMagCapacity--;
                m_timeSinceLastShot = 0;
                m_audioOrigin.PlayOneShot(shotEffect);

                RaycastHit hit;
                if (Physics.Raycast(m_bulletOrigin.position, m_bulletOrigin.forward, out hit, m_bulletRange))
                {
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        hit.transform.gameObject.SendMessage("TakeDamage", m_shotDamage);
                    }
                    else if (hit.transform.root.CompareTag("Enemy"))
                    {
                        hit.transform.root.gameObject.SendMessage("TakeDamage", m_shotDamage);                   
                    }
                }

                m_bulletParticleSystem.Emit(1);
                m_playerController.ShotFired();
            }
        }

        //Reload
        if (Input.GetKeyDown(GlobalValues.g_settings.m_kcKeyReload) && m_currentMagCapacity != m_maxMagCapacity && m_ammoCount > 0)
        {
            m_reloadProgress = 0;

            if (m_ammoCount - (m_maxMagCapacity - m_currentMagCapacity) < 0)
            { m_ammoCount = 0; }
            else
            { m_ammoCount -= m_maxMagCapacity - m_currentMagCapacity; }

            m_currentMagCapacity = m_maxMagCapacity;

            m_audioOrigin.PlayOneShot(reloadEffect);
        }

        //Aim Down Sight
        if (m_left || m_right)
        {
            float distCovered = (Time.time - m_startTime) * m_adsSpeed;
            float fractionOfJourney = distCovered / m_journeyLength;

            if (m_left)
            { transform.localPosition = Vector3.Lerp(m_gunPosition, m_adsPos, fractionOfJourney); }

            if (m_right)
            { transform.localPosition = Vector3.Lerp(m_gunPosition, m_hipPos, fractionOfJourney); }
        }

        //Down - Go left
        if (Input.GetKeyDown(GlobalValues.g_settings.m_kcKeyAltFire))
        {
            m_startTime = Time.time;
            m_gunPosition = transform.localPosition;
            m_journeyLength = Vector3.Distance(m_gunPosition, m_adsPos);
            m_left = true;
            m_right = false;
        }

        //Up - Go right
        if (Input.GetKeyUp(GlobalValues.g_settings.m_kcKeyAltFire) || (Input.GetKey(GlobalValues.g_settings.m_kcKeyAltFire) == false && transform.localPosition != m_hipPos)) 
        {
            m_startTime = Time.time;
            m_gunPosition = transform.localPosition;
            m_journeyLength = Vector3.Distance(m_gunPosition, m_hipPos);
            m_right = true;
            m_left = false;
        }
    }

    private void HudValues()
    {
        m_hudController.CurrentWeaponAmmoMagazine = m_currentMagCapacity;
        m_hudController.CurrentWeaponAmmoReserve = m_ammoCount;
        m_hudController.CurrentWeaponAmmoMagazineMax = m_maxMagCapacity;
        m_hudController.CurrentWeaponAmmoReserveMax = m_maxAmmoCount;
    }

    private void WeaponPointingAt()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_bulletOrigin.position, m_bulletOrigin.forward, out hit, m_bulletRange))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.gameObject.SendMessage("PointedAt", m_bulletOrigin);
            }
            else if (hit.transform.root.CompareTag("Enemy"))
            {
                hit.transform.root.gameObject.SendMessage("PointedAt", m_bulletOrigin);
            }
        }
    }
    #endregion


    #region Is Held State Management
    public void StartHolding()
    {
        m_isHeld = true;

        //Makes mesh visable
        m_meshRenderer.enabled = true;

        //Moves gun to hip 
        transform.localPosition = m_hipPos;
        m_right = false;
        m_left = false;

        //Updates recoil values and enables gun
        m_playerController.NewRecoilValues(m_recoil, m_recoilDampening, m_recoilControl);
    }

    public void StopHolding()
    {
        m_isHeld = false;
        m_meshRenderer.enabled = false;

        //Resets reload if mid reload
        if (m_reloadProgress < m_reloadTime)
        { m_reloadProgress = 0; }
    }

    public bool GetIsHeld()
    {
        return m_isHeld;
    }
    #endregion
}
