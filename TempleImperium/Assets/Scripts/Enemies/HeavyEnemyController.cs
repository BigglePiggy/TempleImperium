﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by Eddie

public class HeavyEnemyController : MonoBehaviour
{
    //Enemy controller script
    //What this script does:
    /*
        - Manages bespoke enemy physics
        - Manages enemy behavior & attacks
        - Handles pathfinding data
    */
    
    ////TODO
    //Make health a property for PlayerGun to interact with
    ////
    
    #region Declarations
    [Header("Enemy Configuration")]
    [Tooltip("Maximum health the enemy can have")]
    public float m_startingHealth;
    [Tooltip("Distance at which enemy snaps head to player")]
    public float m_viewDistance;
    [Tooltip("Distance at which enemy stops moving towards the player")]
    public float m_stopDistance;
    [Tooltip("Distance at which enemy starts shooting at the player")]
    public float m_shootingDistance;
    [Space]

    [Header("Shooting")]
    [Tooltip("Rate of fire of enemy's weapon")]
    public float m_fireRate;
    [Tooltip("Range of accuarcy offset on enemy's weapon")]
    public Vector3 m_accuracyOffset;
    [Tooltip("Range of accuarcy offset on enemy's weapon")]
    public Vector3 m_relativeShotForce;
    [Tooltip("Heavy projectile prefab reference")]
    public GameObject m_heavyProjectile;
    [Space]

    [Header("Sound Effects")]
    [Tooltip("Effect played when the enemy shoots")]
    public AudioClip m_shotEffect;

    [Header("Enemy Physics")]
    [Tooltip("Downward force the enemy experiences")]
    public float m_gravity;
    [Tooltip("Acceleration applied to enemy to move")]
    public Vector3 m_acceleration;
    [Tooltip("Maximum Y axis velocity the enemy can experience")]
    public float m_verticalLimit;
    [Tooltip("Maximum Z & X axis velocity the player can experience")]
    public float m_horizontalLimit;
    [Tooltip("Z & X axis drag applied to the enemy whilst on the ground")]
    public Vector3 m_horizontalDrag;
    [Tooltip("Z & X axis drag applied to the enemy whilst in the air")]
    public Vector3 m_airHorizontalDrag;
    [Tooltip("The speed that the enemy turns at")]
    public float m_rotateSpeed;

    [Header("Materials")]
    public Material matSummon;
    public Material matArc;
    public Material matHazard;
    public Material matPower;


    //Pathfinding
    Pathfinder m_pathfinder;    //Pathfinder script reference - Updates m_path 
    Stack<Vector3> m_path;      //Holds the entire path generated by Pathfinder
    Vector3 m_nextNode;         //Holds the next node in the path
    float m_pathUpdateBuffer;

    //Enemy States
    bool m_isGrounded;          //True when on a surface
    bool m_onSlope;             //True when on a slope
    bool m_applyGravity;        //Changes state dependent on the above two (stops slope momentum issues)
    bool m_moving;              //When true movement is applied

    GameLogic.StarstoneElement m_starstone;  //Enemy Starstone element

    bool m_playerInSight;       //True when player is in (unobstruced) view
    float m_timeSinceLastShot;  //Manages gun firerate by tracking time since last shot
    float m_stunnedTimer;  

    bool m_pointedAt;
    Transform m_pointedAtBulletOrigin; //Transform of bulletorigin of ray that last hit
    public void PointedAt(Transform bulletOrigin)
    { m_pointedAtBulletOrigin = bulletOrigin; }

    float m_currentHealth;      //Current Health
    Transform m_player;         //Player position reference
    Rigidbody m_enemyRb;        //Rigidbody component
    AudioSource m_audioSource;  //Audio source component
    AmmoDropController m_AmmoDropController;    //ammo drop controller reference
    Transform m_bulletOrigin;   //Reference to editor positioned bullet origin
    #endregion


    //Initalization
    public void Initialize(GameLogic.StarstoneElement element)
    {
        //Startstone
        m_starstone = element;


        switch (m_starstone)
        {
            case GameLogic.StarstoneElement.Summon:
                transform.Find("Enemy Head").Find("Outline").GetComponent<MeshRenderer>().material = matSummon;
                break;
            case GameLogic.StarstoneElement.Arc:
                transform.Find("Enemy Head").Find("Outline").GetComponent<MeshRenderer>().material = matArc;
                break;
            case GameLogic.StarstoneElement.Hazard:
                transform.Find("Enemy Head").Find("Outline").GetComponent<MeshRenderer>().material = matHazard;
                break;
            case GameLogic.StarstoneElement.Power:
                transform.Find("Enemy Head").Find("Outline").GetComponent<MeshRenderer>().material = matPower;
                break;
        }

        //Component assignment
        m_enemyRb = GetComponent<Rigidbody>();
        m_pathfinder = GameObject.FindGameObjectWithTag("Nodes").GetComponent<Pathfinder>();
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        m_audioSource = GetComponent<AudioSource>();
        m_bulletOrigin = transform.Find("Bullet Origin");
        m_AmmoDropController = GameObject.Find("AmmoDropController").GetComponent<AmmoDropController>();

        //Variable assignment
        m_path = new Stack<Vector3>();
        m_currentHealth = m_startingHealth;
        m_isGrounded = true;
        m_onSlope = false;
        m_applyGravity = true;
        m_moving = true;
        m_playerInSight = false;
        m_timeSinceLastShot = 0;
        m_stunnedTimer = 0;
    }

    //Called per frame
    void Update()
    {
        PlayerInSight();
        if (m_stunnedTimer <= 0)
        {
            Turning();
            Shooting();
        }
        else { m_stunnedTimer -= Time.deltaTime; }
        PathUpdate();
        PathRead();
    }

    //Fixed update
    private void FixedUpdate()
    {
        ExtremityCheck();
        if (m_stunnedTimer <= 0)
        { Movement(); }
        Gravity();
        LinearDrag();
        VelocityLimits();
        Exceptions();
    }


    #region Weapon
    private void PlayerInSight()
    {
        RaycastHit hit;
        Debug.DrawLine(transform.position, m_player.position);

        if (Physics.Linecast(transform.position, m_player.position, out hit))
        {
            if (hit.transform.CompareTag("Player"))
            {
                m_playerInSight = true;
            }
            else
            {
                m_playerInSight = false;
            }
        }
        else
        {
            m_playerInSight = false;
        }
    }

    private void Shooting()
    {
        if (m_timeSinceLastShot < m_fireRate)
        { m_timeSinceLastShot += Time.deltaTime; }

        if (m_playerInSight && Vector3.Distance(m_player.position,transform.position) < m_shootingDistance && m_timeSinceLastShot >= m_fireRate) 
        {
            //Shoot
            m_timeSinceLastShot = 0;
            HeavyProjectile projectile = Instantiate(m_heavyProjectile, m_bulletOrigin.position, Quaternion.LookRotation(m_player.position - m_bulletOrigin.position)).GetComponent<HeavyProjectile>();
            projectile.Initalization(m_starstone);
            projectile.GetComponent<Rigidbody>().AddRelativeForce(m_relativeShotForce);
        }
    }
    #endregion


    #region Pathfinding
    public void PathUpdate()
    {
        if (m_pathUpdateBuffer <= 0)
        {
            m_path = new Stack<Vector3>();
            m_path = m_pathfinder.FindPathBetween(transform.position, m_player.position);
            m_pathUpdateBuffer = 0.4f;
        }
        else
        {
            m_pathUpdateBuffer -= Time.deltaTime;
        }
    }

    public void PathRead()
    {
        float m_nodeSwitchDistance = 2f;

        if (m_path != null)
        {
            if (Vector3.Distance(m_path.Peek(), new Vector3(transform.position.x, m_path.Peek().y, transform.position.z)) > m_nodeSwitchDistance && m_path.Count > 1)
            {
                m_nextNode = m_path.Peek();
            }
            else
            {
                m_path.Pop();
            }
        }
        else
        {
            m_nextNode = m_player.position;
        }
    }
    #endregion


    #region Physics & Movement
    private void ExtremityCheck()
    {
        float startHeight = -0.88f;
        float spacing = 0.1f;
        float rayLength = 0.1f;
        bool slope = false;

        Vector3 TL = transform.position + new Vector3(-spacing, startHeight, spacing);
        Vector3 TR = transform.position + new Vector3(spacing, startHeight, spacing);
        Vector3 BL = transform.position + new Vector3(-spacing, startHeight, -spacing);
        Vector3 BR = transform.position + new Vector3(spacing, startHeight, -spacing);

        RaycastHit[] TLDown = Physics.RaycastAll(TL, -transform.up, rayLength);
        RaycastHit[] TRDown = Physics.RaycastAll(TR, -transform.up, rayLength);
        RaycastHit[] BLDown = Physics.RaycastAll(BL, -transform.up, rayLength);
        RaycastHit[] BRDown = Physics.RaycastAll(BR, -transform.up, rayLength);

        //All hit combination
        List<RaycastHit> allHits = new List<RaycastHit>();
        allHits.AddRange(TLDown);
        allHits.AddRange(TRDown);
        allHits.AddRange(BLDown);
        allHits.AddRange(BRDown);

        //Slope check
        for (int i = 0; i < allHits.Count; i++)
        {
            if (allHits[i].collider.gameObject.CompareTag("Slope"))
            {
                slope = true;
                break;
            }
        }

        if (slope)
        { m_onSlope = true; }
        else
        { m_onSlope = false; }

        //Grounded Check
        if (allHits.Count > 0)
        { m_isGrounded = true; }
        else
        { m_isGrounded = false; }
    }
    
    private void Turning()
    {
        Vector3 point;

        if (m_playerInSight)
        {
            point = new Vector3(m_player.position.x, m_player.position.y, m_player.position.z);
        }
        else
        {
            point = new Vector3(m_nextNode.x, m_nextNode.y, m_nextNode.z);
        }

        Vector3 targetDirection = point - transform.position;
        float step = m_rotateSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    private void Movement()
    {
        //Stopping distance
        if (Vector3.Distance(transform.position, m_player.position) < m_stopDistance && m_playerInSight)
        { m_moving = false; }
        else
        { m_moving = true; }

        //Movement applciation
        if (m_moving)
        { m_enemyRb.AddForce((m_nextNode - transform.position).normalized * m_acceleration.z * (Time.deltaTime * 100), ForceMode.Force); }
    }

    private void VelocityLimits()
    {
        //Velocity relative to view
        Vector3 relativeVelocity = new Vector3();
        relativeVelocity.x = (Vector3.right * Vector3.Dot(m_enemyRb.velocity, transform.right)).x;
        relativeVelocity.z = (Vector3.forward * Vector3.Dot(m_enemyRb.velocity, transform.forward)).z;

        //Clamps velocity
        Vector3 limitedVeloicty = new Vector3();
        float totalHorLimit = m_horizontalLimit;

        //Total Horizonal Velocity
        if (new Vector3(relativeVelocity.x, 0, relativeVelocity.z).magnitude > totalHorLimit)
        {
            limitedVeloicty = new Vector3(m_enemyRb.velocity.x, 0, m_enemyRb.velocity.z).normalized * totalHorLimit;
            m_enemyRb.velocity = new Vector3(limitedVeloicty.x, m_enemyRb.velocity.y, limitedVeloicty.z);
        }

        //Vertical Velocity
        if (new Vector3(0, m_enemyRb.velocity.y, 0).magnitude > m_verticalLimit)
        {
            limitedVeloicty = new Vector3(0, m_enemyRb.velocity.y, 0).normalized * m_verticalLimit;
            m_enemyRb.velocity = new Vector3(m_enemyRb.velocity.x, limitedVeloicty.y, m_enemyRb.velocity.z);
        }
    }

    private void LinearDrag()
    {
        //Velocity relative to view
        Vector3 relativeVelocity = new Vector3();
        relativeVelocity.x = (Vector3.right * Vector3.Dot(m_enemyRb.velocity, transform.right)).x;
        relativeVelocity.z = (Vector3.forward * Vector3.Dot(m_enemyRb.velocity, transform.forward)).z;
        Vector3 drag = m_horizontalDrag;

        //Air modifer
        if (m_isGrounded == false)
        { drag = m_airHorizontalDrag; }

        //Drag application
        if (relativeVelocity.x != 0 || relativeVelocity.z != 0)
        {
            //Left 
            if (relativeVelocity.x > 0)
            { m_enemyRb.AddRelativeForce(Vector3.left * (relativeVelocity.x * drag.x) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Right 
            if (relativeVelocity.x < 0)
            { m_enemyRb.AddRelativeForce(Vector3.right * (relativeVelocity.x * -drag.x) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Forward 
            if (relativeVelocity.z > 0)
            { m_enemyRb.AddRelativeForce(Vector3.back * (relativeVelocity.z * drag.z) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Back
            if (relativeVelocity.z < 0)
            { m_enemyRb.AddRelativeForce(Vector3.forward * (relativeVelocity.z * -drag.z) * (Time.deltaTime * 100), ForceMode.Acceleration); }
        }
    }

    private void Gravity()
    {
        if (m_applyGravity)
        { m_enemyRb.AddForce(Vector3.down * m_gravity, ForceMode.Acceleration); }
    }

    private void Exceptions()
    {
        //Slope stop Y velocity Avoidance
        if (m_moving == false && m_onSlope && m_enemyRb.velocity.y > 0)
        { m_enemyRb.velocity = new Vector3(m_enemyRb.velocity.x, 0, m_enemyRb.velocity.z); }

        //Slope sliding
        if (m_moving == false && m_onSlope && m_enemyRb.velocity.y != 0)
        { m_applyGravity = false; }
        else { m_applyGravity = true; }

        //Slope climbing
        if (m_onSlope)
        { m_applyGravity = false; }
        else { m_applyGravity = true; }
    }

    public void Stun(float input_stunTime) 
    {
        m_stunnedTimer = input_stunTime;
    }
    #endregion


    #region Health & Enemy State
    public void TakeDamage(float change)
    {
        if (m_currentHealth - change <= 0)
        {
            Destroy(this.gameObject);
            GameObject.Find("Game Logic").GetComponent<GameLogic>().WaveEventEnemyDeath(2);
            m_AmmoDropController.RollDropChanceAtPosition(transform.position);
        }
        else
        { m_currentHealth -= change; }
    }
    #endregion
}
