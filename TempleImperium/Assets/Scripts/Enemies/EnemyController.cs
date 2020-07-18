﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by Eddie

public class EnemyController : MonoBehaviour
{
    //Enemy controller script
    //What this script does:
    /*
        - Manages bespoke enemy physics
        - Manages enemy behavior & attacks
        - Handles pathfinding data
    */

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
    [Tooltip("Range of accuarcy offset on enemy's weapon")]
    public Vector3 m_accuracyOffset;
    [Tooltip("Shot damage applied to player when hit")]
    public float m_shotDamage;
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
    [Tooltip("The speed that enemy's body turns at")]
    public float m_bodyRotateSpeed;
    [Tooltip("The speed that enemy's head turns at")]
    public float m_headRotateSpeed;


    //Pathfinding
    private Pathfinder m_pathfinder;    //Pathfinder script reference - Updates m_path 
    private Stack<Vector3> m_path;      //Holds the entire path generated by Pathfinder
    private Vector3 m_nextNode;         //Holds the next node in the path

    //Enemy States
    private bool m_isGrounded;          //True when on a surface
    private bool m_onSlope;             //True when on a slope
    private bool m_applyGravity;        //Changes state dependent on the above two (stops slope momentum issues)
    private bool m_moving;              //When true movement is applied

    private bool m_playerInSight;       //True when player is in (unobstruced) view
    private float m_timeSinceLastShot;  //Manages gun firerate by tracking time since last shot

    private float m_currentHealth;      //Current Health
    private Transform m_player;         //Player position reference
    private Transform m_enemyHead;      //Enemy head reference - Head is rotated
    private Rigidbody m_enemyRb;        //Rigidbody component
    private AudioSource m_audioSource;  //Audio source component
    #endregion


    //Initalization
    void Start()
    {
        //Component assignment
        m_enemyRb = GetComponent<Rigidbody>();
        m_pathfinder = GameObject.FindGameObjectWithTag("Nodes").GetComponent<Pathfinder>();
        m_enemyHead = transform.Find("Enemy Head");
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        m_audioSource = GetComponent<AudioSource>();

        //Variable assignment
        m_path = new Stack<Vector3>();
        m_currentHealth = m_startingHealth;
        m_isGrounded = true;
        m_onSlope = false;
        m_applyGravity = true;
        m_moving = true;
        m_playerInSight = false;
        m_timeSinceLastShot = 0;
    }

    //Called per frame
    void Update()
    {
        _playerInSight();
        _turning();
        _shooting();
        _pathUpdate();
        _pathRead();
    }

    //Fixed update
    private void FixedUpdate()
    {
        _extremityCheck();
        _movement();
        _gravity();
        _linearDrag();
        _velocityLimits();
        _exceptions();
    }


    #region Weapon
    private void _playerInSight()
    {
        RaycastHit[] inLineOfSight = Physics.RaycastAll(m_enemyHead.position, m_player.position - m_enemyHead.position, m_viewDistance);

        if (inLineOfSight.Length > 0)
        {
            if (inLineOfSight[0].collider.CompareTag("Player"))
            { m_playerInSight = true; }
        }
        else
        {
            m_playerInSight = false;
        }
    }

    private void _shooting()
    {
        if (m_timeSinceLastShot < m_fireRate)
        { m_timeSinceLastShot += Time.deltaTime; }

        if (m_timeSinceLastShot >= m_fireRate && m_playerInSight && Vector3.Distance(transform.position, m_player.position) < m_shootingDistance)
        {
            Vector3 offset = new Vector3(Random.Range(-m_accuracyOffset.x, m_accuracyOffset.x), Random.Range(-m_accuracyOffset.y, m_accuracyOffset.y), Random.Range(-m_accuracyOffset.z, m_accuracyOffset.z));
            RaycastHit[] bulletCollisions = Physics.RaycastAll(m_enemyHead.position, (m_player.position + offset) - m_enemyHead.position, m_viewDistance);
            Debug.DrawRay(m_enemyHead.position, (m_player.position + offset) - m_enemyHead.position, Color.green, 100);

            if (bulletCollisions.Length > 0)
            {
                if (bulletCollisions[0].transform.root.CompareTag("Player"))
                {
                    bulletCollisions[0].transform.root.GetComponent<PlayerController>().TakeDamage(m_shotDamage);
                }
            }

            m_timeSinceLastShot = 0;
            m_audioSource.PlayOneShot(m_shotEffect);
        }
    }
    #endregion

    #region Pathfinding
    public void _pathUpdate()
    {
        m_path = new Stack<Vector3>();
        m_path = m_pathfinder._findPathBetween(transform.position, m_player.position);
    }

    public void _pathRead()
    {
        if (m_path != null)
        {
            if (m_path.Peek() != transform.position)
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
    private void _extremityCheck()
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
        allHits.AddRange(BLDown);

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
    
    private void _turning()
    {
        Vector3 headPoint;
        Vector3 bodyPoint;

        if (m_playerInSight)
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

    private void _movement()
    {
        //Stopping distance
        if (Vector3.Distance(transform.position, m_player.position) < m_stopDistance)
        { m_moving = false; }
        else
        { m_moving = true; }

        //Movement applciation
        if (m_moving)
        { m_enemyRb.AddForce((m_nextNode - transform.position).normalized * m_acceleration.z * (Time.deltaTime * 100), ForceMode.Force); }
    }

    private void _velocityLimits()
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

    private void _linearDrag()
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

    private void _gravity()
    {
        if (m_applyGravity)
        { m_enemyRb.AddForce(Vector3.down * m_gravity, ForceMode.Acceleration); }
    }

    private void _exceptions()
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
    #endregion

    #region Health & Enemy state
    private void _takeDamage(float change)
    {
        if (m_currentHealth - change < 0)
        {
            Destroy(this.gameObject);
            GameObject.Find("Game Logic").GetComponent<GameLogic>().WaveEventEnemyDeath();
        }
        else
        { m_currentHealth -= change; }
    }

    private void OnParticleCollision(GameObject other)
    {
        _takeDamage(other.transform.parent.GetComponent<PlayerGun>().shotDamage);
    }

    public void _raycastHit(float damage)
    {
        _takeDamage(damage);
    }
    #endregion
}
