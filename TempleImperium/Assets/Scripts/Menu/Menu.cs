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
        - Handles settings object management
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

        if(GlobalSettings.m_globalSettings == null) 
        {
            SettingsManager m_settingsManager = GameObject.Find("Settings Manager").GetComponent<SettingsManager>();
            m_settingsManager.BuildSettingsObject();
            m_settingsManager.SaveObject();
        }

        m_settings = GlobalSettings.m_globalSettings;
    }

    //Called per frame
    private void Update()
    {
        if (m_pauseMode) 
        {
            if (Input.GetKeyDown(m_settings.m_kcKeyEscape)) 
            {
                if (m_pausePage.activeInHierarchy)              //Unpause
                {   
                    ResumeButton();                 
                }

                else if(m_optionsPage.activeInHierarchy == false)   //Pause
                {
                    Time.timeScale = 0.0f;
                    Cursor.lockState = CursorLockMode.None;
                    m_pausePage.SetActive(true);
                }
            }
        }   
    }
    

    #region Button Actions
    //Main Menu 
    public void StartButton()
    {
        GlobalSettings.m_globalSettings = m_settings;
        SceneManager.LoadScene("Ase Expansion");
    }

    public void ExitButton()
    {

    }

    //Pause Menu
    public void ResumeButton()
    {
        m_pausePage.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1.0f;
    }

    public void MainMenuButton()
    {
        GameObject.Find("Settings Manager").GetComponent<SettingsManager>().SaveObject();
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Menu");
    }


    //Shared
    public void OptionsButton()
    {
        m_mainPage.SetActive(false);
        m_pausePage.SetActive(false);      
        m_optionsPage.SetActive(true);
    }

    public void IncreaseXSensitivity() 
    {
        m_settings.m_fMouseSensitivityX += 0.15f;
    }

    public void DecreaseXSensitivity()
    { 
        if (m_settings.m_fMouseSensitivityX - 0.15f < 0)
        { m_settings.m_fMouseSensitivityX = 0; }
        else 
        { m_settings.m_fMouseSensitivityX -= 0.15f; }
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