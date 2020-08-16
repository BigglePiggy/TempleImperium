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

    [Header("Wave Music")]
    public AudioClip m_waveOneToThree;
    
}
