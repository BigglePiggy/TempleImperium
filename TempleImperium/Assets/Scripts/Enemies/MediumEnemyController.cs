﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by Eddie

public class MediumEnemyController : MonoBehaviour
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
    [Tooltip("Maximum distance enemy shots can travel")]
    public float m_maxShotDistance;
    [Tooltip("Rate of fire of enemy's weapon")]
    public float m_fireRate;
    [Tooltip("Number of bullets that can be shot before needing to reload")]
    public int m_magSize;
    [Tooltip("RTime it takes to reload in seconds")]
    public float m_reloadTime;
    [Tooltip("Range of accuarcy offset on enemy's weapon")]
    public Vector3 m_accuracyOffset;
    [Tooltip("Shot damage applied to player when hit")]
    public float m_shotDamage;

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
    [Tooltip("The speed that enemy's body turns at")]
    public float m_bodyRotateSpeed;
    [Tooltip("The speed that enemy's head turns at")]
    public float m_headRotateSpeed;

    [Header("Starstones")]
    [Tooltip("Force applied to the player when hit by enemy influenced by the 'Power' starstone")]
    public float m_powerPushback;

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
    float m_coverUpdateBuffer;
    Stack<Vector3> m_coverPath;
    Vector3 m_nextCoverNode;
    Vector3 m_coverDestinationNode;

    //Enemy States
    bool m_isGrounded;          //True when on a surface
    bool m_onSlope;             //True when on a slope
    bool m_applyGravity;        //Changes state dependent on the above two (stops slope momentum issues)
    bool m_moving;              //When true movement is applied

    GameLogic.StarstoneElement m_starstone;  //Enemy Starstone element

    float m_timeSinceLastShot;  //Manages gun firerate by tracking time since last shot
    float m_reloadTimer;
    float m_stunnedTimer;
    public float GetStunnedTimer() { return m_stunnedTimer; }

    bool m_pointedAt;   //True when gun aimed at enemy
    Transform m_pointedAtBulletOrigin; //Transform of bulletorigin of ray that last hit
    public void PointedAt(Transform bulletOrigin)
    { m_pointedAtBulletOrigin = bulletOrigin; }


    int m_currentMag;
    float m_currentHealth;      //Current Health
    Transform m_player;         //Player position reference
    Transform m_enemyHead;      //Enemy head reference - Head is rotated
    Rigidbody m_enemyRb;        //Rigidbody component
    AudioSource m_audioSource;  //Audio source component
    AmmoDropController m_AmmoDropController;    //ammo drop controller reference
    Transform m_bulletOrigin;               //Reference to editor positioned bullet origin
    ParticleSystem m_bulletParticleSystem;  //Used to emit when gun is shot
    SoundManager m_soundManager; //Per scene sound clip storage
    #endregion


    //Initalization
    public void Initialize(GameLogic.StarstoneElement element)
    {
        //Startstone
        m_starstone = element;

        switch (m_starstone)
        {
            case GameLogic.StarstoneElement.Summon:
                transform.Find("Outline").GetComponent<MeshRenderer>().material = matSummon;
                break;
            case GameLogic.StarstoneElement.Arc:
                transform.Find("Outline").GetComponent<MeshRenderer>().material = matArc;
                break;
            case GameLogic.StarstoneElement.Hazard:
                transform.Find("Outline").GetComponent<MeshRenderer>().material = matHazard;
                break;
            case GameLogic.StarstoneElement.Power:
                transform.Find("Outline").GetComponent<MeshRenderer>().material = matPower;
                break;
        }

        //Component assignment
        m_enemyRb = GetComponent<Rigidbody>();
        m_pathfinder = GameObject.FindGameObjectWithTag("Nodes").GetComponent<Pathfinder>();
        m_enemyHead = transform.Find("Enemy Head");
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        m_audioSource = GetComponent<AudioSource>();
        m_bulletOrigin = m_enemyHead.Find("Bullet Origin");
        m_bulletParticleSystem = m_bulletOrigin.GetComponent<ParticleSystem>();
        m_AmmoDropController = GameObject.Find("AmmoDropController").GetComponent<AmmoDropController>();
        m_soundManager = GameObject.FindGameObjectWithTag("Sound Manager").GetComponent<SoundManager>();

        //Variable assignment
        m_path = new Stack<Vector3>();
        m_currentHealth = m_startingHealth;
        m_isGrounded = true;
        m_onSlope = false;
        m_applyGravity = true;
        m_moving = true;
        m_timeSinceLastShot = 0;
        m_stunnedTimer = 0;
        m_currentMag = m_magSize;
        m_reloadTimer = m_reloadTime;
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
        {
            Movement();
        }
        Gravity();
        LinearDrag();
        VelocityLimits();
        Exceptions();
    }


    #region Weapon
    //Determines if the enemy is in sight of the player

    private bool PlayerInSight()
    {
        float rayGap = 1.5f;
        int hits = 0;
        int nulls = 0;

        RaycastHit leftHit;
        RaycastHit middleHit;
        RaycastHit rightHit;

        if (Physics.Linecast(new Vector3(transform.localPosition.x - rayGap, transform.localPosition.y, transform.localPosition.z), m_player.position, out leftHit)) 
        { if (leftHit.transform.CompareTag("Player") || leftHit.transform == transform) { hits++; } }
        else 
        { nulls++; }

        if (Physics.Linecast(transform.position, m_player.position, out middleHit))
        { if (middleHit.transform.CompareTag("Player") || middleHit.transform == transform) { hits++; } }
        else
        { nulls++; }

        if (Physics.Linecast(new Vector3(transform.localPosition.x + rayGap, transform.localPosition.y, transform.localPosition.z), m_player.position, out rightHit))
        { if (rightHit.transform.CompareTag("Player") || rightHit.transform == transform) { hits++; } }
        else
        { nulls++; }    

        if (hits + nulls >= 2)
        { return true; }

        else { return false; }
    }

    //Handles shooting, the accuracy of shots and reloading 
    private void Shooting()
    {
        if (m_timeSinceLastShot < m_fireRate)
        { m_timeSinceLastShot += Time.deltaTime; }

        if (m_timeSinceLastShot >= m_fireRate && PlayerInSight() && Vector3.Distance(transform.position, m_player.position) < m_shootingDistance &&  m_reloadTimer >= m_reloadTime)
        {
            Vector3 target = new Vector3(m_player.position.x + Random.Range(-m_accuracyOffset.x, m_accuracyOffset.x), m_player.position.y + Random.Range(-m_accuracyOffset.y, m_accuracyOffset.y), m_player.position.z + Random.Range(-m_accuracyOffset.z, m_accuracyOffset.z));

            RaycastHit hit;
            Debug.DrawLine(m_bulletOrigin.position, target);

            if (Physics.Linecast(m_bulletOrigin.position, target, out hit))
            {
                if (hit.transform.CompareTag("Player")) 
                {
                    hit.transform.GetComponent<PlayerController>().TakeDamage(m_shotDamage);

                    if(m_starstone == GameLogic.StarstoneElement.Arc) 
                    {
                        hit.transform.GetComponent<PlayerController>().ReduceDrag();
                        hit.transform.GetComponent<Rigidbody>().AddForce((hit.point - new Vector3(m_bulletOrigin.position.x, 0, m_bulletOrigin.position.z)).normalized * m_powerPushback, ForceMode.Acceleration);
                    }
                }
            }

            m_timeSinceLastShot = 0;

            m_bulletOrigin.LookAt(target);
            m_bulletParticleSystem.Emit(1);
            m_audioSource.PlayOneShot(m_soundManager.m_lightEnemyAttack, GlobalValues.g_settings.m_fVolumeEnemies);

            m_currentMag--;
        }

        if(m_currentMag <= 0) 
        {
            m_reloadTimer = 0;
            m_currentMag = m_magSize;
        }

        if(m_reloadTimer < m_reloadTime) 
        {
            m_reloadTimer += Time.deltaTime;
        }
    }
    #endregion


    #region Pathfinding
    //Generates a new path for the enemy
    private void PathUpdate()
    {
        //Path update buffer limits the comutaional strain of pathfinding updates
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

    //Switches the target node if withtin range of the current one
    private void PathRead()
    {
        float m_nodeSwitchDistance = 2f;

        if (m_path != null && m_path.Count > 0)
        {
            //Calculates the distance disregarding the Y axis
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
    //Determines if grounded and/or on a slope thorugh raycasts
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

    //Turns the enemy towards appropriate point
    private void Turning()
    {
        Vector3 headPoint;
        Vector3 bodyPoint;

        if (PlayerInSight())
        {
            headPoint = new Vector3(m_player.position.x, m_player.position.y, m_player.position.z);
            bodyPoint = new Vector3(m_player.position.x, m_player.position.y, m_player.position.z);
        }
        else
        {
            headPoint = new Vector3(m_nextNode.x, m_enemyHead.position.y, m_nextNode.z);
            bodyPoint = new Vector3(m_nextNode.x, m_nextNode.y, m_nextNode.z);
        }

        Vector3 headTargetDirection = headPoint - m_enemyHead.position;
        float headStep = m_headRotateSpeed * Time.deltaTime;
        Vector3 headNewDirection = Vector3.RotateTowards(m_enemyHead.forward, headTargetDirection, headStep, 0.0f);
        m_enemyHead.rotation = Quaternion.LookRotation(headNewDirection);

        Vector3 bodyTargetDirection = bodyPoint - transform.position;
        float bodyStep = m_bodyRotateSpeed * Time.deltaTime;
        Vector3 bodyNewDirection = Vector3.RotateTowards(transform.forward, bodyTargetDirection, bodyStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(bodyNewDirection);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    //Creates movement by applying forces 
    private void Movement()
    {
        //Stopping distance
        if (Vector3.Distance(transform.position, m_player.position) < m_stopDistance && PlayerInSight())
        { m_moving = false; }
        else
        { m_moving = true; }

        //Going to cover when reloading
        if (m_reloadTimer < m_reloadTime) 
        {
            float m_nodeSwitchDistance = 2f;

            if (m_coverUpdateBuffer <= 0 )
            {
                RaycastHit hit;
                if (Physics.Linecast(m_coverDestinationNode, m_player.position, out hit)) 
                { 
                    if (hit.collider.CompareTag("Player") && hit.collider.CompareTag("Enemy") == false) 
                    {
                        m_coverDestinationNode = m_pathfinder.FindClosestCoveredNode(transform.position, 15f).transform.position;
                        m_coverPath = m_pathfinder.FindPathBetween(transform.position, m_coverDestinationNode);
                    } 
                }

                if (Physics.Linecast(transform.position, m_nextNode, out hit))
                {
                    m_coverDestinationNode = m_pathfinder.FindClosestCoveredNode(transform.position, 15f).transform.position;
                    m_coverPath = m_pathfinder.FindPathBetween(transform.position, m_coverDestinationNode);
                }

                m_coverUpdateBuffer = 1f;
            }
            else 
            {
                m_coverUpdateBuffer -= Time.deltaTime;
            }

            if (m_coverPath != null && m_coverPath.Count > 0)
            {
                if (Vector3.Distance(m_coverPath.Peek(), new Vector3(transform.position.x, m_coverPath.Peek().y, transform.position.z)) > m_nodeSwitchDistance)
                {
                    m_nextCoverNode = m_coverPath.Peek();
                }
                else
                {
                    m_coverPath.Pop();
                }
            }

            if (PlayerInSight())
            { m_enemyRb.AddForce((m_nextCoverNode - transform.position).normalized * m_acceleration.z * (Time.deltaTime * 100), ForceMode.Force); }
        }

        //Movement applciation
        else if (m_moving)
        { m_enemyRb.AddForce((m_nextNode - transform.position).normalized * m_acceleration.z * (Time.deltaTime * 100), ForceMode.Force); }
    }

    //Limits the maximum veloicty relative to orientation
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

    //Constantly applies drag forces against the enemy relative to orientation
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

    //Applies gravity to the enemy when needed
    private void Gravity()
    {
        if (m_applyGravity)
        { m_enemyRb.AddForce(Vector3.down * m_gravity, ForceMode.Acceleration); }
    }

    //Edge cases related to slope veloicty issues
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

    //Stops movement and attacks for a period of time - Activated by bugs on contact
    public void Stun(float input_stunTime)
    {
        m_stunnedTimer = input_stunTime;
    }
    #endregion


    #region Health & Enemy State
    //Removes health form the enemy
    public void TakeDamage(float change)
    {
        if (m_currentHealth - change <= 0)
        {
            Destroy(this.gameObject);
            GameObject.Find("Game Logic").GetComponent<GameLogic>().WaveEventEnemyDeath(1);
            m_AmmoDropController.RollDropChanceAtPosition(transform.position);
        }
        else
        { m_currentHealth -= change; }
    }
    #endregion
}
