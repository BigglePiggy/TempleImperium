using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Created by Ase and Eddie

public class SettingsObject
{
    //C# object to package player config into one place

    //movement
    public KeyCode m_kcKeyMoveForward           = KeyCode.W;
    public KeyCode m_kcKeyMoveBackward          = KeyCode.S;
    public KeyCode m_kcKeyMoveLeft              = KeyCode.A;
    public KeyCode m_kcKeyMoveRight             = KeyCode.D;
    public KeyCode m_kcKeyJump                  = KeyCode.Space;
    public KeyCode m_kcKeyCrouch                = KeyCode.LeftControl;
    public KeyCode m_kcKeySprint                = KeyCode.LeftShift;

    //camera
    public KeyCode m_kcKeyFire                  = KeyCode.Mouse0;
    public KeyCode m_kcKeyAltFire               = KeyCode.Mouse1;
    public float m_fMouseSensitivityX           = 1.3f;
    public float m_fMouseSensitivityY           = 1.3f;
    public bool m_bMouseInvertX                 = false;
    public bool m_bMouseInvertY                 = false;

    //gameplay
    public KeyCode m_kcKeyReload = KeyCode.R;
    public KeyCode m_kcKeyMelee = KeyCode.V;
    public KeyCode m_kcKeyAbility1 = KeyCode.E;
    public KeyCode m_kcKeyAbility2 = KeyCode.F;
    public KeyCode m_kcKeyWeaponSlot1 = KeyCode.Alpha1;
    public KeyCode m_kcKeyWeaponSlot2 = KeyCode.Alpha2;
    public KeyCode m_kcKeyWeaponSlot3 = KeyCode.Alpha3;
    public KeyCode m_kcKeyWeaponQuickSwitch = KeyCode.Q;

    //meta
    public KeyCode m_kcKeyEscape = KeyCode.Escape;

    //game config
    public float m_fVolumeOverall = 0.5f;       //unity uses 0-1 for vol control. careful with float rounding errors if doing +-0.1.
    public float m_fVolumeGuns = 0.055f;           //(briefly multiplying by 10 to work in integer space instead works around this)
    public float m_fVolumeEnemies = 0.065f;           //(briefly multiplying by 10 to work in integer space instead works around this)
    public float m_fVolumeSFX = 0.045f;           //(briefly multiplying by 10 to work in integer space instead works around this)
    public float m_fVolumeMusic = 0.5f;
    public float m_fVolumeUI = 0.15f;

    //Crosshair
    public Color m_CrosshairColor = Color.white;
    public Texture m_CrosshairMaterial;
}
