using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase


public class SettingsManager : MonoBehaviour
{
    //stores user settings, handles PlayerPrefs interaction, and pushes to scripts.

    [Header("Default Controls")]

    public KeyCode m_kcKeyMoveForward       = KeyCode.W;
    public KeyCode m_kcKeyMoveBackward      = KeyCode.S;
    public KeyCode m_kcKeyMoveLeft          = KeyCode.A;
    public KeyCode m_kcKeyMoveRight         = KeyCode.D;
    public KeyCode m_kcKeyJump              = KeyCode.Space;
    public KeyCode m_kcKeyCrouch            = KeyCode.LeftControl;
    [Space]
    public KeyCode m_kcKeyFire              = KeyCode.Mouse0;
    public KeyCode m_kcKeyAltFire           = KeyCode.Mouse1;
    public float m_fMouseSensitivityX       = 5;
    public float m_fMouseSensitivityY       = 5;
    public bool m_bMouseInvertX             = false;
    public bool m_bMouseInvertY             = false;
    [Space]
    public KeyCode m_kcKeyReload            = KeyCode.R;
    public KeyCode m_kcKeyMelee             = KeyCode.V;
    public KeyCode m_kcKeyAbility1          = KeyCode.E;
    public KeyCode m_kcKeyAbility2          = KeyCode.F;
    public KeyCode m_kcKeyWeaponSlot1       = KeyCode.Alpha1;
    public KeyCode m_kcKeyWeaponSlot2       = KeyCode.Alpha2;
    public KeyCode m_kcKeyWeaponSlot3       = KeyCode.Alpha3;
    public KeyCode m_kcKeyWeaponQuickSwitch = KeyCode.Q;
    [Space]
    public KeyCode m_kcKeyEscape            = KeyCode.Escape;

    [Header("Default Configs")]
    public float m_fVolumeOverall           = 0.5f;         //unity uses 0-1 for vol control. careful with float rounding errors if doing +-0.1.
    public float m_fVolumeSFX               = 0.5f;         //(briefly multiplying by 10 to work in integer space instead works around this)
    public float m_fVolumeMusic             = 0.5f;
    public float m_fVolumeUI                = 0.5f;

    [Header("State")]
    public bool onMenu;

    SettingsObject m_SettingsObject;


    void Start()
    {
        if (onMenu == false)
        {
            m_SettingsObject = GlobalValues.g_settings; //Reads global settings (From the menu)
            SendToScripts();    //Push to scripts
        }
    }


    /// <summary>
    /// Send all referenced scripts a new SettingsObject
    /// </summary>
    /// <param name="input_RebuildSettingsObject">Make a fresh SettingsObject. Set true if settingsManager values have changed!</param>
    public void SendToScripts(bool input_RebuildSettingsObject = false)
    {
        //if no settings object is built yet, or if told to, build new settings object
        if (m_SettingsObject == null || input_RebuildSettingsObject) { BuildSettingsObject(); }

        //todo dispatch to scripts!
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Settings = m_SettingsObject;
        GameObject.FindGameObjectWithTag("Player").transform.Find("Player Camera").Find("Primary Gun").GetComponent<PlayerGun>().Settings = m_SettingsObject;
        GameObject.FindGameObjectWithTag("Player").transform.Find("Player Camera").Find("Secondary Gun").GetComponent<PlayerGun>().Settings = m_SettingsObject;
        GameObject.FindGameObjectWithTag("Menu").GetComponent<Menu>().Settings = m_SettingsObject;
    }

    public void BuildSettingsObject()
    {   
        m_SettingsObject = new SettingsObject(
        m_kcKeyMoveForward, m_kcKeyMoveBackward, m_kcKeyMoveLeft, m_kcKeyMoveRight, m_kcKeyJump, m_kcKeyCrouch, m_kcKeyFire, m_kcKeyAltFire,
        m_fMouseSensitivityX, m_fMouseSensitivityY, m_bMouseInvertX, m_bMouseInvertY, m_kcKeyReload, m_kcKeyMelee, m_kcKeyAbility1, m_kcKeyAbility2,
        m_kcKeyWeaponSlot1, m_kcKeyWeaponSlot2, m_kcKeyWeaponSlot3, m_kcKeyWeaponQuickSwitch, m_kcKeyEscape,
        m_fVolumeOverall, m_fVolumeSFX, m_fVolumeMusic, m_fVolumeUI
        );
    }

    public void SaveObject() 
    {
        GlobalValues.g_settings = m_SettingsObject; //Writes global settings (For the menu)
    }
}
