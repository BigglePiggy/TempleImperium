//////////////////////////////////////////////////                                              
//                                              //
//  PlayerGun                                   //
//  Creates and allows use of bespoke guns      //
//                                              //
//  Contributors : Eddie                        //
//                                              //
//////////////////////////////////////////////////  

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    ////Declarations
    //Public
    public bool particleBased;
    public bool automatic;
    public float adsSpeed;
    public float recoil;
    public float recoilDampening;
    public float recoilControl;
    public int startingAmmoCount;
    public int maxMagCapacity;
    public float reloadTime;
    public float fireRate;
    public float bulletRange;
    public float shotDamage;
    public Vector3 adsPos;
    public Vector3 hipPos;

    //Private
    private bool isHeld;
    private int ammoCount;
    private int currentMagCapacity;
    private float timeSinceLastShot;
    private float reloadProgress;

    private bool left;
    private bool right;
    private float startTime;
    private float journeyLength;
    private Vector3 objectPosition;

    //Components
    private Transform bulletOrigin;
    private AudioSource audioOrigin;
    private PlayerController playerController;
    private ParticleSystem bulletParticleSystem;
    private MeshRenderer meshRenderer;

    //Sound effects
    public AudioClip shotEffect;
    public AudioClip reloadEffect;


    //Initalization
    void Start()
    {
        isHeld = false;
        left = false;
        right = false;
        ammoCount = startingAmmoCount;
        currentMagCapacity = maxMagCapacity;
        reloadProgress = reloadTime;
        timeSinceLastShot = fireRate;

        bulletOrigin = transform.Find("Bullet Origin");
        audioOrigin = GetComponent<AudioSource>();
        playerController = transform.root.GetComponent<PlayerController>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (particleBased)
        { bulletParticleSystem = bulletOrigin.GetComponent<ParticleSystem>(); }
    }

    //Called per frame
    void Update()
    {
        if (isHeld)
        {
            _inputs();
        }
    }


    ////Bespoke functions
    ///Private
    private void _inputs()
    {
        ////Shooting
        //Since last shot update
        if (timeSinceLastShot < fireRate)
        { timeSinceLastShot += Time.deltaTime; }

        //Reload update
        if (reloadProgress < reloadTime)
        { reloadProgress += Time.deltaTime; }

        //Stops shooting on an empty magazines or during a reload
        if (currentMagCapacity != 0 && reloadProgress >= reloadTime)
        {
            //Automatic
            if (automatic && Input.GetMouseButton(0) && timeSinceLastShot >= fireRate)
            {
                currentMagCapacity--;
                timeSinceLastShot = 0;
                audioOrigin.PlayOneShot(shotEffect);

                if (particleBased)
                {
                    bulletParticleSystem.Emit(1);
                }

                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(bulletOrigin.position, bulletOrigin.forward, out hit, bulletRange))
                    {
                        if (hit.transform.CompareTag("Enemy"))
                        { hit.transform.GetComponent<EnemyController>()._raycastHit(shotDamage); }
                    }
                }

                //Apply recoil
                playerController._shotFired();
            }

            //Semi Automatic
            else if (automatic == false && Input.GetMouseButtonDown(0) && timeSinceLastShot >= fireRate)
            {
                currentMagCapacity--;
                timeSinceLastShot = 0;
                audioOrigin.PlayOneShot(shotEffect);

                if (particleBased)
                {
                    bulletParticleSystem.Emit(1);
                }

                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(bulletOrigin.position, bulletOrigin.forward, out hit, bulletRange))
                    { } //Hit
                }

                //Apply recoil
                playerController._shotFired();
            }
        }

        ////Reload
        if (Input.GetKeyDown(KeyCode.R) && currentMagCapacity != maxMagCapacity && ammoCount > 0)
        {
            reloadProgress = 0;

            if (ammoCount - (maxMagCapacity - currentMagCapacity) < 0)
            { ammoCount = 0; }
            else
            { ammoCount -= maxMagCapacity - currentMagCapacity; }

            currentMagCapacity = maxMagCapacity;

            audioOrigin.PlayOneShot(reloadEffect);
        }

        ////Aim Down Sight
        if (left || right)
        {
            float distCovered = (Time.time - startTime) * adsSpeed;
            float fractionOfJourney = distCovered / journeyLength;

            if (left)
            { transform.localPosition = Vector3.Lerp(objectPosition, adsPos, fractionOfJourney); }

            if (right)
            { transform.localPosition = Vector3.Lerp(objectPosition, hipPos, fractionOfJourney); }
        }

        //Down - Go left
        if (Input.GetMouseButtonDown(1))
        {
            startTime = Time.time;
            objectPosition = transform.localPosition;
            journeyLength = Vector3.Distance(objectPosition, adsPos);
            left = true;
            right = false;
        }

        //Up - Go right
        if (Input.GetMouseButtonUp(1))
        {
            startTime = Time.time;
            objectPosition = transform.localPosition;
            journeyLength = Vector3.Distance(objectPosition, hipPos);
            right = true;
            left = false;
        }
    }

    ///Public
    //Start holding
    public void _startHolding()
    {
        isHeld = true;

        //Makes mesh visable
        meshRenderer.enabled = true;

        //Moves gun to hip 
        transform.localPosition = hipPos;
        right = false;
        left = false;

        //Updates recoil values and enables gun
        playerController._newRecoilValues(recoil, recoilDampening, recoilControl);
    }

    //stop holding
    public void _stopHolding()
    {
        isHeld = false;
        meshRenderer.enabled = false;

        //Resets reload if mid reload
        if (reloadProgress < reloadTime)
        { reloadProgress = 0; }
    }

    //Get isHeld
    public bool _getIsHeld()
    {
        return isHeld;
    }
}
