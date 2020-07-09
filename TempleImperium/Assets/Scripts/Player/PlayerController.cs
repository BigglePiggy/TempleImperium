//////////////////////////////////////////////////                                              
//                                              //
//  PlayerController                            //
//  Handles all player movement & interactions  //
//                                              //
//  Contributors : Eddie                        //
//                                              //
////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    ////Declarations
    //Public
    public float xSensitivity, ySensitivity;
    public float downAngleLimit, upAngleLimit;
    public float gravity;
    public Vector3 acceleration;
    public Vector3 airAcceleration;
    public float jumpPower;
    public float jumpBuffer;
    public float verticalLimit;
    public float horizontalLimit;
    public Vector3 horizontalDrag;
    public Vector3 airHorizontalDrag;
    public float sprintLimitIncrease;

    //Private
    private float currentRecoil;
    private float recoil;
    private float recoilDampening;
    private float recoilControl;
    private string xDirection, zDirection;
    private string lastKeyDownX, lastKeyDownZ;
    private float sinceLastJump;
    private bool onSlope;
    private bool isGrounded;
    private bool isJumping;
    private bool applyGravity;

    //Components    
    private Transform playerCamera;
    private PlayerGun primaryGun;
    private PlayerGun secondaryGun;
    private Rigidbody playerRb;
    private AudioSource audioOrigin;

    //Sound effects
    public AudioClip jump;


    //Initalization
    private void Start()
    {
        //variables
        playerRb = GetComponent<Rigidbody>();
        audioOrigin = GetComponent<AudioSource>();
        onSlope = false;
        isGrounded = false;
        isJumping = false;
        applyGravity = true;

        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = transform.Find("Player Camera");

        //Gun setting
        primaryGun = transform.Find("Player Camera").transform.Find("Primary Gun").GetComponent<PlayerGun>();
        secondaryGun = transform.Find("Player Camera").transform.Find("Secondary Gun").GetComponent<PlayerGun>();

        //Primary gun active
        primaryGun._startHolding();
    }

    //Called per frame
    private void Update()
    {
        _keyboardInput();
        _mouseInput();
        _weaponSwitching();

        if (Input.GetKeyDown(KeyCode.O))
        { GameObject.Find("Game Logic").GetComponent<GameLogic>().WaveEventEnemyDeath(); }
    }

    //Fixed update
    private void FixedUpdate()
    {
        _extremityCheck();
        _exceptions();
        _gravity();
        _movement();
        _linearDrag();
        _velocityLimits();
    }


    ////Bespoke functions
    ///Private
    private void _mouseInput()
    {
        //Recoil value manager
        if (currentRecoil < 0 && playerCamera.localRotation.x > -0.7 * (upAngleLimit / 90))
        {
            currentRecoil += recoilDampening * (Time.deltaTime * 100);
            playerCamera.Rotate(currentRecoil, 0, 0);
        }

        //Vertical axis
        float rotateVertical = Input.GetAxis("Mouse Y") * (Time.deltaTime * 100);

        //Recoil control
        if (currentRecoil < 0 && rotateVertical < 0)
        {
            playerCamera.transform.Rotate(recoilControl * (Time.deltaTime * 100), 0, 0);
        }

        //Normal control
        else
        {
            if (playerCamera.localRotation.x >= 0.7 * (downAngleLimit / 90))
            {
                if (rotateVertical > 0)
                { playerCamera.transform.Rotate(-rotateVertical * ySensitivity, 0, 0); }
            }
            else if (playerCamera.localRotation.x <= -0.7 * (upAngleLimit / 90))
            {
                if (rotateVertical < 0)
                { playerCamera.Rotate(-rotateVertical * ySensitivity, 0, 0); }
            }
            else
            { playerCamera.Rotate(-rotateVertical * ySensitivity, 0, 0); }
        }

        //Player horizontal rotation
        float rotateHorizontal = Input.GetAxis("Mouse X") * (Time.deltaTime * 100);
        transform.Rotate(transform.up * rotateHorizontal * xSensitivity);
    }

    //Keyboard inputs
    private void _keyboardInput()
    {
        ////Left & Right
        //Records the last direction pressed
        if (Input.GetKeyDown(KeyCode.A))
        { lastKeyDownX = "Left"; }
        if (Input.GetKeyDown(KeyCode.D))
        { lastKeyDownX = "Right"; }
        //Sets direction to last key pressed if both are down
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        { xDirection = lastKeyDownX; }
        else
        {
            if (Input.GetKey(KeyCode.A))
            { xDirection = "Left"; }
            if (Input.GetKey(KeyCode.D))
            { xDirection = "Right"; }
        }
        //No input setting
        if (Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.D) == false)
        { xDirection = "None"; }


        ////Forward & Back
        //Records the last direction pressed
        if (Input.GetKeyDown(KeyCode.W))
        { lastKeyDownZ = "Forward"; }
        if (Input.GetKeyDown(KeyCode.S))
        { lastKeyDownZ = "Back"; }
        //Sets direction to last key pressed if both are down
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
        { zDirection = lastKeyDownZ; }
        else
        {
            if (Input.GetKey(KeyCode.W))
            { zDirection = "Forward"; }
            if (Input.GetKey(KeyCode.S))
            { zDirection = "Back"; }
        }
        //No input setting
        if (Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.S) == false)
        { zDirection = "None"; }


        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && sinceLastJump > jumpBuffer && isGrounded)
        {
            isJumping = true;

            playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);

            playerRb.AddRelativeForce(Vector3.up * jumpPower);
            sinceLastJump = 0;

            audioOrigin.PlayOneShot(jump);
        }

        //Jump buffer
        if (sinceLastJump < jumpBuffer)
        { sinceLastJump += Time.deltaTime; }
        else if (isGrounded)
        { isJumping = false; }
    }

    //Weapon switching
    private void _weaponSwitching()
    {
        //Switch to primary
        if (Input.GetKeyDown(KeyCode.Alpha1) && primaryGun._getIsHeld() == false)
        {
            secondaryGun._stopHolding();
            primaryGun._startHolding();
        }

        //Switch to secondary
        if (Input.GetKeyDown(KeyCode.Alpha2) && secondaryGun._getIsHeld() == false)
        {
            primaryGun._stopHolding();
            secondaryGun._startHolding();
        }
    }

    //Grounded check
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

    //Movement application
    private void _movement()
    {
        Vector3 direction = new Vector3();

        switch (xDirection)
        {
            case "Left":
                direction.x = -1;
                break;

            case "Right":
                direction.x = 1;
                break;
        }

        switch (zDirection)
        {
            case "Forward":
                direction.z = 1;
                break;

            case "Back":
                direction.z = -1;
                break;
        }

        Vector3 speeds = acceleration;

        if (isGrounded == false)
        { speeds = airAcceleration; }
        speeds *= Time.deltaTime * 100;

        playerRb.AddRelativeForce(new Vector3(direction.normalized.x * speeds.x, 0, direction.normalized.z * speeds.z) * (Time.deltaTime * 100), ForceMode.Acceleration);
    }

    //Velocity limiter
    private void _velocityLimits()
    {
        //Velocity relative to view
        Vector3 relativeVelocity = new Vector3();
        relativeVelocity.x = (Vector3.right * Vector3.Dot(playerRb.velocity, transform.right)).x;
        relativeVelocity.z = (Vector3.forward * Vector3.Dot(playerRb.velocity, transform.forward)).z;

        //Clamps velocity
        Vector3 limitedVeloicty = new Vector3();
        float totalHorLimit = horizontalLimit;

        //Sprint modifer
        if (Input.GetKey(KeyCode.LeftShift))
        {
            totalHorLimit += sprintLimitIncrease;
            verticalLimit = verticalLimit + sprintLimitIncrease;
        }

        //Total Horizonal Velocity
        if (new Vector3(relativeVelocity.x, 0, relativeVelocity.z).magnitude > totalHorLimit)
        {
            limitedVeloicty = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z).normalized * totalHorLimit;
            playerRb.velocity = new Vector3(limitedVeloicty.x, playerRb.velocity.y, limitedVeloicty.z);
        }

        //Vertical Velocity
        if (new Vector3(0, playerRb.velocity.y, 0).magnitude > verticalLimit)
        {
            limitedVeloicty = new Vector3(0, playerRb.velocity.y, 0).normalized * verticalLimit;
            playerRb.velocity = new Vector3(playerRb.velocity.x, limitedVeloicty.y, playerRb.velocity.z);
        }
    }

    //Linear drag 
    private void _linearDrag()
    {
        //Velocity relative to view
        Vector3 relativeVelocity = new Vector3();
        relativeVelocity.x = (Vector3.right * Vector3.Dot(playerRb.velocity, transform.right)).x;
        relativeVelocity.z = (Vector3.forward * Vector3.Dot(playerRb.velocity, transform.forward)).z;
        Vector3 drag = horizontalDrag;

        //Air modifer
        if (isGrounded == false)
        { drag = airHorizontalDrag; }

        //Drag application
        if (relativeVelocity.x != 0 || relativeVelocity.z != 0)
        {
            //Left 
            if (relativeVelocity.x > 0 && xDirection != "Right")
            { playerRb.AddRelativeForce(Vector3.left * (relativeVelocity.x * drag.x) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Right 
            if (relativeVelocity.x < 0 && xDirection != "Left")
            { playerRb.AddRelativeForce(Vector3.right * (relativeVelocity.x * -drag.x) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Forward 
            if (relativeVelocity.z > 0 && zDirection != "Forward")
            { playerRb.AddRelativeForce(Vector3.back * (relativeVelocity.z * drag.z) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Back
            if (relativeVelocity.z < 0 && zDirection != "Back")
            { playerRb.AddRelativeForce(Vector3.forward * (relativeVelocity.z * -drag.z) * (Time.deltaTime * 100), ForceMode.Acceleration); }
        }
    }

    //Gravity
    private void _gravity()
    {
        //Gravity
        if (applyGravity)
        { playerRb.AddForce(Vector3.down * gravity * (Time.deltaTime * 100), ForceMode.Acceleration); }
    }

    //Exceptions
    private void _exceptions()
    {
        //Slope stop Y velocity Avoidance
        if (isJumping == false && onSlope && xDirection == "None" && zDirection == "None" && playerRb.velocity.y > 0)
        { playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z); }

        //Slope sliding
        if (onSlope && xDirection == "None" && zDirection == "None" && playerRb.velocity.y != 0)
        { applyGravity = false; }
        else { applyGravity = true; }
    }

    ///Public
    //Recoil values
    public void _newRecoilValues(float newRecoil, float newRecoilDampening, float newRecoilControl)
    {
        recoil = newRecoil;
        recoilDampening = newRecoilDampening;
        recoilControl = newRecoilControl;
    }

    //Shot fired
    public void _shotFired()
    {
        currentRecoil = recoil;
    }
}