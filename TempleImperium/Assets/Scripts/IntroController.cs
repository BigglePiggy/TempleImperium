using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public float IntroLength;   
    float IntroTimer;

    SoundManager m_soundManager;
    AudioSource m_audioOrigin;

    private void Start()
    {
        m_audioOrigin = GetComponent<AudioSource>();
        m_soundManager = GameObject.FindGameObjectWithTag("Sound Manager").GetComponent<SoundManager>();
        m_audioOrigin.PlayOneShot(m_soundManager.m_introCinematic, GlobalValues.g_settings.m_fVolumeMusic); // Play MainMenu Music on menu screen **Josh
    }
    //Called Per Frame
    void Update()
    {
        if (IntroTimer < IntroLength)
        { IntroTimer += Time.deltaTime; }

        if (Input.GetMouseButtonDown(0) || IntroTimer >= IntroLength) 
        { LoadTemple(); }
    }

    private void LoadTemple() 
    {
        SceneManager.LoadScene("Temple");
    }
}
