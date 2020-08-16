using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by Eddie

public class SoundManager : MonoBehaviour
{
    [Header("Guns")]
    public AudioClip m_primaryGunEquip;
    public AudioClip m_primaryGunFire;
    public AudioClip m_primaryGunDryFire;
    public AudioClip m_primaryGunReload;
    [Space]
    public AudioClip m_secondaryGunEquip;
    public AudioClip m_secondaryGunFire;
    public AudioClip m_secondaryGunDryFire;
    public AudioClip m_secondaryGunReload;

    [Header("Player")]
    public AudioClip m_playerJump;
    public AudioClip m_playerLand;
    public AudioClip m_playerFootstep;
    public AudioClip m_playerDamaged;
    public AudioClip m_playerDeath;
    public AudioClip m_playerLaunchPad;
    public AudioClip m_playerAmmoBox;

    [Header("Menu")]
    public AudioClip m_pressOne;
    public AudioClip m_pressTwo;
    public AudioClip m_pressThree;
    public AudioClip m_menuMusic;

    [Header("Cinematic Music")]
    public AudioClip m_introCinematic;
    public AudioClip m_outroCinematic;

    [Header("Enemies")]
    public AudioClip m_lightEnemyAttack;
    public AudioClip m_lightEnemyDamaged;
    public AudioClip m_lightEnemyDeath;
    public AudioClip m_lightEnemyHitGround;
    public AudioClip m_lightEnemyMovement;
    [Space]
    public AudioClip m_mediumEnemyShot;
    public AudioClip m_mediumEnemyReload;
    public AudioClip m_mediumEnemyDamaged;
    public AudioClip m_mediumEnemyDeath;
    [Space]
    public AudioClip m_heavyEnemyShot;
    public AudioClip m_heavyEnemyDamaged;
    public AudioClip m_heavyEnemyDeath;

    [Header("Wave 1 - 3")]
    public AudioClip m_waveOneToThreeMain;
    public AudioClip m_waveOneToThreeAlarm;
    public AudioClip m_waveOneToThreeDanger;

    [Header("Wave 4 - 5")]
    public AudioClip m_waveFourToSixMain;
    public AudioClip m_waveFourToSixAlarm;
    public AudioClip m_waveFourToSixDanger;

    [Header("Wave 7")]
    public AudioClip m_waveSevenMain;
    public AudioClip m_waveSevenAlarm;
    public AudioClip m_waveSevenDanger;

    [Header("Wave Rest")]
    public AudioClip m_waveRest;

}
