using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase
//deprecated!!



public class PlayerMovement : MonoBehaviour
{
    //basic player movement class - placeholder for development, will be using Ed's instead

    public enum ControllerMode { KeyboardMouse, Gamepad };

    //member variables

    public ControllerMode m_eControllerMode = ControllerMode.KeyboardMouse; //input method

    [Tooltip("index of floor/ground collider layer")]
    public int m_iGroundCollisionLayerID = 9;
    //TODO! maybe add support for groundcheck without layers being considered? would that just be checking against layer 0?

    [Header("control bindings - keyboard/mouse")]
    public KeyCode m_keyMoveForward = KeyCode.W;
    public KeyCode m_keyMoveBackward = KeyCode.S;
    public KeyCode m_keyMoveLeft = KeyCode.A;
    public KeyCode m_keyMoveRight = KeyCode.D;
    [Space]
    public KeyCode m_keyMoveJump = KeyCode.Space;
    public KeyCode m_keyMoveCrouch = KeyCode.LeftControl;
    public KeyCode m_keyMoveSprint = KeyCode.LeftShift;

    [Header("input settings - all")]
    public bool m_bInvertLookAxisY = false;             //invert Y look axis?
    [Header("input settings - controller")]
    [Tooltip("if set, ignore joystick tilt amount and force full movement speed on any input.")]
    public bool m_bControllerIgnoreInputSensitivity = false;        //use raw or snap to absolute for input axes scales?*
    [Header("input settings - keyboard/mouse")]
    [Tooltip("if set, holding opposing movement keys stops the player.")]
    public bool m_bKeyboardOpposingKeysNullifyInput = false;        //hold A, then D. do you stop (nullify) or keep going (don't nullify)?

    [Header("movement configuration")]
    public float m_fMoveSpeed = 10;                 //move speed
    [Tooltip("how fast does the player stop moving when not pressing a key? divide horizontal velocity by this value")]
    public float m_fMoveDecayDivider = 2;           //drag on no input
    [Tooltip("view rotation speed multiplier for both up/down and left/right axes.")]
    public float m_fLookMultiplierAbsolute = 1;     //mouselook multiplier
    [Tooltip("view rotation speed multiplier for left/right axis. stacks with absolute multiplier.")]
    public float m_fLookMultiplierYaw = 1;          //mouselook pitch multiplier
    [Tooltip("view rotation speed multiplier for up/down axis. stacks with absolute multiplier.")]
    public float m_fLookMultiplierPitch = 2;        //mouselook yaw multiplier

    [Space]
    [Tooltip("clearance beneath player to check for touching the ground. keep me small!")]
    public float m_fGroundCheckDistance = 0.2f;     //ground clearance
    public float m_fJumpForce = 1;                  //jump power

    [Space]
    [Tooltip("divide player's capsulecollider height by this amount when crouching")]
    public float m_fCrouchHeightDivider = 2;        //divide default capsule height by this amount when crouching
    [Tooltip("divide player's movement speed by this amount when crouching")]
    public float m_fCrouchSpeedDivider = 2;         //divide player movement by this when crouching
    [Tooltip("should the player be allowed to jump while they're crouching?")]
    public bool m_bAllowJumpWhileCrouching = true;  //allow player to jump while crouching?

    [Space]
    public float m_fSprintSpeedMultiplier = 2;          //sprint speed multiplier
    [Tooltip("can the player use their weapon while sprinting?")]
    public bool m_bAllowWeaponWhileSprinting = true;    //allow weapon use while sprinting?



    float m_fAxisMoveSide = 0;              //move input axes
    float m_fAxisMoveForward = 0;

    bool m_bIsGrounded = true;              //are we on the ground this frame?

    float m_fPlayerYaw = 0;                 //player forward angle
    float m_fPlayerPitch = 0;               //player pitch angle

    float m_fPlayerCapsuleHeight;           //player's default capsule height

    //component references
    Rigidbody m_LocalRigidbody;
    CapsuleCollider m_LocalCapsuleCollider;
    //object references
    GameObject oGroundCheck;
    GameObject oPlayerCamera;

    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError("error: deprecated script! use Ed's player movement!"); //this script is placeholder - alert!


        m_LocalRigidbody = GetComponent<Rigidbody>();               //get player object's rigidbody collider
        m_LocalCapsuleCollider = GetComponent<CapsuleCollider>();   //get player object's capsule collider
        oGroundCheck = transform.Find("GroundCheck").gameObject;    //get player groundCheck
        oPlayerCamera = transform.Find("PlayerCamera").gameObject;  //get player camera

        m_fPlayerCapsuleHeight = m_LocalCapsuleCollider.height;     //record player capsule height

        //TODO: implement controller input things!
        //TODO: lock out kbm input when on controller mode? add input method detection to hotswitch?
        if(m_eControllerMode == ControllerMode.Gamepad) { Debug.LogWarning("controller inputs aren't programmed yet - bug Ase about it!"); }
    }

    // Update is called once per frame
    void Update()
    {
        _MoveInput();
        _Move();
        _Rotate();
    }

    /// <summary>
    /// handle various input methods
    /// </summary>
    private void _MoveInput()
    {
        switch (m_eControllerMode) {    //no default - enum always HAS to be one of these

            case ControllerMode.Gamepad:        //if gamepad

                #region NESW

                //read axes
                m_fAxisMoveSide = Input.GetAxis("Horizontal");        //-1 to 1 float for strafe movement
                m_fAxisMoveForward = Input.GetAxis("Vertical");       //-1 to 1 float for fwd/back movement

                //snap inputs, if configured to do so
                if (m_bControllerIgnoreInputSensitivity)
                {
                    if (m_fAxisMoveSide > 0) { m_fAxisMoveSide = 1f; }          //if strafe input +, =1
                    if (m_fAxisMoveSide < 0) { m_fAxisMoveSide = -1f; }         //if strafe input -, =-1
                    if (m_fAxisMoveForward > 0) { m_fAxisMoveForward = 1f; }    //if fwd/back input +, =1
                    if (m_fAxisMoveForward < 0) { m_fAxisMoveForward = -1f; }   //if fwd/back input -, =-1
                }
                #endregion
                break;

            case ControllerMode.KeyboardMouse:  //if keyboard and mouse

                #region NESW
                //get input keys
                bool m_bInputKeyNorth = Input.GetKey(m_keyMoveForward);     //fwd +
                bool m_bInputKeyEast = Input.GetKey(m_keyMoveRight);        //strafe +
                bool m_bInputKeySouth = Input.GetKey(m_keyMoveBackward);    //fwd -
                bool m_bInputKeyWest = Input.GetKey(m_keyMoveLeft);         //strafe -

                /* 
                //nah actually four bools is easier
                byte m_byteInputNet = 0;
                if (Input.GetKeyDown(KeyCode.W)) { m_byteInputNet += 1; } //read W    (cardinal North)
                if (Input.GetKeyDown(KeyCode.D)) { m_byteInputNet += 2; } //read D    (cardinal East)
                if (Input.GetKeyDown(KeyCode.S)) { m_byteInputNet += 4; } //read S    (cardinal South)
                if (Input.GetKeyDown(KeyCode.A)) { m_byteInputNet += 8; } //read A    (cardinal West)
                //this is basically a bitflag system in all but the fact i'm not using bitwise operators (because they scare me)
                */

                //input tests
                //fwd/back
                var m_InputTest = _KBMInputRules(m_bInputKeyNorth, m_bInputKeySouth);
                if (m_InputTest.Item2)  //if should affect,
                {
                    m_fAxisMoveForward = m_InputTest.Item1; //write from output
                }
                //fwd/back
                m_InputTest = _KBMInputRules(m_bInputKeyEast, m_bInputKeyWest);
                if (m_InputTest.Item2)  //if should affect,
                {
                    m_fAxisMoveSide = m_InputTest.Item1; //write from output
                }
                #endregion
                break;
        }
    }

    /// <summary>
    /// takes two bools for axis +/- keys (ie. move forward/back).
    /// returns move axis (float) of -1, 0, or 1 in Item1.
    /// returns whether you should actually apply this value (bool) in Item2. this is possible depending on input nullify rules.
    /// </summary>
    /// <param name="input_bAxisPositive"></param>
    /// <param name="input_bAxisNegative"></param>
    /// <returns></returns>
    private (float, bool) _KBMInputRules(bool input_bAxisPositive, bool input_bAxisNegative)
    {
        float m_fAxisOutput = 0;
        bool m_bAffect = true;

        if (input_bAxisPositive || input_bAxisNegative)    //if any input
        {
            if (input_bAxisPositive)   //if +,
            {
                if (input_bAxisNegative)    //if BOTH pressed,
                {
                    if (m_bKeyboardOpposingKeysNullifyInput)    //if oppossing presses nullify,
                    {
                        m_fAxisOutput = 0; //dont move
                    }
                    m_bAffect = false;  //else, don't affect movement (can't return a null m_fAxisOutput, so doing this instead)
                }
                else            //if JUST + pressed
                {
                    m_fAxisOutput = 1; //move +
                }
            }
            else
            {
                if (input_bAxisNegative)    //if JUST - pressed,
                {
                    m_fAxisOutput = -1;    //move -
                }
            }
        }
        else    //else, if no input,
        {
            m_fAxisOutput = 0; //dont move
        }

        return (m_fAxisOutput, m_bAffect);
    }
    /// <summary>
    /// player movement
    /// </summary>
    private void _Move()
    {
        #region crouching
        bool m_bIsCrouching = Input.GetKey(m_keyMoveCrouch);    //store crouch input

        if (Input.GetKeyDown(m_keyMoveCrouch))  //on start crouch frame
        {
            m_LocalCapsuleCollider.height = m_fPlayerCapsuleHeight / m_fCrouchHeightDivider;    //set height to crouch height
        }
        if(Input.GetKeyUp(m_keyMoveCrouch))     //on end crouch frame
        {
            m_LocalCapsuleCollider.height = m_fPlayerCapsuleHeight; //set height to standing height
        }
        #endregion

        #region sprinting
        bool m_bIsSprinting = Input.GetKey(m_keyMoveSprint);    //store sprint input

        if (m_bIsCrouching) { m_bIsSprinting = false; }         //override if crouched
        #endregion

        #region NESW
        Vector3 m_vMoveSide = Vector3.right * m_fAxisMoveSide * m_fMoveSpeed;           //strafe speed = input scale * speed
        Vector3 m_vMoveForward = Vector3.forward * m_fAxisMoveForward * m_fMoveSpeed;   //fwd/back speed = input scale * speed

        Vector3 m_vMoveSum = m_vMoveSide + m_vMoveForward;                              //combine individual axes into one vector

        if (m_bIsCrouching) { m_vMoveSum /= m_fCrouchSpeedDivider; }                //crouching speed adjustment

        if (m_bIsSprinting) { m_vMoveSum *= m_fSprintSpeedMultiplier; }             //sprinting speed adjustment

        m_LocalRigidbody.AddRelativeForce(m_vMoveSum);                                  //affect player rigidbody

        //TODO does this need dT implemented into it somewhere?
        #endregion

        #region drag
        //quick stop on no input
        if (m_fAxisMoveSide == 0 && m_fAxisMoveForward == 0         //if no input,
            && m_bIsGrounded)                                       //and grounded
        {
            Vector3 m_vVelVector = m_LocalRigidbody.velocity;   //get vel

            //slow down horizontal vels
            m_vVelVector = new Vector3(m_vVelVector.x / m_fMoveDecayDivider, m_vVelVector.y, m_vVelVector.z / m_fMoveDecayDivider);

            m_LocalRigidbody.velocity = m_vVelVector;   //write to rigidbody
        }
        #endregion

        #region jumping
        //check if grounded
        //m_bIsGrounded = Physics.CheckSphere(oGroundCheck.transform.position, m_fGroundCheckRadius, m_iGroundCollisionLayerID); //always false! don't know why! thanks unity engine
        Ray m_rGroundCheck = new Ray(oGroundCheck.transform.position, Vector3.down);
        //m_bIsGrounded = Physics.Raycast(m_rGroundCheck, m_fGroundCheckDistance, m_iGroundCollisionLayerID) //always false also. what?
        m_bIsGrounded = !Physics.Raycast(m_rGroundCheck, m_fGroundCheckDistance);    //ver w no layer masking

        if (Input.GetKeyDown(m_keyMoveJump))    //if jumping input sent on this frame...
        {
            if (m_bIsGrounded)  //if grounded...
            {
                if(!m_bIsCrouching ||                                   //if not crouching, OR
                (m_bIsCrouching && m_bAllowJumpWhileCrouching)) {           //if crouching AND allowed to jump while crouched,

                    m_bIsGrounded = false;  //set no longer grounded

                    Vector3 m_vJumpSum = Vector3.up * m_fJumpForce; //calc jump impulse

                    m_LocalRigidbody.AddRelativeForce(m_vJumpSum);  //affect player rigidbody
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// player and camera rotations
    /// </summary>
    private void _Rotate()
    {
        //TODO! split me into _InputRotate(); !!
        //TODO! hook up controller look axes! currently only has mouse input support

        float m_fAxisMouseX = Input.GetAxis("Mouse X");                                     //float for horizontal mouse move delta
        float m_fAxisMouseY = Input.GetAxis("Mouse Y");                                     //float for vertical mouse move delta

        //invert mouse pitch
        //mouse pitch is inverted by default! so if this is FALSE, we *-1 it back to normal.
        if (!m_bInvertLookAxisY)                //if inverted Y is off,
        {
            m_fAxisMouseY = m_fAxisMouseY * -1; //apply the correction
        }

        m_fAxisMouseX = m_fAxisMouseX * m_fLookMultiplierAbsolute * m_fLookMultiplierYaw;   //apply multipliers to pitch
        m_fAxisMouseY = m_fAxisMouseY * m_fLookMultiplierAbsolute * m_fLookMultiplierPitch; //apply multipliers to yaw

        m_fPlayerYaw += m_fAxisMouseX;                                                      //offset yaw by this frame's mouse delta
        m_fPlayerPitch += m_fAxisMouseY;                                                    //offset pitch by this frame's mouse delta

        Vector3 m_vLocalRigidBodyAbsoluteAngles = m_LocalRigidbody.rotation.eulerAngles;    //get absolute rigidbody rotation
        m_vLocalRigidBodyAbsoluteAngles.y = m_fPlayerYaw;                                   //overwrite with new yaw
        m_LocalRigidbody.rotation = Quaternion.Euler(m_vLocalRigidBodyAbsoluteAngles);      //write to rigidbody

        Vector3 m_vPlayerCameraAbsoluteAngles = oPlayerCamera.transform.localRotation.eulerAngles;  //get relative camera rotation
        m_vPlayerCameraAbsoluteAngles.x = m_fPlayerPitch;                                           //overwrite with new pitch
        oPlayerCamera.transform.localRotation = Quaternion.Euler(m_vPlayerCameraAbsoluteAngles);    //write to camera
    }
}
