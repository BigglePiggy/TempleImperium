using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


//written by Ase


public class HUDController : MonoBehaviour
{
    //this script acts as a controller for various HUD elements.

    //pulling custom/arbitrary data into the HUD *has* to be done through code for now - maybe some abomination of sending EVERYTHING to this
    //script and having each data readout be a public list instead could accomplish high flexibility?
    //this probably isn't needed unless design specify they want complete control over things

        //todo: possibly rearrange me? group everything by HUD element instead of 'build strings-->write-->colour' sections


    //-----------------------------------------------------
    //generic declares
    GenericFunctions cGenericFunctions = new GenericFunctions(); //instantiate a GenericFunctions for use here

    //declarations w/ setter funcs, since this script will be sent information constantly
    #region getset declarations

    //private string m_sDebugReadout;                                   //DEPRECATED!
    //public string DebugReadout { set { m_sDebugReadout = value; } }   //DEPRECATED! build string in HUDController.Update();

    //--
    private int m_iWaveCounter;
    public int WaveCounter { set { m_iWaveCounter = value; } }
    //--
    private int m_iWaveTimerTicks;
    public int WaveTimerTicks { set { m_iWaveTimerTicks = value; } }
    //--
    private int m_iPhaseTimerTicks;
    public int PhaseTimerTicks { set { m_iPhaseTimerTicks = value; } }
    //--
    private GameLogic.StarstoneElement m_eWaveStarstoneElement;
    public GameLogic.StarstoneElement WaveStarstoneElement { set { m_eWaveStarstoneElement = value; } }
    //--
    private GameLogic.GameplayPhase m_eGameplayPhase;
    public GameLogic.GameplayPhase GameplayPhase { set { m_eGameplayPhase = value; } }
    //--
    private int m_iCurrentWeaponAmmoMagazine;
    public int CurrentWeaponAmmoMagazine { set { m_iCurrentWeaponAmmoMagazine = value; } }
    //--
    private int m_iCurrentWeaponAmmoMagazineMax;
    public int CurrentWeaponAmmoMagazineMax { set { m_iCurrentWeaponAmmoMagazineMax = value; } }
    //--
    private int m_iCurrentWeaponAmmoReserve;
    public int CurrentWeaponAmmoReserve { set { m_iCurrentWeaponAmmoReserve = value; } }
    //--
    private int m_iCurrentWeaponAmmoReserveMax;
    public int CurrentWeaponAmmoReserveMax { set { m_iCurrentWeaponAmmoReserveMax = value; } }
    //--
    private float m_fPlayerHealth;
    public float PlayerHealth { set { m_fPlayerHealth = value; } }
    //--
    private float m_fPlayerHealthMax;
    public float PlayerHealthMax { set { m_fPlayerHealthMax = value; } }
    //--
    private float m_fAbilityOffensiveCooldown;
    public float AbilityOffensiveCooldown { set { m_fAbilityOffensiveCooldown = value; } }
    //--
    private float m_fAbilityOffensiveCooldownMax;
    public float AbilityOffensiveCooldownMax { set { m_fAbilityOffensiveCooldownMax = value; } }
    //--
    private float m_fAbilityDefensiveCooldown;
    public float AbilityDefensiveCooldown { set { m_fAbilityDefensiveCooldown = value; } }
    //--
    private float m_fAbilityDefensiveCooldownMax;
    public float AbilityDefensiveCooldownMax { set { m_fAbilityDefensiveCooldownMax = value; } }
    //--


    #endregion getset declarations

    //-----------------------------------------------------
    //public vars
    [Header("HUD config")]
    public bool m_bShowDebug = false;
    [Tooltip("Start fading timer colour from default to AlertBad at this number of seconds")]
    public int m_iTimerAlertThreshold = 30;
    [Space]
    //colours
    public Color m_cTextColour = Color.white;
    public Color m_cTextColourAlertBad = Color.red;
    public Color m_cTextColourAlertGood = Color.green;
    public Color m_cColourAbilityCharge = new Color(0.6f, 0.3f, 0.1f);
    [Space]
    public Color m_cCrosshairColourDefault = Color.white;
    public Texture m_texCrosshairDefault;
    [Space]
    //fader
    public Color m_cFaderColourPause = Color.black;
    public int m_iFadeStepPause = 5;
    public Color m_cFaderColourLoss = new Color(0.5f, 0.1f, 0.15f);
    public Color m_cFaderColourWin = new Color(0.3f, 0.6f, 0.6f);
    public int m_iFadeStepGameEnd = 5;
    [Space]
    //results
    public string m_sResultsMessageWin = "Victory!";
    public string m_sResultsMessageLoss = "You have perished...";
    [Space]
    //flying text
    [Tooltip("Reference to a prefab for a Text object with attached HUDFlyingText script")]
    public GameObject oFlyingTextPrefab;
    [Tooltip("How fast will flying text leave the screen? Vertical offset begins at 1 and is multiplied by this value every frame.")]
    public float m_fFlyingTextEaseMultiplier = 1.01f;


    //-----------------------------------------------------
    //text object references (and their related private vars)
    #region output text obj refs declarations   
    [Header("HUD Elements")]
    //public GameObject oHudParent; //can't parent at this point, breaks references in other scripts
    public Text oTextDebugReadout;
    string m_sTextDebugReadout = "";
    public Image oImageCrosshair;
    public Text oTextWaveCounter;
    string m_sTextWaveCounter = "";
    public Text oTextWaveTimer;
    string m_sTextWaveTimer = "";
    public Text oTextStarstoneElement;
    string m_sTextStarstoneElement = "";
    public Text oTextAmmoMag;
    string m_sTextAmmoMag = "";
    public Text oTextAmmoMagMax;
    string m_sTextAmmoMagMax = "";
    public Text oTextAmmoReserve;
    string m_sTextAmmoReserve = "";
    public Text oTextHealth;
    string m_sTextHealth = "";
    public Text oTextHealthMax;
    string m_sTextHealthMax = "";
    public Image oImageHealthBar;
    float m_fImageHealthBarBaseWidth;
    public Text oTextAbilityOffensive;
    string m_sTextAbilityOffensive = "";
    public Image oImageAbilityOffensiveBar;
    float m_fImageAbilityOffensiveBarBaseHeight;
    public Text oTextAbilityDefensive;
    string m_sTextAbilityDefensive = "";
    public Image oImageAbilityDefensiveBar;
    float m_fImageAbilityDefensiveBarBaseHeight;
    #endregion output text obj refs declarations
    //---
    [Header("other non-menu UI objects")]
    public Image oFader;
    public Text oTextResultsMessage;
    string m_sTextResultsMessage = "";
    //-----------------------------------------------------



    void Start()
    {
        //set colour of every text element
        oTextDebugReadout.color     = m_cTextColour;
        oTextWaveCounter.color      = m_cTextColour;
        oTextWaveTimer.color        = m_cTextColour;
        oTextStarstoneElement.color = m_cTextColour;
        oTextAmmoMag.color          = m_cTextColour;
        oTextAmmoMagMax.color       = m_cTextColour;
        oTextAmmoReserve.color      = m_cTextColour;
        oTextHealth.color           = m_cTextColour;
        oTextHealthMax.color        = m_cTextColour;
        oTextAbilityOffensive.color = m_cTextColour;
        oTextAbilityDefensive.color = m_cTextColour;
        oTextResultsMessage.color   = m_cTextColour;

        //initialise text on UnityEngine.Text objects that don't get updated live
        oTextResultsMessage.text = m_sTextResultsMessage;

        //save misc things
        //health bar width
        m_fImageHealthBarBaseWidth = oImageHealthBar.rectTransform.rect.width;
        //ability offensive bar height
        m_fImageAbilityOffensiveBarBaseHeight = oImageAbilityOffensiveBar.rectTransform.rect.height;
        //ability defensive bar height
        m_fImageAbilityDefensiveBarBaseHeight = oImageAbilityDefensiveBar.rectTransform.rect.height;

        //make sure global value's crosshair material ISN'T null
        if(GlobalValues.g_settings.m_CrosshairMaterial == null)
        {
            GlobalValues.g_settings.m_CrosshairMaterial = m_texCrosshairDefault;
        }


        //reflection attempt
        //TODO make me work somehow? PropertyInfo.SetValue wants an object and i'm not sure why / how to give it a UnityEngine.Text
        //Text dummy = new Text();  //grrr let me instantiate in code you cowards
        /*
        PropertyInfo[] m_ReflectionPropertyInfos = typeof(Text).GetProperties();
        foreach (PropertyInfo m_ThisPropertyInfo in m_ReflectionPropertyInfos)
        {
            m_ThisPropertyInfo.SetValue(m_ThisPropertyInfo, m_cTextColour);
        }
        */
    }

    void Update()
    {
        //write out values every frame

        //debug readout
        if (m_bShowDebug)   //(if configured to)
        {
            m_sTextDebugReadout = (
                m_eGameplayPhase +
                "\nphase\t" + cGenericFunctions.ConvertTickstoSeconds(m_iPhaseTimerTicks).ToString() + "s\t(" + m_iPhaseTimerTicks.ToString() +
                "t)\nwave\t" + cGenericFunctions.ConvertTickstoSeconds(m_iWaveTimerTicks).ToString() + "s\t(" + m_iWaveTimerTicks.ToString() + "t)"
                );
        }
        else { m_sTextDebugReadout = ""; }

        //timer
        float m_iTimerNumber = 0;   //this exists for timer colouring (int.Parse threw TONS of errors on the full string)
        switch (m_eGameplayPhase)
        {
            //write WAVE ticker
            case GameLogic.GameplayPhase.Subwave:
            case GameLogic.GameplayPhase.InbetweenSubwave:
                m_iTimerNumber = Mathf.FloorToInt(cGenericFunctions.ConvertTickstoSeconds(m_iWaveTimerTicks)); //ticks --> seconds
                m_sTextWaveTimer = "Time: \n" + m_iTimerNumber.ToString();
                break;

            //write PHASE ticker
            case GameLogic.GameplayPhase.InbetweenWave:
            case GameLogic.GameplayPhase.PreGame:
                m_iTimerNumber = Mathf.FloorToInt(cGenericFunctions.ConvertTickstoSeconds(m_iPhaseTimerTicks)); //ticks --> seconds
                m_sTextWaveTimer = "Prepare... \n" + m_iTimerNumber.ToString(); 
                break;

            //write BLANK
            default:
                m_sTextWaveTimer = "";
                break;
        }


        //wave counter
        m_sTextWaveCounter = "Wave " + (m_iWaveCounter + 1); //(0 based)

        //wave starstone element
        m_sTextStarstoneElement = m_eWaveStarstoneElement.ToString();

        //ammo mag
        m_sTextAmmoMag = m_iCurrentWeaponAmmoMagazine.ToString();
        //ammo mag max
        m_sTextAmmoMagMax = "\\" + m_iCurrentWeaponAmmoMagazineMax;
        //ammo reserve
        m_sTextAmmoReserve = m_iCurrentWeaponAmmoReserve.ToString();

        //health
        m_sTextHealth = Mathf.FloorToInt(m_fPlayerHealth).ToString();
        //health max
        m_sTextHealthMax = m_fPlayerHealthMax + "/";

        //ability offensive
        m_sTextAbilityOffensive = Math.Max(0,Mathf.FloorToInt(m_fAbilityOffensiveCooldownMax - m_fAbilityOffensiveCooldown)+1).ToString();
        if(m_sTextAbilityOffensive == "0") { m_sTextAbilityOffensive = ""; }    //hide text if 0 (show just the underlying icon)
        //ability defensive
        m_sTextAbilityDefensive = Math.Max(0,Mathf.FloorToInt(m_fAbilityDefensiveCooldownMax - m_fAbilityDefensiveCooldown)+1).ToString();
        if(m_sTextAbilityDefensive == "0") { m_sTextAbilityDefensive = ""; }

        //don't put results text here. pointless to write every frame when it'll be hidden during gameplay, and unchanging during game end where it'll be displayed


        //WRITE --------------------------------------------------------------
        //text
        oTextDebugReadout.text          = m_sTextDebugReadout;
        oTextWaveCounter.text           = m_sTextWaveCounter;
        oTextWaveTimer.text             = m_sTextWaveTimer;
        oTextStarstoneElement.text      = m_sTextStarstoneElement;
        oTextAmmoMag.text               = m_sTextAmmoMag;
        oTextAmmoMagMax.text            = m_sTextAmmoMagMax;
        oTextAmmoReserve.text           = m_sTextAmmoReserve;
        oTextHealth.text                = m_sTextHealth;
        oTextHealthMax.text             = m_sTextHealthMax;
        oTextAbilityOffensive.text      = m_sTextAbilityOffensive;
        oTextAbilityDefensive.text      = m_sTextAbilityDefensive;


        //starstone colour
        oTextStarstoneElement.color = cGenericFunctions.GetStarstoneElementColour(m_eWaveStarstoneElement);

        //wave timer
        oTextWaveTimer.color = cGenericFunctions.LerpColor(m_cTextColourAlertBad, m_cTextColour, m_iTimerNumber, m_iTimerAlertThreshold);

        //ammo mag counter colour (lerp)
        oTextAmmoMag.color = cGenericFunctions.LerpColor(m_cTextColourAlertBad, m_cTextColour, m_iCurrentWeaponAmmoMagazine, m_iCurrentWeaponAmmoMagazineMax);
        //ammo reserve counter colour (lerp <50%, highlight max)
        oTextAmmoReserve.color = cGenericFunctions.LerpColor(m_cTextColourAlertBad, m_cTextColour, m_iCurrentWeaponAmmoReserve, m_iCurrentWeaponAmmoReserveMax / 2);
        if (m_iCurrentWeaponAmmoReserve == m_iCurrentWeaponAmmoReserveMax) { oTextAmmoReserve.color = m_cTextColourAlertGood; }

        //hp colour (lerp <50%)
        //oTextHealth.color = cGenericFunctions.LerpColor(m_cTextColourAlertBad, m_cTextColour, m_fPlayerHealth, m_fPlayerHealthMax / 2);
        //hp bar colour
        oImageHealthBar.color = cGenericFunctions.LerpColor(m_cTextColourAlertBad, m_cTextColourAlertGood, m_fPlayerHealth, m_fPlayerHealthMax);
        //hp bar width
        oImageHealthBar.rectTransform.sizeDelta = new Vector2(
            m_fImageHealthBarBaseWidth * (m_fPlayerHealth / m_fPlayerHealthMax),
            oImageHealthBar.rectTransform.rect.height
            );

        //ability offensive bar height
        oImageAbilityOffensiveBar.rectTransform.sizeDelta = new Vector2(    //uncomment the middle bit for bar to go full-->empty
            oImageAbilityOffensiveBar.rectTransform.rect.width,
            m_fImageAbilityOffensiveBarBaseHeight * ((/*m_fAbilityOffensiveCooldownMax - */m_fAbilityOffensiveCooldown) / m_fAbilityOffensiveCooldownMax)
            );
        //ability defensive bar height
        oImageAbilityDefensiveBar.rectTransform.sizeDelta = new Vector2(
            oImageAbilityDefensiveBar.rectTransform.rect.width,
            m_fImageAbilityDefensiveBarBaseHeight * ((/*m_fAbilityDefensiveCooldownMax - */m_fAbilityDefensiveCooldown) / m_fAbilityDefensiveCooldownMax)
            );


        //crosshair
        //this script's vague semblance of an organised structure is VERY swiftly falling apart
        //check if mat changed
        if(oImageCrosshair.material.mainTexture != m_texCrosshairDefault)
        {
            Debug.LogWarning("changing crosshair mat");
            oImageCrosshair.material.mainTexture = GlobalValues.g_settings.m_CrosshairMaterial;
        }
        //write colour because whatever it's cheap
        oImageCrosshair.color = GlobalValues.g_settings.m_CrosshairColor;


    }

    #region flying text
    public void FlyingTextAddTime(float input_timeAdded)
    {
        Vector3 m_vPos = oTextWaveTimer.rectTransform.position;
        m_vPos.y -= oTextWaveTimer.rectTransform.sizeDelta.y / 2;

        GameObject oNewFlyingText = Instantiate(oFlyingTextPrefab, m_vPos, Quaternion.identity, gameObject.transform);

        string m_sFlyTextContent = "+" + input_timeAdded;

        oNewFlyingText.GetComponent<HUDFlyingText>().Initialise(m_cTextColourAlertGood, m_sFlyTextContent, m_fFlyingTextEaseMultiplier);
    }


    #endregion flying text

    #region fader
    //fader handling
    public void FadeOut(bool input_instant = false)
    {
        oFader.GetComponent<HUDFader>().FadeOut(input_instant);     //fade out
        m_sTextResultsMessage = "";                                 //wipe results text
        oTextResultsMessage.text = m_sTextResultsMessage;           //write to object
    }
    public void FadeToPause(bool input_instant = false)
    {
        oFader.GetComponent<HUDFader>().ConfigureColour(m_cFaderColourPause);
        oFader.GetComponent<HUDFader>().ConfigureStep(m_iFadeStepPause);
        oFader.GetComponent<HUDFader>().FadeIn(input_instant);
    }
    public void FadeToWin(bool input_instant = false)
    {
        ShowResults(m_sResultsMessageWin);
        oFader.GetComponent<HUDFader>().ConfigureColour(m_cFaderColourWin);
        oFader.GetComponent<HUDFader>().ConfigureStep(m_iFadeStepGameEnd);
        oFader.GetComponent<HUDFader>().FadeIn(input_instant);
    }
    public void FadeToLoss(bool input_instant = false)
    {
        ShowResults(m_sResultsMessageLoss);
        oFader.GetComponent<HUDFader>().ConfigureColour(m_cFaderColourLoss);
        oFader.GetComponent<HUDFader>().ConfigureStep(m_iFadeStepGameEnd);
        oFader.GetComponent<HUDFader>().FadeIn(input_instant);
    }
    /// <summary>
    /// show the Results text and update its string.
    /// </summary>
    /// <param name="input_message"></param>
    void ShowResults(string input_message)
    {
        //build string
        m_sTextResultsMessage = (
            input_message + "\n\n\n"
            + GlobalValues.g_iLightEnemiesKilled + " Light Enemy kills\n"
            + GlobalValues.g_iMediumEnemiesKilled + " Medium Enemy kills\n"
            + GlobalValues.g_iHeavyEnemiesKilled + " Heavy Enemy kills"
            );
        //write to object
        oTextResultsMessage.text = m_sTextResultsMessage;
    }
    #endregion fader
}
