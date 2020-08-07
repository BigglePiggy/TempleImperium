﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//written by Eddie

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
    [Tooltip("Acceleration applied to enemy when dodging")]
    public float m_dodgeAcceleration;
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

    //Enemy States
    bool m_isAttacking;         //When true, movement is applied
    bool m_playerInSight;       //True when player is in (unobstruced) view
    bool m_playerHasBeenHit;    //True when enemy hits into the player
    bool m_alive;
    bool m_deathHitEffectPlayed;

    float m_heightTarget;
    float m_attackTimer;
    float m_deadTimer;
    float m_stunnedTimer;
    public float GetStunnedTimer() { return m_stunnedTimer; }

    bool m_pointedAt;
    Transform m_pointedAtBulletOrigin; //Transform of bulletorigin of ray that last hit
    public void PointedAt(Transform bulletOrigin)
    { m_pointedAtBulletOrigin = bulletOrigin; }

    GameLogic.StarstoneElement m_starstone;  //Enemy Starstone element

    float m_currentHealth;      //Current Health
    Transform m_player;         //Player position reference
    Rigidbody m_enemyRb;        //Rigidbody component
    AudioSource m_audioSource;  //Audio source component       
    AmmoDropController m_AmmoDropController;    //ammo drop controller reference
    SoundManager m_soundManager;
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
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        m_audioSource = GetComponent<AudioSource>();
        m_AmmoDropController = GameObject.Find("AmmoDropController").GetComponent<AmmoDropController>();
        m_soundManager = GameObject.FindGameObjectWithTag("Sound Manager").GetComponent<SoundManager>();

        //Variable assignment
        m_path = new Stack<Vector3>();
        m_currentHealth = m_startingHealth;
        m_isAttacking = false;
        m_playerInSight = false;
        m_pointedAt = false;
        m_alive = true;

        m_heightTarget = Random.Range(m_minimumHeight, m_maximumHeight);
        m_attackTimer = m_attackRate;
        m_stunnedTimer = 0;
        m_deadTimer = 0;
        m_deathHitEffectPlayed = false;
    }

    //Called per frame
    void Update()
    {
        PlayerInSight();
        if (m_stunnedTimer <= 0 && m_alive)
        {
            Turning();
        }
        else { m_stunnedTimer -= Time.deltaTime; }
        PathUpdate();
        PathRead();

        if (m_alive == false) 
        {
            if(m_deadTimer < 4) 
            { m_deadTimer += Time.deltaTime; }
            else 
            { Destroy(gameObject); }
        }
    }

    //Fixed update
    private void FixedUpdate()
    {
        if (m_stunnedTimer <= 0 && m_alive)
        { Movement(); }
        Gravity();
        LinearDrag();
        VelocityLimits();
    }


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

        if (m_path != null && m_path.Count > 0)
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
        RaycastHit downRay;
        RaycastHit upRay;

        float targetSwitchDistance = 0.6f;
        if (Physics.Raycast(transform.position, Vector3.down, out downRay, 100))
        {}

        if (Mathf.Abs(m_heightTarget - transform.position.y) < targetSwitchDistance)
        {
            m_heightTarget = Random.Range(downRay.point.y + m_minimumHeight, downRay.point.y + m_maximumHeight);
        }

        if (Physics.Raycast(transform.position, Vector3.up, out upRay, 100))
        {
            if (m_heightTarget > upRay.point.y - targetSwitchDistance)
            {
                m_heightTarget = Random.Range(downRay.point.y + m_minimumHeight, upRay.point.y - targetSwitchDistance);
            }
        }


        //Is being pointed at by the player's gun
        RaycastHit hit = new RaycastHit();
        if (m_pointedAtBulletOrigin != null)
        {
            if (Physics.Raycast(m_pointedAtBulletOrigin.position, m_pointedAtBulletOrigin.forward, out hit, 100))
            {
                if (hit.transform == transform)
                {
                    m_pointedAt = true;
                }

                else
                {
                    m_pointedAt = false;
                }
            }
            else
            {
                m_pointedAt = false;
            }
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
                        //Dodge player's gun 
                        if (m_pointedAt)
                        {
                            m_enemyRb.AddForce((transform.position - hit.point).normalized * m_dodgeAcceleration);
                        }

                        //Float about the Y Axis
                        else
                        {
                            m_enemyRb.AddForce((new Vector3(transform.position.x, m_heightTarget, transform.position.z) - transform.position).normalized * m_normalAcceleration * (Time.deltaTime * 100), ForceMode.Force);
                        }
                    }
                }

                //Attacking & not yet hit
                if (m_isAttacking && m_playerHasBeenHit == false)
                {
                    Vector3 target = new Vector3(m_nextNode.x, m_player.position.y, m_nextNode.z);
                    m_enemyRb.AddForce((target - transform.position).normalized * m_normalAcceleration * (Time.deltaTime * 100), ForceMode.Force);
                }

                //Attacking and has hit - Retreats away
                if (m_isAttacking && m_playerHasBeenHit)
                {
                    //Dodge player's gun 
                    if (m_pointedAt)
                    {
                        m_enemyRb.AddForce((transform.position - hit.point).normalized * m_dodgeAcceleration);
                    }

                    else
                    {
                        m_enemyRb.AddForce((new Vector3(transform.position.x, m_heightTarget, transform.position.z) - m_player.position).normalized * m_attackAcceleration * (Time.deltaTime * 100), ForceMode.Force);
                    }
                }
            }

            //Out of attack range
            else
            {
                //Get into attack range through nodes
                Vector3 target = new Vector3(m_nextNode.x,  m_heightTarget, m_nextNode.z);
                m_enemyRb.AddForce((target - transform.position).normalized * m_normalAcceleration * (Time.deltaTime * 100), ForceMode.Force);               
            }

            //Outside of retreating range
            if (Vector3.Distance(new Vector3(transform.position.x, m_player.position.y, transform.position.z), m_player.position) > m_retreatDistance)
            {
                //Is retreating from an attack  - Stop retreating
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
            Vector3 target = new Vector3(m_nextNode.x, m_heightTarget, m_nextNode.z);
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

    public void Stun(float input_stunTime)
    {
        m_stunnedTimer = input_stunTime;
    }
    #endregion


    #region Health & Enemy State
    public void TakeDamage(float change)
    {
        if (m_currentHealth - change <= 0 && m_alive)
        {
            m_audioSource.PlayOneShot(m_soundManager.m_lightEnemyDeath, 0.3f);
            GameObject.Find("Game Logic").GetComponent<GameLogic>().WaveEventEnemyDeath(0);
            m_AmmoDropController.RollDropChanceAtPosition(transform.position);
            m_alive = false;
            m_gravity = 10;
            m_enemyRb.constraints = RigidbodyConstraints.None;
            m_enemyRb.AddTorque(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
            Destroy(transform.Find("Body").GetComponent<Animator>());
        }
        else
        { 
            m_currentHealth -= change;
            m_audioSource.PlayOneShot(m_soundManager.m_lightEnemyDamaged);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_isAttacking && m_playerHasBeenHit == false)
        {
            if (collision.transform.CompareTag("Player"))
            {
                m_playerHasBeenHit = true;
                collision.transform.GetComponent<PlayerController>().TakeDamage(m_attackDamage);
                m_audioSource.PlayOneShot(m_soundManager.m_lightEnemyAttack);

                if (m_starstone == GameLogic.StarstoneElement.Arc)
                {
                    collision.transform.GetComponent<PlayerController>().ReduceDrag();
                    collision.transform.GetComponent<Rigidbody>().AddForce(new Vector3(m_enemyRb.velocity.x, 0, m_enemyRb.velocity.z).normalized * m_powerPushback, ForceMode.Acceleration);
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (m_alive == false && m_deathHitEffectPlayed == false && Mathf.Abs(m_enemyRb.velocity.y) < 0.5f)
        {
            RaycastHit downRay;
            if (Physics.Raycast(transform.position, Vector3.down, out downRay, 100))
            {
                if (Mathf.Abs(downRay.point.y - transform.position.y) < 2)
                {
                    m_audioSource.PlayOneShot(m_soundManager.m_lightEnemyHitGround);
                    Debug.Log("Played");
                    m_deathHitEffectPlayed = true;
                }
            }
        }
    }
    #endregion
}
