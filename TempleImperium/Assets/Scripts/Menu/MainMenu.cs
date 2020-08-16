using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    SoundManager m_soundManager;
    AudioSource m_audioOrigin;

    // This class is a temporary holding to play main menu  music on start during the main menu scene **Josh

    void Start()
    {
        m_audioOrigin = GetComponent<AudioSource>();
        m_soundManager = GameObject.FindGameObjectWithTag("Sound Manager").GetComponent<SoundManager>();
        m_audioOrigin.PlayOneShot(m_soundManager.m_menuMusic, GlobalValues.g_settings.m_fVolumeMusic); // Play MainMenu Music on menu screen **Josh
    }

    void Update()
    {
        
    }
}
