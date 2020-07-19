using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase


public class SettingsObject
{
    //user settings container (C# object, NOT a Unity GameObject)

    //--
    public KeyCode m_kcKeyMoveForward { get; set; }
    public KeyCode m_kcKeyMoveBackward { get; set; }
    public KeyCode m_kcKeyMoveLeft { get; set; }
    public KeyCode m_kcKeyMoveRight { get; set; }
    public KeyCode m_kcKeyJump { get; set; }
    public KeyCode m_kcKeyCrouch { get; set; }
    public KeyCode m_kcKeySprint { get; set; }

    public KeyCode m_kcKeyFire { get; set; }
    public KeyCode m_kcKeyAltFire { get; set; }
    public float m_fMouseSensitivityX { get; set; }
    public float m_fMouseSensitivityY { get; set; }
    public bool m_bMouseInvertX { get; set; }
    public bool m_bMouseInvertY { get; set; }

    public KeyCode m_kcKeyReload { get; set; }
    public KeyCode m_kcKeyMelee { get; set; }
    public KeyCode m_kcKeyAbility1 { get; set; }
    public KeyCode m_kcKeyAbility2 { get; set; }
    public KeyCode m_kcKeyWeaponSlot1 { get; set; }
    public KeyCode m_kcKeyWeaponSlot2 { get; set; }
    public KeyCode m_kcKeyWeaponSlot3 { get; set; }
    public KeyCode m_kcKeyWeaponQuickSwitch { get; set; }

    public KeyCode m_kcKeyEscape { get; set; }
    //--
    public float m_fVolumeOverall { get; set; }
    public float m_fVolumeSFX { get; set; }
    public float m_fVolumeMusic { get; set; }
    public float m_fVolumeUI { get; set; }
    //--

    public SettingsObject(KeyCode input_KeyMoveForward, KeyCode input_KeyMoveBackward, KeyCode input_KeyMoveLeft, KeyCode input_KeyMoveRight,
        KeyCode input_KeyJump, KeyCode input_KeyCrouch, KeyCode input_KeySprint, KeyCode input_KeyFire, KeyCode input_KeyAltFire, float input_MouseSensitivityX, float input_MouseSensitivityY,
        bool input_MouseInvertX, bool input_MouseInvertY, KeyCode input_KeyReload, KeyCode input_KeyMelee, KeyCode input_KeyAbility1, KeyCode input_KeyAbility2,
        KeyCode input_KeyWeaponSlot1, KeyCode input_KeyWeaponSlot2, KeyCode input_KeyWeaponSlot3, KeyCode input_KeyWeaponQuickSwitch,
        KeyCode input_KeyEscape, float input_VolumeOverall, float input_VolumeSFX, float input_VolumeMusic, float input_VolumeUI)
    {
        m_kcKeyMoveForward              = input_KeyMoveForward;
        m_kcKeyMoveBackward             = input_KeyMoveBackward;
        m_kcKeyMoveLeft                 = input_KeyMoveLeft;
        m_kcKeyMoveRight                = input_KeyMoveRight;
        m_kcKeyJump                     = input_KeyJump;
        m_kcKeyCrouch                   = input_KeyCrouch;
        m_kcKeySprint                   = input_KeySprint;
        m_kcKeyFire                     = input_KeyFire;
        m_kcKeyAltFire                  = input_KeyAltFire;
        m_fMouseSensitivityX            = input_MouseSensitivityX;
        m_fMouseSensitivityY            = input_MouseSensitivityY;
        m_bMouseInvertX                 = input_MouseInvertX;
        m_bMouseInvertY                 = input_MouseInvertY;
        m_kcKeyReload                   = input_KeyReload;
        m_kcKeyMelee                    = input_KeyMelee;
        m_kcKeyAbility1                 = input_KeyAbility1;
        m_kcKeyAbility2                 = input_KeyAbility2;
        m_kcKeyWeaponSlot1              = input_KeyWeaponSlot1;
        m_kcKeyWeaponSlot2              = input_KeyWeaponSlot2;
        m_kcKeyWeaponSlot3              = input_KeyWeaponSlot3;
        m_kcKeyWeaponQuickSwitch        = input_KeyWeaponQuickSwitch;
        m_kcKeyEscape                   = input_KeyEscape;
        m_fVolumeOverall                = input_VolumeOverall;
        m_fVolumeSFX                    = input_VolumeSFX;
        m_fVolumeMusic                  = input_VolumeMusic;
        m_fVolumeUI                     = input_VolumeUI;
    }

#if UNITY_EDITOR
    void Start()
    {
        Debug.LogError("SettingsObject script (somehow) attached to a Unity GameObject! this is a C# object class! Use SettingsManager instead!");
    }
#endif
}
