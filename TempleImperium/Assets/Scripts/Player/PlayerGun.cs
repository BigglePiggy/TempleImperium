﻿using System.Collections;
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
    [Tooltip("Used to determine sound effects played")]
    public bool m_isPrimaryWeapon;
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


    //ADS variables
    bool m_left;                //True when gun moving towards ADS position 
    bool m_right;               //True when gun moving back to hip position 
    float m_startTime;          //Stores a Lerp journey's start time
    float m_journeyLength;      //Stores the length of the current proposed journey
    Vector3 m_gunPosition;      //Lerp gun position

    bool m_isHeld;              //True when gun is selected by player & active   
    int m_ammoCount;            //Total ammo for this gun
    public int GetAmmoCount() { return m_ammoCount; }   //Getter for ammo count
    public void AddAmmo(int value) { m_ammoCount += value; }   //Getter for ammo count
    int m_currentMagCapacity;   //Ammo in the current magazine
    float m_timeSinceLastShot;  //Time since last shot in seconds
    float m_reloadProgress;     //Tracks reload progress 

    PlayerController m_playerController;    //Reference to Player Controller script - Recoil values are passed 
    HUDController m_hudController;          //Reference to Hud Controller script - Display values are passed
    SoundManager m_soundManager;            //Per scene sound clip storage

    AudioSource m_audioOrigin;              //Audio origin component
    GameObject m_gunModel;                  //Mesh renderer component - used to make gun invisible when not in use
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

        m_gunModel = transform.Find("Gun Model").gameObject;
        m_bulletOrigin = m_gunModel.transform.Find("Bullet Origin");
        m_audioOrigin = GetComponent<AudioSource>();
        m_bulletParticleSystem = m_bulletOrigin.GetComponent<ParticleSystem>();
        m_soundManager = GameObject.FindGameObjectWithTag("Sound Manager").GetComponent<SoundManager>();

        StopHolding();
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
    //Gun input logic
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

            //Automatic firing detection
            if (m_automatic && Input.GetKey(GlobalValues.g_settings.m_kcKeyFire) && m_timeSinceLastShot >= m_fireRate) 
            { shoot = true; }

            //Semi-Automatic firing detection
            if (m_automatic == false && Input.GetKeyDown(GlobalValues.g_settings.m_kcKeyFire) && m_timeSinceLastShot >= m_fireRate) 
            { shoot = true; }

            //If a shot is fired
            if (shoot) 
            {
                m_currentMagCapacity--;
                m_timeSinceLastShot = 0;

                //Raycasts and applies damage to any enemy hit
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

                m_bulletParticleSystem.Emit(1); //Bullet particle representation
                m_playerController.ShotFired(); //Applies recoil to the player camera for from this shot

                //Detemines which effect is needed and plays it
                if (m_isPrimaryWeapon)
                { m_audioOrigin.PlayOneShot(m_soundManager.m_primaryGunFire, GlobalValues.g_settings.m_fVolumeGuns); }
                else
                { m_audioOrigin.PlayOneShot(m_soundManager.m_secondaryGunFire, GlobalValues.g_settings.m_fVolumeGuns); }
            }
        }

        //Dry fire
        else if (Input.GetKeyDown(GlobalValues.g_settings.m_kcKeyFire))
        {
            if (m_isPrimaryWeapon)
            { m_audioOrigin.PlayOneShot(m_soundManager.m_primaryGunDryFire, GlobalValues.g_settings.m_fVolumeGuns); }
            else
            { m_audioOrigin.PlayOneShot(m_soundManager.m_secondaryGunDryFire, GlobalValues.g_settings.m_fVolumeGuns); }
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

            //Relod audcio clip
            if (m_isPrimaryWeapon)
            { m_audioOrigin.PlayOneShot(m_soundManager.m_primaryGunReload, GlobalValues.g_settings.m_fVolumeGuns); }
            else
            { m_audioOrigin.PlayOneShot(m_soundManager.m_secondaryGunReload, GlobalValues.g_settings.m_fVolumeGuns); }
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

    //Sends messages to enemeis when pointed at them
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

    //Updates the HUD values when being held
    private void HudValues()
    {
        m_hudController.CurrentWeaponAmmoMagazine = m_currentMagCapacity;
        m_hudController.CurrentWeaponAmmoReserve = m_ammoCount;
        m_hudController.CurrentWeaponAmmoMagazineMax = m_maxMagCapacity;
        m_hudController.CurrentWeaponAmmoReserveMax = m_maxAmmoCount;
    }
    #endregion


    #region Is Held State Management
    //Sets this gun as held
    public void StartHolding()
    {
        m_isHeld = true;

        //Makes model visable
        m_gunModel.SetActive(true);

        //Moves gun to hip 
        transform.localPosition = m_hipPos;
        m_right = false;
        m_left = false;

        //Updates recoil values and enables gun
        m_playerController.NewRecoilValues(m_recoil, m_recoilDampening, m_recoilControl);
    }

    //Sets this gun as not-held
    public void StopHolding()
    {
        m_isHeld = false;
        m_gunModel.SetActive(false);

        //Resets reload if mid reload
        if (m_reloadProgress < m_reloadTime)
        { m_reloadProgress = 0; }
    }

    //Returns the held state of the gun
    public bool GetIsHeld()
    {
        return m_isHeld;
    }
    #endregion
}
