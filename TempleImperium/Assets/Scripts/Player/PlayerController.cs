using System.Collections.Generic;
using UnityEngine;

//Created by Eddie

public class PlayerController : MonoBehaviour
{
    //Player controller script - there should be only one player object with an instance of me!
    //What this script does:
    /*
        - Handles keyboard and mouse inputs
        - Handles bespoke player physics
        - Manages weapon switching
        - Manages abilities
    */

    ////TODO
    //Make health a property for EnemyController to interact with
    ////

    #region Declarations
    [Header("Player Configuration")]
    [Tooltip("Maximum health the player can have")]
    public float m_maximumHealth;
    [Tooltip("Maximum angle that the player is able to look up at")]
    public float m_upAngleLimit;
    [Tooltip("Maximum angle that the player is able to look down at")]
    public float m_downAngleLimit;
    [Tooltip("Buffer in seconds between offensive ability uses")]
    public float m_offensiveCooldown;
    [Tooltip("Buffer in seconds between defensive ability uses")]
    public float m_defensiveCooldown;
    [Tooltip("Prefab of the defenisve ability greande")]
    public GameObject grenade;
    [Space]

    [Header("Sound Effects")]
    [Tooltip("Effect played when the player jumps")]
    public AudioClip jumpClip;
    [Space]

    [Header("Player Physics")]
    [Tooltip("Downward force the player experiences")]
    public float m_gravity;
    [Tooltip("Acceleration the player's ground inputs apply")]
    public Vector3 m_acceleration;
    [Tooltip("Acceleration the player's in-air inputs apply")]
    public Vector3 m_airAcceleration;
    [Tooltip("Maximum Y axis velocity the player can experience")]
    public float m_verticalLimit;
    [Tooltip("Maximum Z & X axis velocity the player can experience")]
    public float m_horizontalLimit;
    [Tooltip("Z & X axis drag applied to the player whilst on the ground")]
    public Vector3 m_horizontalDrag;
    [Tooltip("Z & X axis drag applied to the player whilst in the air")]
    public Vector3 m_airHorizontalDrag;
    [Space]

    [Tooltip("Increase to the maximum horizonal velocity when sprinting")]
    public float m_sprintLimitIncrease;
    [Tooltip("Acceleration the player's jump action applies")]
    public float m_jumpPower;
    [Tooltip("Buffer in seconds between jump actions")]
    public float m_jumpBuffer;


    //Recoil 
    float m_currentRecoil;      //Amount of recoil being applied to the player's veiw
    float m_gunRecoil;          //Is set to m_currentRecoil when a bullet is shot - Taken from the currently held gun  
    float m_gunRecoilDampening; //Rate of decay on m_currentRecoil- Taken from the currently held gun   
    float m_gunRecoilControl;   //Amount of control the player's Y Mouse input has on m_currentRecoil

    //Movement
    string m_zDirection;        //Holds "Forward" or "Back"
    string m_xDirection;        //Holds "Left" or "Right"
    string m_lastKeyDownZ;      //Determines value of m_zDirection
    string m_lastKeyDownX;      //Determines value of m_xDirection

    float m_sinceLastJump;      //Tracks jump buffer in seconds

    //Player States           
    bool m_isGrounded;          //True when on a surface
    bool m_onSlope;             //True when on a slope
    bool m_isJumping;           //True whilst in the air after a jump
    bool m_applyGravity;        //Changes state depending on the above three (stops slope momentum issues)

    float m_offensiveCurrentCooldown;   //Offensive ability cooldown in seconds
    float m_defensiveCurrentCooldown;   //Defensive ability cooldown in seconds

    float m_health; //Current health

    private SettingsObject m_settings;  //Settings object used to determine all input keys
    public SettingsObject Settings { set { m_settings = value; } }  //Setter for m_settings - used by SettingsManager

    Rigidbody m_playerRb;       //Rigidbody component
    AudioSource m_audioOrigin;  //Audio source component
    PlayerGun m_primaryGun;     //Primary weapon script
    PlayerGun m_secondaryGun;   //Secondary weapon script

    Transform m_playerCamera;   //Player's POV camera
    Transform m_grenadeOrigin;  //Defensive ability grenade origin  
    #endregion


    //Initalization
    private void Start()
    {
        m_sinceLastJump = m_jumpBuffer; //Jump buffer setting
        m_offensiveCurrentCooldown = m_offensiveCooldown;   //Offensive cooldown setting
        m_defensiveCurrentCooldown = m_defensiveCooldown;   //Defensive cooldown setting

        m_health = m_maximumHealth; //Health set to maximum

        m_playerRb = GetComponent<Rigidbody>();     //Rigidbody reference
        m_audioOrigin = GetComponent<AudioSource>();    //AudioSource reference
        m_playerCamera = transform.Find("Player Camera");   //Player Camera reference
        m_grenadeOrigin = transform.Find("Player Camera").transform.Find("Grenade Origin"); //Grenade Origin reference

        m_primaryGun = transform.Find("Player Camera").transform.Find("Primary Gun").GetComponent<PlayerGun>();     //Primary gun script reference
        m_secondaryGun = transform.Find("Player Camera").transform.Find("Secondary Gun").GetComponent<PlayerGun>(); //Secondary gun script reference
        m_primaryGun.StartHolding();   //Primary gun configured to be held

        Cursor.lockState = CursorLockMode.Locked;   //Locks the mouse
    }

    //Called per frame
    private void Update()
    {
        if (m_health > 0)
        {
            KeyboardInput();
            MouseInputRecoil();
            WeaponSwitching();
        }
    }

    //Fixed update
    private void FixedUpdate()
    {
        if (m_health > 0)
        {
            Movement();
        }

        ExtremityCheck();
        Exceptions();
        Gravity();
        LinearDrag();
        VelocityLimits();
    }


    #region Mouse & Weapons
    private void MouseInputRecoil()
    {
        float rotateVertical = Input.GetAxis("Mouse Y") * m_settings.m_fMouseSensitivityY * (Time.deltaTime * 100);     //Frame rate indenpentdent Y rotation value
        float rotateHorizontal = Input.GetAxis("Mouse X") * m_settings.m_fMouseSensitivityX * (Time.deltaTime * 100);   //Frame rate indenpentdent X rotation value

        transform.Rotate(transform.up * rotateHorizontal);  //Rotates the X Axis

        //Recoil value manager
        if (m_currentRecoil < 0 && m_playerCamera.localRotation.x > -0.7 * (m_upAngleLimit / 90)) //Limits the camera's angle of rotation
        {
            m_playerCamera.Rotate(m_currentRecoil, 0, 0); //Applies current recoil to the camera
            m_currentRecoil += m_gunRecoilDampening * (Time.deltaTime * 100);   //Reduces current recoil recoil 
        }

        if (m_currentRecoil < 0 && rotateVertical < 0 && m_playerCamera.localRotation.x >= 0.7 * (m_downAngleLimit / 90)) //Limits the camera's angle of rotation
        { m_playerCamera.transform.Rotate(rotateVertical * m_gunRecoilControl, 0, 0); } //Applies the players mouse inputs against recoil

        //Normal control
        else
        {
            if (m_playerCamera.localRotation.x >= 0.7 * (m_downAngleLimit / 90))
            {
                if (rotateVertical > 0)
                { m_playerCamera.transform.Rotate(-rotateVertical, 0, 0); }
            }
            else if (m_playerCamera.localRotation.x <= -0.7 * (m_upAngleLimit / 90))
            {
                if (rotateVertical < 0)
                { m_playerCamera.Rotate(-rotateVertical, 0, 0); }
            }
            else
            { m_playerCamera.Rotate(-rotateVertical, 0, 0); }
        }
    }

    private void WeaponSwitching()
    {
        //Switch to primary
        if (Input.GetKeyDown(m_settings.m_kcKeyWeaponSlot1) && m_primaryGun.GetIsHeld() == false)
        {
            m_secondaryGun.StopHolding();
            m_primaryGun.StartHolding();
        }

        //Switch to secondary
        if (Input.GetKeyDown(m_settings.m_kcKeyWeaponSlot2) && m_secondaryGun.GetIsHeld() == false)
        {
            m_primaryGun.StopHolding();
            m_secondaryGun.StartHolding();
        }
    }

    public void NewRecoilValues(float newRecoil, float newRecoilDampening, float newRecoilControl)
    {
        m_gunRecoil = newRecoil;
        m_gunRecoilDampening = newRecoilDampening;
        m_gunRecoilControl = newRecoilControl;
    }

    public void ShotFired()
    {
        m_currentRecoil = m_gunRecoil;
    }
    #endregion


    #region Keyboard & Abilities
    private void KeyboardInput()
    {
        ////Left & Right
        //Records the last direction pressed
        if (Input.GetKeyDown(m_settings.m_kcKeyMoveLeft))
        { m_lastKeyDownX = "Left"; }
        if (Input.GetKeyDown(m_settings.m_kcKeyMoveRight))
        { m_lastKeyDownX = "Right"; }
        //Sets direction to last key pressed if both are down
        if (Input.GetKey(m_settings.m_kcKeyMoveLeft) && Input.GetKey(m_settings.m_kcKeyMoveRight))
        { m_xDirection = m_lastKeyDownX; }
        else
        {
            if (Input.GetKey(m_settings.m_kcKeyMoveLeft))
            { m_xDirection = "Left"; }
            if (Input.GetKey(m_settings.m_kcKeyMoveRight))
            { m_xDirection = "Right"; }
        }
        //No input setting
        if (Input.GetKey(m_settings.m_kcKeyMoveLeft) == false && Input.GetKey(m_settings.m_kcKeyMoveRight) == false)
        { m_xDirection = "None"; }


        ////Forward & Back
        //Records the last direction pressed
        if (Input.GetKeyDown(m_settings.m_kcKeyMoveForward))
        { m_lastKeyDownZ = "Forward"; }
        if (Input.GetKeyDown(m_settings.m_kcKeyMoveBackward))
        { m_lastKeyDownZ = "Back"; }
        //Sets direction to last key pressed if both are down
        if (Input.GetKey(m_settings.m_kcKeyMoveForward) && Input.GetKey(m_settings.m_kcKeyMoveBackward))
        { m_zDirection = m_lastKeyDownZ; }
        else
        {
            if (Input.GetKey(m_settings.m_kcKeyMoveForward))
            { m_zDirection = "Forward"; }
            if (Input.GetKey(m_settings.m_kcKeyMoveBackward))
            { m_zDirection = "Back"; }
        }
        //No input setting
        if (Input.GetKey(m_settings.m_kcKeyMoveForward) == false && Input.GetKey(m_settings.m_kcKeyMoveBackward) == false)
        { m_zDirection = "None"; }


        //Jump
        if (Input.GetKeyDown(m_settings.m_kcKeyJump) && m_sinceLastJump >= m_jumpBuffer && m_isGrounded)
        {
            m_isJumping = true;

            m_playerRb.velocity = new Vector3(m_playerRb.velocity.x, 0, m_playerRb.velocity.z);

            m_playerRb.AddRelativeForce(Vector3.up * m_jumpPower);
            m_sinceLastJump = 0;

            m_audioOrigin.PlayOneShot(jumpClip);
        }

        //Jump buffer
        if (m_sinceLastJump < m_jumpBuffer)
        { m_sinceLastJump += Time.deltaTime; }
        else if (m_isGrounded)
        { m_isJumping = false; }


        //Offensive ability
        if (Input.GetKeyDown(m_settings.m_kcKeyAbility1) && m_offensiveCurrentCooldown >= m_offensiveCooldown)
        {
            OffensiveAbility();
            m_offensiveCurrentCooldown = 0;
        }

        //Defensive ability
        if (Input.GetKeyDown(m_settings.m_kcKeyAbility2) && m_defensiveCurrentCooldown >= m_defensiveCooldown)
        {
            DefensiveAbility();
            m_defensiveCurrentCooldown = 0;
        }

        //Increase Cooldown Counters
        if (m_offensiveCurrentCooldown < m_offensiveCooldown)
        { m_offensiveCurrentCooldown += Time.deltaTime; }

        if (m_defensiveCurrentCooldown < m_defensiveCooldown)
        { m_defensiveCurrentCooldown += Time.deltaTime; }
    }

    private void OffensiveAbility()
    {

    }

    private void DefensiveAbility()
    {
        Instantiate(grenade, m_grenadeOrigin.position, m_grenadeOrigin.rotation);
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

    private void Movement()
    {
        Vector3 direction = new Vector3();

        switch (m_xDirection)
        {
            case "Left":
                direction.x = -1;
                break;

            case "Right":
                direction.x = 1;
                break;
        }

        switch (m_zDirection)
        {
            case "Forward":
                direction.z = 1;
                break;

            case "Back":
                direction.z = -1;
                break;
        }

        Vector3 speeds = m_acceleration;

        if (m_isGrounded == false)
        { speeds = m_airAcceleration; }
        speeds *= Time.deltaTime * 100;

        m_playerRb.AddRelativeForce(new Vector3(direction.normalized.x * speeds.x, 0, direction.normalized.z * speeds.z) * (Time.deltaTime * 100), ForceMode.Acceleration);
    }

    private void VelocityLimits()
    {
        //Velocity relative to view
        Vector3 relativeVelocity = new Vector3();
        relativeVelocity.x = (Vector3.right * Vector3.Dot(m_playerRb.velocity, transform.right)).x;
        relativeVelocity.z = (Vector3.forward * Vector3.Dot(m_playerRb.velocity, transform.forward)).z;

        //Clamps velocity
        Vector3 limitedVeloicty = new Vector3();
        float totalHorLimit = m_horizontalLimit;

        //Sprint modifer
        if (Input.GetKey(KeyCode.LeftShift))
        {
            totalHorLimit += m_sprintLimitIncrease;
            m_verticalLimit = m_verticalLimit + m_sprintLimitIncrease;
        }

        //Total Horizonal Velocity
        if (new Vector3(relativeVelocity.x, 0, relativeVelocity.z).magnitude > totalHorLimit)
        {
            limitedVeloicty = new Vector3(m_playerRb.velocity.x, 0, m_playerRb.velocity.z).normalized * totalHorLimit;
            m_playerRb.velocity = new Vector3(limitedVeloicty.x, m_playerRb.velocity.y, limitedVeloicty.z);
        }

        //Vertical Velocity
        if (new Vector3(0, m_playerRb.velocity.y, 0).magnitude > m_verticalLimit)
        {
            limitedVeloicty = new Vector3(0, m_playerRb.velocity.y, 0).normalized * m_verticalLimit;
            m_playerRb.velocity = new Vector3(m_playerRb.velocity.x, limitedVeloicty.y, m_playerRb.velocity.z);
        }
    }

    private void LinearDrag()
    {
        //Velocity relative to view
        Vector3 relativeVelocity = new Vector3();
        relativeVelocity.x = (Vector3.right * Vector3.Dot(m_playerRb.velocity, transform.right)).x;
        relativeVelocity.z = (Vector3.forward * Vector3.Dot(m_playerRb.velocity, transform.forward)).z;
        Vector3 drag = m_horizontalDrag;

        //Air modifer
        if (m_isGrounded == false)
        { drag = m_airHorizontalDrag; }

        //Drag application
        if (relativeVelocity.x != 0 || relativeVelocity.z != 0)
        {
            //Left 
            if (relativeVelocity.x > 0 && m_xDirection != "Right")
            { m_playerRb.AddRelativeForce(Vector3.left * (relativeVelocity.x * drag.x) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Right 
            if (relativeVelocity.x < 0 && m_xDirection != "Left")
            { m_playerRb.AddRelativeForce(Vector3.right * (relativeVelocity.x * -drag.x) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Forward 
            if (relativeVelocity.z > 0 && m_zDirection != "Forward")
            { m_playerRb.AddRelativeForce(Vector3.back * (relativeVelocity.z * drag.z) * (Time.deltaTime * 100), ForceMode.Acceleration); }

            //Back
            if (relativeVelocity.z < 0 && m_zDirection != "Back")
            { m_playerRb.AddRelativeForce(Vector3.forward * (relativeVelocity.z * -drag.z) * (Time.deltaTime * 100), ForceMode.Acceleration); }
        }
    }

    private void Gravity()
    {
        //Gravity
        if (m_applyGravity)
        { m_playerRb.AddForce(Vector3.down * m_gravity * (Time.deltaTime * 100), ForceMode.Acceleration); }
    }

    private void Exceptions()
    {
        //Slope stop Y velocity Avoidance
        if (m_isJumping == false && m_onSlope && m_xDirection == "None" && m_zDirection == "None" && m_playerRb.velocity.y > 0)
        { m_playerRb.velocity = new Vector3(m_playerRb.velocity.x, 0, m_playerRb.velocity.z); }

        //Slope sliding
        if (m_onSlope && m_xDirection == "None" && m_zDirection == "None" && m_playerRb.velocity.y != 0)
        { m_applyGravity = false; }
        else { m_applyGravity = true; }
    }
    #endregion


    #region Health & Player state
    public void TakeDamage(float damage)
    {
        m_health -= damage;

        if(m_health <= 0) 
        { PlayerDeath(); }
    }

    public void PlayerDeath() 
    {
        m_health = 0;
        m_primaryGun.StopHolding();
        m_secondaryGun.StopHolding();
    }
    #endregion
}