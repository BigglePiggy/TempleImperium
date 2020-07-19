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


    //-----------------------------------------------------
    //generic declares
    GenericFunctions cGenericFunctions = new GenericFunctions(); //instantiate a GenericFunctions for use here

    //declarations w/ setter funcs, since this script will be sent information constantly
    #region getset declarations
    /*      template
        private string m_sPrivateVar;
        public string PublicVar { set { m_sPrivateVar = value; } }
        public Text oTextObject;
    */

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


    #endregion getset declarations

    //-----------------------------------------------------
    //settings vars
    public bool m_bShowDebug = false;
    [Tooltip("Start fading timer colour from default to AlertBad at this number of seconds")]
    public int m_iTimerAlertThreshold = 30;
    [Space]
    //colours
    public Color m_cTextColour = Color.white;
    public Color m_cTextColourAlertBad = Color.red;
    public Color m_cTextColourAlertGood = Color.green;

    //-----------------------------------------------------
    //text object references (and their private output strings)
    #region output text obj refs declarations   
    [Header("HUD Elements")]
    public Text oTextDebugReadout;
    string m_sTextDebugReadout = "";
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
    public Text oTextAbilityDefensive;
    string m_sTextAbilityDefensive = "";
    #endregion output text obj refs declarations
    //-----------------------------------------------------



    void Start()
    {
        //set colour of every element

        oTextDebugReadout.color     = m_cTextColour;
        oTextWaveCounter.color      = m_cTextColour;
        oTextWaveTimer.color        = m_cTextColour;
        oTextStarstoneElement.color = m_cTextColour;
        oTextAmmoMag.color          = m_cTextColour;
        oTextAmmoMagMax.color       = m_cTextColour;
        oTextAmmoReserve.color      = m_cTextColour;

        //save misc things
        //health bar width
        m_fImageHealthBarBaseWidth = oImageHealthBar.rectTransform.rect.width;


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
        oTextHealth.color = cGenericFunctions.LerpColor(m_cTextColourAlertBad, m_cTextColour, m_fPlayerHealth, m_fPlayerHealthMax / 2);
        //hp bar colour
        oImageHealthBar.color = cGenericFunctions.LerpColor(m_cTextColourAlertBad, m_cTextColourAlertGood, m_fPlayerHealth, m_fPlayerHealthMax);
        //hp bar width
        oImageHealthBar.rectTransform.sizeDelta = new Vector2(
            m_fImageHealthBarBaseWidth * (m_fPlayerHealth / m_fImageHealthBarBaseWidth),
            oImageHealthBar.rectTransform.rect.height
            );
    }
}
