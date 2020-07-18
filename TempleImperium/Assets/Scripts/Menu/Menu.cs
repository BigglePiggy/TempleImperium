using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by Eddie

public class Menu : MonoBehaviour
{
    //Menu script
    //What this script does:
    /*
        - Handles showing and hiding menu pages
        - Handles options and exiting game
        - Handles scene loading
    */

    
    #region Declarations
    GameObject m_mainPage;
    GameObject m_optionsPage;
    #endregion


    //Initalization
    void Start()
    {
        m_mainPage = transform.Find("Main Page").gameObject;
        m_optionsPage = transform.Find("Options Page").gameObject;

        m_mainPage.SetActive(true);
        m_optionsPage.SetActive(false);
    }


    public void StartButton()
    {
        
    }

    public void OptionsButton()
    {
        m_mainPage.SetActive(false);
        m_optionsPage.SetActive(true);
    }

    public void ExitButton()
    {

    }

    public void BackButton()
    {
        m_mainPage.SetActive(true);
        m_optionsPage.SetActive(false);
    }
}