using System.Reflection;
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

    //Key change settings
    bool m_keyCaptureMode;
    PropertyInfo m_keyCaptureProperty;
    Text m_keyCaptureText;

    Text m_ySensitivityText;
    Text m_xSensitivityText;
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

        if(GlobalValues.g_settings == null) 
        {
            SettingsManager m_settingsManager = GameObject.Find("Settings Manager").GetComponent<SettingsManager>();
            m_settingsManager.BuildSettingsObject();
            m_settingsManager.SaveObject();
        }

        m_settings = GlobalValues.g_settings;

        m_ySensitivityText = transform.Find("Options Page").Find("Sensitivity").Find("Y").Find("Y Sensitivity Text").GetComponent<Text>();
        m_xSensitivityText = transform.Find("Options Page").Find("Sensitivity").Find("X").Find("X Sensitivity Text").GetComponent<Text>();

        m_ySensitivityText.text = m_settings.m_fMouseSensitivityY.ToString();
        m_xSensitivityText.text = m_settings.m_fMouseSensitivityX.ToString();
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
        GlobalValues.g_settings = m_settings;
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
        if (m_keyCaptureMode == false)
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
    }

    public void ChangeXSensitivity(float change) 
    {
        if (m_keyCaptureMode == false)
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
    }

    public void SettingKeyChange(Text text) 
    {
        if (m_keyCaptureMode == false)
        {
            m_keyCaptureMode = true;
            m_keyCaptureProperty = GetSettingsProperty(text.gameObject.name);
            m_keyCaptureText = text;
        }
    }
   
    private void OnGUI()    //On GUI event - Captures keys for settings
    {
        if (m_keyCaptureMode)
        {
            Event e = Event.current;
            
            if (e.isKey)
            {
                m_keyCaptureProperty.SetValue(m_settings, e.keyCode); 
                
                m_keyCaptureMode = false;
                m_keyCaptureText.text = m_keyCaptureProperty.GetValue(m_settings).ToString();
            }

            if (e.isMouse)
            {
                if (e.button == 0)
                { m_keyCaptureProperty.SetValue(m_settings, KeyCode.Mouse0); }

                if (e.button == 1)
                { m_keyCaptureProperty.SetValue(m_settings, KeyCode.Mouse1); }

                m_keyCaptureMode = false;
                m_keyCaptureText.text = m_keyCaptureProperty.GetValue(m_settings).ToString();
            }

            if (e.shift == true)
            {
                m_keyCaptureProperty.SetValue(m_settings, KeyCode.LeftShift);

                m_keyCaptureMode = false;
                m_keyCaptureText.text = m_keyCaptureProperty.GetValue(m_settings).ToString();
            }
        }
    }

    private PropertyInfo GetSettingsProperty(string name) 
    {
        PropertyInfo[] settingsProperties = m_settings.GetType().GetProperties();

        for (int i = 0; i < settingsProperties.Length; i++)
        {
            if(settingsProperties[i].Name == name) 
            {
                return settingsProperties[i];
            }
        }

        return null;
    }

    public void BackButton()
    {
        if (m_keyCaptureMode == false)
        {
            if (m_pauseMode)
            { m_pausePage.SetActive(true); }
            else
            { m_mainPage.SetActive(true); }

            m_optionsPage.SetActive(false);
        }
    }
    #endregion   
}