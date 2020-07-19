using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase


public static class GlobalSettings
{
    public static KeyCode m_kcKeyMoveForward = KeyCode.W;
    public static KeyCode m_kcKeyMoveBackward = KeyCode.S;
    public static KeyCode m_kcKeyMoveLeft = KeyCode.A;
    public static KeyCode m_kcKeyMoveRight = KeyCode.D;
    public static KeyCode m_kcKeyJump = KeyCode.Space;
    public static KeyCode m_kcKeyCrouch = KeyCode.LeftControl;
    public static KeyCode m_kcKeySprint = KeyCode.LeftShift;

    public static KeyCode m_kcKeyFire = KeyCode.Mouse0;
    public static KeyCode m_kcKeyAltFire = KeyCode.Mouse1;
    public static float m_fMouseSensitivityX = 2.5f;
    public static float m_fMouseSensitivityY = 2.5f;
    public static bool m_bMouseInvertX = false;
    public static bool m_bMouseInvertY = false;

    public static KeyCode m_kcKeyReload = KeyCode.R;
    public static KeyCode m_kcKeyMelee = KeyCode.V;
    public static KeyCode m_kcKeyAbility1 = KeyCode.E;
    public static KeyCode m_kcKeyAbility2 = KeyCode.F;
    public static KeyCode m_kcKeyWeaponSlot1 = KeyCode.Alpha1;
    public static KeyCode m_kcKeyWeaponSlot2 = KeyCode.Alpha2;
    public static KeyCode m_kcKeyWeaponSlot3 = KeyCode.Alpha3;
    public static KeyCode m_kcKeyWeaponQuickSwitch = KeyCode.Q;

    public static KeyCode m_kcKeyEscape = KeyCode.Escape;

    public static float m_fVolumeOverall = 0.5f;       //unity uses 0-1 for vol control. careful with float rounding errors if doing +-0.1.
    public static float m_fVolumeSFX = 0.5f;           //(briefly multiplying by 10 to work in integer space instead works around this)
    public static float m_fVolumeMusic = 0.5f;
    public static float m_fVolumeUI = 0.5f;

    //Value changing
    public static void ChangeKeySetting(KeyCode newKey, string feildName)
    {
        FieldInfo[] fields = typeof(GlobalSettings).GetFields();

        switch (feildName)
        {
            case "m_kcKeyMoveForward":
                m_kcKeyMoveForward = newKey;
                break;

            case "m_kcKeyMoveBackward":
                m_kcKeyMoveBackward = newKey;
                break;

            case "m_kcKeyMoveLeft":
                m_kcKeyMoveLeft = newKey;
                break;

            case "m_kcKeyMoveRight":
                m_kcKeyMoveRight = newKey;
                break;

            case "m_kcKeyJump":
                m_kcKeyJump = newKey;
                break;

            case "m_kcKeyCrouch":
                m_kcKeyCrouch = newKey;
                break;

            case "m_kcKeySprint":
                m_kcKeySprint = newKey;
                break;

            case "m_kcKeyFire":
                m_kcKeyFire = newKey;
                break;

            case "m_kcKeyAltFire":
                m_kcKeyAltFire = newKey;
                break;

            case "m_kcKeyReload":
                m_kcKeyReload = newKey;
                break;

            case "m_kcKeyMelee":
                m_kcKeyMelee = newKey;
                break;

            case "m_kcKeyAbility1":
                m_kcKeyAbility1 = newKey;
                break;

            case "m_kcKeyAbility2":
                m_kcKeyAbility2 = newKey;
                break;
        }
    }
}
