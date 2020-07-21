﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemyController : MonoBehaviour
{
    //Light Enemy controller script
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
    [Header("Configuration")]
    [Tooltip("Maximum health the enemy can have")]
    public float m_startingHealth;
    [Tooltip("Distance at which looks at the player")]
    public float m_viewDistance;

    [Space]

    [Header("Physics")]
    [Tooltip("Downward force the enemy (can )experiences")]
    public float m_gravity;
    [Tooltip("Acceleration applied to enemy to move")]
    public float m_normalAcceleration;
    [Tooltip("Acceleration applied to enemy to attack")]
    public float m_attackAcceleration;
    [Tooltip("Maximum Y axis velocity the enemy can experience")]
    public float m_verticalLimit;
    [Tooltip("Maximum Z & X axis velocity the player can experience")]
    public float m_horizontalLimit;
    [Tooltip("Z & X axis drag applied to the enemy whilst on the ground")]
    public Vector3 m_drag;
    [Tooltip("The speed that enemy turns at")]
    public float m_rotateSpeed;
    [Space]

    [Header("Movement & Attack")]
    [Tooltip("The maximum vertical distance that the enemy can go to above any node")]
    public float m_maximumHeight;
    [Tooltip("The minimum vertical distance that the enemy can go to below any node")]
    public float m_minimumHeight;
    [Tooltip("Distance at which enemy starts attacking the player")]
    public float m_attackDistance;
    [Tooltip("Distance at which enemy float around player whilst waiting to attack")]
    public float m_retreatDistance;
    [Tooltip("Rate of enemy attacks (In seconds)")]
    public float m_attackRate;
    [Tooltip("Damage that the attack applies to the player")]
    public float m_attackDamage;



    //Pathfinding
    Pathfinder m_pathfinder;    //Pathfinder script reference - Updates m_path 
    Stack<Vector3> m_path;      //Holds the entire path generated by Pathfinder
    Vector3 m_nextNode;         //Holds the next node in the path

    //Enemy States
    bool m_isAttacking;         //When true, movement is applied
    bool m_playerInSight;       //True when player is in (unobstruced) view
    bool m_playerHasBeenHit;    //True when enemy hits into the player

    float m_heightTarget;
    float m_groundYLevel;

    float m_attackTimer;
    float m_currentHealth;      //Current Health
    Transform m_player;         //Player position reference
    Rigidbody m_enemyRb;        //Rigidbody component
    AudioSource m_audioSource;  //Audio source component        
    #endregion


    //Initalization
    void Start()
    {
        //Component assignment
        m_enemyRb = GetComponent<Rigidbody>();
        m_pathfinder = GameObject.FindGameObjectWithTag("Nodes").GetComponent<Pathfinder>();
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        m_audioSource = GetComponent<AudioSource>();

        //Variable assignment
        m_path = new Stack<Vector3>();
        m_currentHealth = m_startingHealth;
        m_isAttacking = false;
        m_playerInSight = false;

        m_heightTarget = Random.Range(m_minimumHeight, m_maximumHeight);
        m_attackTimer = m_attackRate;
    }

    //Called per frame
    void Update()
    {
        PlayerInSight();
        Turning();
        PathUpdate();
        PathRead();
    }

    //Fixed update
    private void FixedUpdate()
    {
        Movement();
        Gravity();
        LinearDrag();
        VelocityLimits();
    }


    #region Pathfinding
    public void PathUpdate()
    {
        m_path = new Stack<Vector3>();
        m_path = m_pathfinder.FindPathBetween(transform.position, m_player.position);
    }

    public void PathRead()
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

    private void Turning()
    {
        Vector3 focusPoint;

        if (m_playerInSight)
        {
            focusPoint = new Vector3(m_player.position.x, m_player.position.y, m_player.position.z);
        }
        else
        {
            focusPoint = new Vector3(m_nextNode.x, transform.position.y, m_nextNode.z);
        }

        Vector3 targetDirection = focusPoint - transform.position;
        float step = m_rotateSpeed * Time.deltaTime;
        Vector3 headNewDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(headNewDirection);
    }

    private void Movement()
    {
        //Variable Y height ground detector
        RaycastHit[] downRay = Physics.RaycastAll(transform.position, -transform.up, 100);
        if (downRay.Length > 0)
        { m_groundYLevel = downRay[0].point.y; }
        float targetSwitchDistance = 0.4f;
        if (Mathf.Abs(m_groundYLevel + m_heightTarget - transform.position.y) < targetSwitchDistance)
        {
            m_heightTarget = Random.Range(m_minimumHeight, m_maximumHeight);
        }

        //Attack rate tracker
        if (m_attackTimer < m_attackRate)
        { m_attackTimer += Time.deltaTime; }

        //Player in sight
        if (m_playerInSight)
        {
            //Withtin attack range
            if (Vector3.Distance(new Vector3(transform.position.x, m_player.position.y, transform.position.z), m_player.position) < m_attackDistance)
            {
                //Not attacking
                if (m_isAttacking == false)
                {
                    //Ready to attack
                    if (m_attackTimer >= m_attackRate)
                    {
                        //Start attacking
                        m_isAttacking = true;
                        m_playerHasBeenHit = false;
                        m_attackTimer = 0;
                    }

                    //Not ready to attack
                    else
                    {
                        //Float about the Y Axis
                        m_enemyRb.AddForce((new Vector3(transform.position.x, m_groundYLevel + m_heightTarget, transform.position.z) - transform.position).normalized * m_normalAcceleration * (Time.deltaTime * 100), ForceMode.Force);
                    }
                }

                //Attacking & not yet hit
                if (m_isAttacking && m_playerHasBeenHit == false)
                {
                    Vector3 target = new Vector3(m_nextNode.x, m_groundYLevel + m_heightTarget, m_nextNode.z);
                    m_enemyRb.AddForce((target - transform.position).normalized * m_normalAcceleration * (Time.deltaTime * 100), ForceMode.Force);
                }

                //Attacking and has hit
                if (m_isAttacking && m_playerHasBeenHit)
                {
                    m_enemyRb.AddForce((new Vector3(transform.position.x, m_groundYLevel + m_heightTarget, transform.position.z) - m_player.position).normalized * m_attackAcceleration * (Time.deltaTime * 100), ForceMode.Force);
                }
            }

            //Out of attack range
            else
            {
                //Get into attack range through nodes
                Vector3 target = new Vector3(m_nextNode.x, m_groundYLevel + m_heightTarget, m_nextNode.z);
                m_enemyRb.AddForce((target - transform.position).normalized * m_normalAcceleration * (Time.deltaTime * 100), ForceMode.Force);
            }

            //Outside of retreating range
            if (Vector3.Distance(new Vector3(transform.position.x, m_player.position.y, transform.position.z), m_player.position) > m_retreatDistance)
            {
                //Is retreating from an attack
                if (m_isAttacking && m_playerHasBeenHit)
                {
                    m_isAttacking = false;
                    m_playerHasBeenHit = false;
                }
            }
        }

        //Player not in sight - navigate through the nodes 
        else
        {
            Vector3 target = new Vector3(m_nextNode.x, m_groundYLevel + m_heightTarget, m_nextNode.z);
            m_enemyRb.AddForce((target - transform.position).normalized * m_normalAcceleration * (Time.deltaTime * 100), ForceMode.Force);

            m_isAttacking = false;
            m_playerHasBeenHit = false;
        }
    }

    private void VelocityLimits()
    {
        //Velocity relative to view
        Vector3 relativeVelocity = new Vector3();
        relativeVelocity.x = (Vector3.right * Vector3.Dot(m_enemyRb.velocity, transform.right)).x;
        relativeVelocity.z = (Vector3.forward * Vector3.Dot(m_enemyRb.velocity, transform.forward)).z;

        //Clamps velocity
        Vector3 limitedVeloicty = new Vector3();

        //Total Horizonal Velocity
        if (new Vector3(relativeVelocity.x, 0, relativeVelocity.z).magnitude > m_horizontalLimit)
        {
            limitedVeloicty = new Vector3(m_enemyRb.velocity.x, 0, m_enemyRb.velocity.z).normalized * m_horizontalLimit;
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
        relativeVelocity.y = (Vector3.up * Vector3.Dot(m_enemyRb.velocity, transform.up)).y;

        relativeVelocity.z = (Vector3.forward * Vector3.Dot(m_enemyRb.velocity, transform.forward)).z;
        Vector3 drag = m_drag;

        //Drag application
        if (relativeVelocity.x != 0 || relativeVelocity.z != 0)
        {
            //Left 
            if (relativeVelocity.x > 0)
            { m_enemyRb.AddRelativeForce(Vector3.left * (relativeVelocity.x * m_drag.x) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Right 
            if (relativeVelocity.x < 0)
            { m_enemyRb.AddRelativeForce(Vector3.right * (relativeVelocity.x * -m_drag.x) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Forward 
            if (relativeVelocity.z > 0)
            { m_enemyRb.AddRelativeForce(Vector3.back * (relativeVelocity.z * m_drag.z) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Back
            if (relativeVelocity.z < 0)
            { m_enemyRb.AddRelativeForce(Vector3.forward * (relativeVelocity.z * -m_drag.z) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Down
            if (m_enemyRb.velocity.y > 0)
            { m_enemyRb.AddRelativeForce(Vector3.down * (relativeVelocity.y * m_drag.y) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Up
            if (m_enemyRb.velocity.y < 0)
            { m_enemyRb.AddRelativeForce(Vector3.up * (relativeVelocity.y * -m_drag.y) * (Time.deltaTime * 100), ForceMode.Acceleration); }
        }
    }

    private void Gravity()
    {
        m_enemyRb.AddForce(Vector3.down * m_gravity, ForceMode.Acceleration);
    }
    #endregion


    #region Health & Enemy State
    private void TakeDamage(float change)
    {
        if (m_currentHealth - change <= 0)
        {
            Destroy(this.gameObject);
            GameObject.Find("Game Logic").GetComponent<GameLogic>().WaveEventEnemyDeath();
        }
        else
        { m_currentHealth -= change; }
    }

    public void RaycastHit(float damage)
    {
        TakeDamage(damage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_isAttacking && m_playerHasBeenHit == false)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                m_playerHasBeenHit = true;
                collision.gameObject.SendMessage("TakeDamage", m_attackDamage);
            }

            else if (collision.transform.root.gameObject.CompareTag("Player"))
            { 
                m_playerHasBeenHit = true;
                collision.transform.root.gameObject.SendMessage("TakeDamage", m_attackDamage);
            }
        }
    }
    #endregion
}
