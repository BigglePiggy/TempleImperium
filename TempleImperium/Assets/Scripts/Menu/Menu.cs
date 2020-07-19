using UnityEngine;
using UnityEngine.UI;
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

    bool m_keyCaptureMode;
    string m_keyCaptureTarget;

    Text m_ySensitivityText;
    Text m_xSensitivityText;
    Text m_weaponFireText;
    #endregion


    //Initalization
    void Start()
    {
        m_keyCaptureMode = false;

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

        m_ySensitivityText = transform.Find("Options Page").Find("Sensitivity").Find("Y").Find("Y Sensitivity Text").GetComponent<Text>();
        m_xSensitivityText = transform.Find("Options Page").Find("Sensitivity").Find("X").Find("X Sensitivity Text").GetComponent<Text>();
        m_weaponFireText = transform.Find("Options Page").Find("Weapon Use").Find("Weapon Fire Text").GetComponent<Text>();

        m_ySensitivityText.text = m_settings.m_fMouseSensitivityY.ToString();
        m_xSensitivityText.text = m_settings.m_fMouseSensitivityX.ToString();
        m_weaponFireText.text = m_settings.m_kcKeyFire.ToString();
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
    

    #region Main Menu Actions
    public void StartButton()
    {
        GlobalSettings.m_globalSettings = m_settings;
        SceneManager.LoadScene("Ase Expansion");
    }

    public void ExitButton()
    {

    }
    #endregion

    #region Pause Menu Actions
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
    #endregion

    #region Shared Actions
    public void OptionsButton()
    {
        m_mainPage.SetActive(false);
        m_pausePage.SetActive(false);      
        m_optionsPage.SetActive(true);
    }

    public void ChangeYSensitivity(float change)
    {
        if (change >= 0)
        {
            m_settings.m_fMouseSensitivityY += change;
        }

        else
        {
            if (m_settings.m_fMouseSensitivityY + change < 0)
            { m_settings.m_fMouseSensitivityY = 0; }
            else
            { m_settings.m_fMouseSensitivityY += change; }
        }

        m_ySensitivityText.text = m_settings.m_fMouseSensitivityY.ToString();
    }

    public void ChangeXSensitivity(float change) 
    {
        if (change >= 0)
        {
            m_settings.m_fMouseSensitivityX += change;
        }

        else
        {
            if (m_settings.m_fMouseSensitivityX + change < 0)
            { m_settings.m_fMouseSensitivityX = 0; }
            else
            { m_settings.m_fMouseSensitivityX += change; }
        }

        m_xSensitivityText.text = m_settings.m_fMouseSensitivityX.ToString();
    }

    public void WeaponUseKeyChange() 
    {
        m_keyCaptureMode = true;
        m_keyCaptureTarget = "m_kcKeyFire";
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

    private void OnGUI()
    {
        if (m_keyCaptureMode)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                if(m_keyCaptureTarget == "m_kcKeyFire") 
                {
                    m_settings.m_kcKeyFire = e.keyCode;
                    m_weaponFireText.text = m_settings.m_kcKeyFire.ToString();
                }

                m_keyCaptureMode = false;
            }
        }
    }
}