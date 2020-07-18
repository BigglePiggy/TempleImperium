using UnityEngine;
using UnityEngine.SceneManagement;

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
    public bool m_pauseMode;

    GameObject m_mainPage;
    GameObject m_pausePage;
    GameObject m_optionsPage;

    private SettingsObject m_settings;  //Settings object used to determine all input keys
    public SettingsObject Settings { set { m_settings = value; } }  //Setter for m_settings - used by SettingsManager
    #endregion


    //Initalization
    void Start()
    {
        m_mainPage = transform.Find("Main Page").gameObject;
        m_pausePage = transform.Find("Pause Page").gameObject;
        m_optionsPage = transform.Find("Options Page").gameObject;

        if (m_pauseMode)
        {
            m_mainPage.SetActive(false);
            m_pausePage.SetActive(false);
            m_optionsPage.SetActive(false);
        }

        else 
        {
            m_mainPage.SetActive(true);
            m_pausePage.SetActive(false);
            m_optionsPage.SetActive(false);
        }
    }

    //Called per frame
    private void Update()
    {
        if (m_pauseMode) 
        {
            if (Input.GetKeyDown(m_settings.m_kcKeyEscape)) 
            {
                if (m_pausePage.activeInHierarchy) 
                {
                    ResumeButton();
                    Cursor.lockState = CursorLockMode.Locked;   //Locks the mouse
                }

                else 
                {
                    Cursor.lockState = CursorLockMode.None;   //Unlocks the mouse
                    m_pausePage.SetActive(true);
                }
            }
        }   
    }
    

    #region Button Actions
    //Main Menu 
    public void StartButton()
    {
        SceneManager.LoadScene("Ase Expansion");
    }

    public void ExitButton()
    {

    }


    //Pause Menu
    public void ResumeButton()
    {
        m_pausePage.SetActive(false);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("Menu");
    }


    //Shared
    public void OptionsButton()
    {
        m_mainPage.SetActive(false);
        m_pausePage.SetActive(false);      
        m_optionsPage.SetActive(true);
    }

    public void BackButton()
    {
        if (m_pauseMode)
        { m_pausePage.SetActive(true); }
        else 
        { m_mainPage.SetActive(true); }

        m_optionsPage.SetActive(false);
    }
    #endregion
}