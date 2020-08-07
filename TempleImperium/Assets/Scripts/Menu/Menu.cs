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

    HUDController m_gameHUD;
    SoundManager m_soundManager;
    AudioSource m_audioOrigin;

    //Key change settings
    bool m_keyCaptureMode;
    Text m_keyCaptureText;

    //Text
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
        m_audioOrigin = GetComponent<AudioSource>();

        //Assigns all default values to options text
        m_optionsPage.SetActive(true);
        GameObject[] settingsTexts = GameObject.FindGameObjectsWithTag("SettingsText");
        for (int i = 0; i < settingsTexts.Length; i++)
        {
            settingsTexts[i].GetComponent<Text>().text = typeof(SettingsObject).GetField(settingsTexts[i].name).GetValue(GlobalValues.g_settings).ToString();
        }
        m_optionsPage.SetActive(false);


        if (m_pauseMode)
        {
            m_mainPage.SetActive(false);
            m_pausePage.SetActive(false);
            m_optionsPage.SetActive(false);
            m_gameHUD = GameObject.Find("HUD").GetComponent<HUDController>();
        }

        else
        {
            m_mainPage.SetActive(true);
            m_pausePage.SetActive(false);
            m_optionsPage.SetActive(false);
        }

        m_ySensitivityText = transform.Find("Options Page").Find("Sensitivity").Find("Y").Find("Y Sensitivity Text").GetComponent<Text>();
        m_xSensitivityText = transform.Find("Options Page").Find("Sensitivity").Find("X").Find("X Sensitivity Text").GetComponent<Text>();

        m_ySensitivityText.text = GlobalValues.g_settings.m_fMouseSensitivityY.ToString();
        m_xSensitivityText.text = GlobalValues.g_settings.m_fMouseSensitivityX.ToString();

        m_soundManager = GameObject.FindGameObjectWithTag("Sound Manager").GetComponent<SoundManager>();
    }

    //Called per frame
    private void Update()
    {
        if (m_pauseMode) 
        {
            if (Input.GetKeyDown(GlobalValues.g_settings.m_kcKeyEscape)) 
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
                    m_gameHUD.FadeToPause();
                }
            }
        }
    }


    #region Main Menu Actions
    public void StartButton()
    {
        SceneManager.LoadScene("Temple");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
    #endregion

    #region Pause Menu Actions
    public void ResumeButton()
    {
        m_pausePage.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1.0f;
        m_gameHUD.FadeOut();
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Menu");
        m_audioOrigin.PlayOneShot(m_soundManager.m_pressOne);
    }
    #endregion

    #region Shared Actions
    public void OptionsButton()
    {
        m_mainPage.SetActive(false);
        m_pausePage.SetActive(false);      
        m_optionsPage.SetActive(true);
        m_audioOrigin.PlayOneShot(m_soundManager.m_pressOne);
    }

    public void ChangeYSensitivity(float change)
    {
        if (m_keyCaptureMode == false)
        {
            if (change >= 0)
            {
                GlobalValues.g_settings.m_fMouseSensitivityY += change;
            }

            else
            {
                if (GlobalValues.g_settings.m_fMouseSensitivityY + change < 0)
                { GlobalValues.g_settings.m_fMouseSensitivityY = 0; }
                else
                { GlobalValues.g_settings.m_fMouseSensitivityY += change; }
            }

            m_ySensitivityText.text = GlobalValues.g_settings.m_fMouseSensitivityY.ToString();
            m_audioOrigin.PlayOneShot(m_soundManager.m_pressTwo);
        }
    }

    public void ChangeXSensitivity(float change) 
    {
        if (m_keyCaptureMode == false)
        {
            if (change >= 0)
            {
                GlobalValues.g_settings.m_fMouseSensitivityX += change;
            }

            else
            {
                if (GlobalValues.g_settings.m_fMouseSensitivityX + change < 0)
                { GlobalValues.g_settings.m_fMouseSensitivityX = 0; }
                else
                { GlobalValues.g_settings.m_fMouseSensitivityX += change; }
            }

            m_xSensitivityText.text = GlobalValues.g_settings.m_fMouseSensitivityX.ToString();
            m_audioOrigin.PlayOneShot(m_soundManager.m_pressTwo);
        }
    }

    public void SettingKeyChange(Text text) 
    {
        if (m_keyCaptureMode == false)
        {
            m_keyCaptureMode = true;
            m_keyCaptureText = text;
        }
        m_audioOrigin.PlayOneShot(m_soundManager.m_pressTwo);
    }
   
    private void OnGUI()    //On GUI event - Captures keys for settings
    {
        if (m_keyCaptureMode)
        {
            Event e = Event.current;
            
            if (e.isKey)
            {
                FieldInfo[] feilds = typeof(SettingsObject).GetFields();

                foreach (FieldInfo feild in feilds)
                {
                    if(feild.Name == m_keyCaptureText.gameObject.name) 
                    {
                        feild.SetValue(GlobalValues.g_settings, e.keyCode);
                    }
                }

                m_keyCaptureMode = false;
                m_keyCaptureText.text = e.keyCode.ToString();
            }

            if (e.isMouse)
            {
                if (e.button == 0)
                {
                    FieldInfo[] feilds = typeof(SettingsObject).GetFields();

                    foreach (FieldInfo feild in feilds)
                    {
                        if (feild.Name == m_keyCaptureText.gameObject.name)
                        {
                            feild.SetValue(GlobalValues.g_settings, KeyCode.Mouse0);
                        }
                    }

                    m_keyCaptureMode = false;
                    m_keyCaptureText.text = KeyCode.Mouse0.ToString();
                }

                if (e.button == 1)
                {
                    FieldInfo[] feilds = typeof(SettingsObject).GetFields();

                    foreach (FieldInfo feild in feilds)
                    {
                        if (feild.Name == m_keyCaptureText.gameObject.name)
                        {
                            feild.SetValue(GlobalValues.g_settings, KeyCode.Mouse1);
                        }
                    }

                    m_keyCaptureMode = false;
                    m_keyCaptureText.text = KeyCode.Mouse1.ToString();
                }
            }

            if (e.shift == true)
            {
                FieldInfo[] feilds = typeof(SettingsObject).GetFields();

                foreach (FieldInfo feild in feilds)
                {
                    if (feild.Name == m_keyCaptureText.gameObject.name)
                    {
                        feild.SetValue(GlobalValues.g_settings, KeyCode.LeftShift);
                    }
                }

                m_keyCaptureMode = false;
                m_keyCaptureText.text = KeyCode.LeftShift.ToString();
            }
        }
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

            m_audioOrigin.PlayOneShot(m_soundManager.m_pressOne);
        }
    }
    #endregion   
}