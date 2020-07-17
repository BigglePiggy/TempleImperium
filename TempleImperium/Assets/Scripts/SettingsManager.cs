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


    SettingsObject m_SettingsObject;

    /// <summary>
    /// send all referenced scripts a new SettingsObject
    /// </summary>
    /// <param name="RebuildSettingsObject">make a fresh SettingsObject? set true if settingsManager values have changed!</param>
    public void SendToScripts(bool input_RebuildSettingsObject = false)
    {
        //if no settings object is built yet, or if told to, build new settings object
        if (m_SettingsObject == null || input_RebuildSettingsObject) { BuildSettingsObject(); }

        //todo dispatch to scripts!
    }
    public void BuildSettingsObject()
    {
        m_SettingsObject = new SettingsObject(
            m_kcKeyMoveForward, m_kcKeyMoveBackward, m_kcKeyMoveLeft, m_kcKeyMoveRight, m_kcKeyJump, m_kcKeyCrouch, m_kcKeyFire, m_kcKeyAltFire,
            m_fMouseSensitivityX, m_fMouseSensitivityY, m_bMouseInvertX, m_bMouseInvertY, m_kcKeyReload, m_kcKeyMelee, m_kcKeyAbility1, m_kcKeyAbility2,
            m_kcKeyWeaponSlot1, m_kcKeyWeaponSlot2, m_kcKeyWeaponSlot3, m_kcKeyWeaponQuickSwitch, m_kcKeyEscape
            );
    }


    #region filesystem IO
    public void WriteToPrefs()
    {
        //TODO
        Debug.LogWarning("SettingsManager.WriteToPrefs() needs to be programmed! bug Ase about it");
    }
    public void ReadFromPrefs()
    {
        //TODO
        Debug.LogWarning("SettingsManager.ReadFromPrefs() needs to be programmed! bug Ase about it");
    }
    #endregion filesystem IO

    void Start()
    {
        ReadFromPrefs();
    }
    void Update()
    {
    }
}
