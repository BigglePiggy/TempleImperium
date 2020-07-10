//////////////////////////////////////////////////                                              
//                                              //
//  EnemyController                            //
//  Handles all enemy movement & interactions   //
//                                              //
//  Contributors : Eddie                        //
//                                              //
//////////////////////////////////////////////////                                              

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    ////Declarations
    //Public
    public float startingHealth;
    public float viewDistance;
    public float stopDistance;
    private Transform player;
    public float gravity;
    public Vector3 acceleration;
    public float verticalLimit;
    public float horizontalLimit;
    public Vector3 horizontalDrag;
    public Vector3 airHorizontalDrag;
    public Vector2 rotateSpeed;

    //Private 
    private Stack<Vector3> path;
    private Vector3 nextNode;
    private bool isGrounded;
    private bool onSlope;
    private bool applyGravity;
    private bool moving;
    private bool playerInSight;
    private float currentHealth;
    private int pathUpdateTicker = 100;

    //Components
    private Pathfinder pathfinder;
    private Transform enemyHead;
    private Rigidbody enemyRb;


    //Initalization
    void Start()
    {
        //Component assignment
        enemyRb = GetComponent<Rigidbody>();
        pathfinder = GameObject.FindGameObjectWithTag("Nodes").GetComponent<Pathfinder>();
        enemyHead = transform.Find("Enemy Head");
        player = GameObject.FindGameObjectWithTag("Player").transform;

        //Variable assignment
        path = new Stack<Vector3>();
        currentHealth = startingHealth;
        isGrounded = true;
        onSlope = false;
        applyGravity = true;
        moving = true;
        playerInSight = false;
    }


    //Called per frame
    void Update()
    {
        _playerInSight();
        _turning();

        if (pathUpdateTicker == 100)
        {
            _pathUpdate();
            _pathRead();
            pathUpdateTicker = 0;
        }
    }

    //Handles phyics seperate from frame-rate
    private void FixedUpdate()
    {
        _extremityCheck();
        _movement();
        _gravity();
        _linearDrag();
        _velocityLimits();
        _exceptions();

        if(pathUpdateTicker < 100) 
        {
            pathUpdateTicker++;
        }
    }

    ////Bespoke functions
    //Private
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
        { onSlope = true; }
        else
        { onSlope = false; }

        //Grounded Check
        if (allHits.Count > 0)
        { isGrounded = true; }
        else
        { isGrounded = false; }
    }

    //Player is sight detection
    private void _playerInSight()
    {
        RaycastHit[] inLineOfSight = Physics.RaycastAll(enemyHead.position, player.position - enemyHead.position, viewDistance);

        if (inLineOfSight.Length > 0)
        {
            if (inLineOfSight[0].collider.CompareTag("Player"))
            { playerInSight = true; }
        }
        else
        {
            playerInSight = false;
        }
    }

    //Turning
    private void _turning()
    {
        Vector3 headPoint;
        Vector3 bodyPoint;

        if (playerInSight)
        {
            headPoint = new Vector3(player.position.x, player.position.y, player.position.z);
            bodyPoint = new Vector3(player.position.x, player.position.y, player.position.z);
        }
        else
        {
            headPoint = new Vector3(nextNode.x, enemyHead.position.y, nextNode.z);
            bodyPoint = new Vector3(nextNode.x, nextNode.y, nextNode.z);
        }

        Vector3 headTargetDirection = headPoint - enemyHead.position;
        float headStep = rotateSpeed.x * Time.deltaTime;
        Vector3 headNewDirection = Vector3.RotateTowards(enemyHead.forward, headTargetDirection, headStep, 0.0f);
        enemyHead.rotation = Quaternion.LookRotation(headNewDirection);

        Vector3 bodyTargetDirection = bodyPoint - transform.position;
        float bodyStep = rotateSpeed.y * Time.deltaTime;
        Vector3 bodyNewDirection = Vector3.RotateTowards(transform.forward, bodyTargetDirection, bodyStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(bodyNewDirection);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    //Movement
    private void _movement()
    {
        //Stopping distance
        if (Vector3.Distance(transform.position, player.position) < stopDistance)
        { moving = false; }
        else
        { moving = true; }

        //Movement applciation
        if (moving)
        { enemyRb.AddRelativeForce(Vector3.forward * acceleration.z * (Time.deltaTime * 100), ForceMode.Force); }
    }

    //Velocity limiter
    private void _velocityLimits()
    {
        //Velocity relative to view
        Vector3 relativeVelocity = new Vector3();
        relativeVelocity.x = (Vector3.right * Vector3.Dot(enemyRb.velocity, transform.right)).x;
        relativeVelocity.z = (Vector3.forward * Vector3.Dot(enemyRb.velocity, transform.forward)).z;

        //Clamps velocity
        Vector3 limitedVeloicty = new Vector3();
        float totalHorLimit = horizontalLimit;

        //Total Horizonal Velocity
        if (new Vector3(relativeVelocity.x, 0, relativeVelocity.z).magnitude > totalHorLimit)
        {
            limitedVeloicty = new Vector3(enemyRb.velocity.x, 0, enemyRb.velocity.z).normalized * totalHorLimit;
            enemyRb.velocity = new Vector3(limitedVeloicty.x, enemyRb.velocity.y, limitedVeloicty.z);
        }

        //Vertical Velocity
        if (new Vector3(0, enemyRb.velocity.y, 0).magnitude > verticalLimit)
        {
            limitedVeloicty = new Vector3(0, enemyRb.velocity.y, 0).normalized * verticalLimit;
            enemyRb.velocity = new Vector3(enemyRb.velocity.x, limitedVeloicty.y, enemyRb.velocity.z);
        }
    }

    //Linear drag 
    private void _linearDrag()
    {
        //Velocity relative to view
        Vector3 relativeVelocity = new Vector3();
        relativeVelocity.x = (Vector3.right * Vector3.Dot(enemyRb.velocity, transform.right)).x;
        relativeVelocity.z = (Vector3.forward * Vector3.Dot(enemyRb.velocity, transform.forward)).z;
        Vector3 drag = horizontalDrag;

        //Air modifer
        if (isGrounded == false)
        { drag = airHorizontalDrag; }

        //Drag application
        if (relativeVelocity.x != 0 || relativeVelocity.z != 0)
        {
            ////Left 
            //if (relativeVelocity.x > 0 && xDirection != "Right")
            //{ enemyRb.AddRelativeForce(Vector3.left * (relativeVelocity.x * drag.x) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            ////Right 
            //if (relativeVelocity.x < 0 && xDirection != "Left")
            //{ enemyRb.AddRelativeForce(Vector3.right * (relativeVelocity.x * -drag.x) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            ////Forward 
            //if (relativeVelocity.z > 0 && zDirection != "Forward")
            //{ enemyRb.AddRelativeForce(Vector3.back * (relativeVelocity.z * drag.z) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            ////Back
            //if (relativeVelocity.z < 0 && zDirection != "Back")
            //{ enemyRb.AddRelativeForce(Vector3.forward * (relativeVelocity.z * -drag.z) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Left 
            if (relativeVelocity.x > 0)
            { enemyRb.AddRelativeForce(Vector3.left * (relativeVelocity.x * drag.x) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Right 
            if (relativeVelocity.x < 0)
            { enemyRb.AddRelativeForce(Vector3.right * (relativeVelocity.x * -drag.x) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Forward 
            if (relativeVelocity.z > 0)
            { enemyRb.AddRelativeForce(Vector3.back * (relativeVelocity.z * drag.z) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Back
            if (relativeVelocity.z < 0)
            { enemyRb.AddRelativeForce(Vector3.forward * (relativeVelocity.z * -drag.z) * (Time.deltaTime * 100), ForceMode.Acceleration); }
        }
    }

    //Gravity
    private void _gravity()
    {
        if (applyGravity)
        { enemyRb.AddForce(Vector3.down * gravity, ForceMode.Acceleration); }
    }

    //Exceptions
    private void _exceptions()
    {
        //Slope stop Y velocity Avoidance
        if (moving == false && onSlope && enemyRb.velocity.y > 0)
        { enemyRb.velocity = new Vector3(enemyRb.velocity.x, 0, enemyRb.velocity.z); }

        //Slope sliding
        if (moving == false && onSlope && enemyRb.velocity.y != 0)
        { applyGravity = false; }
        else { applyGravity = true; }
    }

    //Take Damage
    private void _takeDamage(float change)
    {
        if (currentHealth - change < 0)
        { 
            Destroy(this.gameObject);
            GameObject.Find("Game Logic").GetComponent<GameLogic>().WaveEventEnemyDeath();
        }
        else
        { currentHealth -= change; }
    }

    ////Pathfinding
    //Path Update
    public void _pathUpdate()
    {
        path = new Stack<Vector3>();
        path = pathfinder._findPathBetween(transform.position, player.position);
    }

    //Path reader
    public void _pathRead()
    {
        if (path != null)
        {
            if (path.Peek() != transform.position)
            {
                nextNode = path.Peek();
            }
            else
            {
                path.Pop();
            }
        }
        else
        {
            nextNode = player.position;
        }
    }

    ////Collision detection
    //Particle detection
    private void OnParticleCollision(GameObject other)
    {
        _takeDamage(other.transform.parent.GetComponent<PlayerGun>().shotDamage);
    }

    //Raycast hit manager
    public void _raycastHit(float damage)
    {
        _takeDamage(damage);
    }
}
