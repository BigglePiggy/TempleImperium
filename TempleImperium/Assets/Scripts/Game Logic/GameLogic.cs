﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;



//Written by Ase
//Bugs fixed by Eddie


public class GameLogic : MonoBehaviour
{
    [HideInInspector]
    public enum StarstoneElement { Fire, Water, Electricity, Darkness };    //used across lots of classes!! BE CAREFUL WITH ME

    //gameplay controller script - there should be only one instance of me!
    //what this script does:
    /*
        - handling wavedata
        - handling wave logic
        - telling enemy spawner when/what to spawn
        - has important public structs (eg StarstoneElement enum)
    */

        //TODO!!!
        //PYLONS
        //GENERATOR
        //PASS ON WAVE STARSTONE ELEMENT TO ENEMIES AND PLAYER

    [Header("level configuration")]
    [Tooltip("number of seconds to wait at the start before beginning gameplay")]
    public float m_fStartRestDuration = 10;    
    [Tooltip("number of seconds between all enemies dying and the next wave starting")]
    public float m_fInterwaveRestDuration = 10;
    [Tooltip("list of objects to retrieve WaveData from, in order")]
    public List<GameObject> m_oWaveDataContainerList = new List<GameObject>();      //list of objects containing a WaveData script
    [Tooltip("list of pylon objects")]
    public List<GamePylon> m_oPylonList = new List<GamePylon>();  //list of all pylon gameobjects

    [Header("script references")]
    [Tooltip("reference to a GameObject that has a GameEnemyDispatch script attached")]
    public GameObject oEnemyDispatch;   //entity that has GameEnemyDispatch script
    public Text timerText;


    WaveDataObject[] m_WaveDataArray;       //wave data retrieved from objects

    int m_iCurrentWave = 0;                     //index of current wave
    int m_iCurrentWaveSub = 0;                  //index of current subwave
    int m_iEnemiesAlive = 0;                    //num of enemies alive
    /// <summary>
    /// gameplay PHASE duration. this is NOT the wave timer!!!
    /// </summary>
    int m_iTickerCurrentPhase = 1000;    //duration of current gameplay phase, in ticks (this is **NOT** THE WAVE TIMER)
    /// <summary>
    /// WAVE timer. this is NOT the gameplay phase!!
    /// </summary>
    int m_iTickerCurrentWave = 1000;    //duration of current wave, in ticks
    /// <summary>
    /// controls m_iTickerCurrentWave decrement
    /// </summary>
    bool m_bWaveTimerActive = false;    //should WAVE ticker decrement?



    enum GameplayPhase { Init, PreGame, Subwave, InbetweenSubwave, InbetweenWave, PostGame } //gameplay phase
    GameplayPhase m_eGameplayPhase = GameplayPhase.Init;
    //init->pregame->
    //subwave(enemy deaths)->inbetweenSubwave(timer)->subwave(enemy deaths)->...->inbetweenWave(timer)->subwave->...->postGame (win state)


    GenericFunctions cGenericFunctions = new GenericFunctions(); //instantiate a GenericFunctions for use here - having issues making it a static class

    // Start is called before the first frame update
    void Start()
    {
        //wavedata integrity checks should be in WaveData.Start(), not here!

        //redeclare array with proper size
        m_WaveDataArray = new WaveDataObject[m_oWaveDataContainerList.Count];

        //retrieve all wavedata
        for (int i = 0; i < m_oWaveDataContainerList.Count; i++) //for every wavedata container...
        {
            //retrieve wave datas, save to array
            m_WaveDataArray[i] = m_oWaveDataContainerList[i].GetComponent<WaveData>().GetWaveData();
        }

        //initialisation set up in EnterNextPhase - right now, gamestate = Init
        EnactPhase();
    }

    // Update is called once per frame
    void Update()
    {
        //do wave logic
    }
    void FixedUpdate()
    {
        //check to decrement wave ticker
        if (m_bWaveTimerActive) { m_iTickerCurrentWave--; }

        //check to gameover
            //if ticker less/somehow below 0, AND ticker active, AND in (subwave OR inbetween subwave),
        if ( m_iTickerCurrentWave <= 0 && m_bWaveTimerActive && (
            m_eGameplayPhase == GameplayPhase.Subwave || m_eGameplayPhase == GameplayPhase.InbetweenSubwave)
            )
        {
            //game over!
            //TODO! kill player here
            //waiting to merge builds with Ed's player & AI stuff
            Debug.LogError("Game over! GameLogic.FixedUpdate has no gameover programmed yet. Bug Ase about it!");
        }

        //check phase ticker
        if (m_iTickerCurrentPhase > 0)    //if phase ticker nonzero,
        {
            m_iTickerCurrentPhase--;     //decrement phase ticker
        }
        else    //if end of phase ticker...
        {
            switch (m_eGameplayPhase)
            {
                case GameplayPhase.Init:    //init step --> pregame wait
                    m_eGameplayPhase = GameplayPhase.PreGame;   //set next phase
                    EnactPhase();                               //enact
                    break;

                case GameplayPhase.PreGame: //pregame wait --> subwave (combat phase)
                    m_eGameplayPhase = GameplayPhase.Subwave;   //set next phase
                    EnactPhase();                               //enact
                    break;

                case GameplayPhase.InbetweenWave:   // InbetweenWave --> next waves first subwave 
                case GameplayPhase.InbetweenSubwave:    // InbetweenSubwave --> next subwave
                    m_eGameplayPhase = GameplayPhase.Subwave;
                    EnactPhase();                              
                    break;

                case GameplayPhase.Subwave: //subwave (doesn't go anywhere)
                    //going to InbetweenSubwave is handled in WaveEventEnemyDeath()
                    break;

                default:
                    //layer 8 error catcher
                    Debug.LogError("GameLogic FixedUpdate timer=0 switch reading unaccounted for GameplayPhase \"" + m_eGameplayPhase + "\"!!");
                    break;
            }
        }

        timerText.text = m_iTickerCurrentPhase.ToString() + ":" + m_iTickerCurrentWave.ToString();
    }

    #region wave expected event handling
    /// <summary>
    /// does stuff depending on what the (sub)wave indeces, game phase ticker, and gamephase enum are.
    /// </summary>
    public void EnactPhase()
    {
        //check if subwave is in bounds (if not, subwave=0 and ++wave)
        //if current subwave >= current wave's subwaveArray length
        if(m_iCurrentWaveSub >= m_WaveDataArray[m_iCurrentWave].m_iSubWavesArray.GetLength(0))
        {
            m_iCurrentWave++;           //move to next wave
            m_iCurrentWaveSub = 0;      //reset subwave counter
            m_eGameplayPhase = GameplayPhase.InbetweenWave;     //we're now between waves!
        }

        //check if wave is in bounds (if not, gamephase = postgame)
        //if current waven >= wave data array's length (aka no more wave data left)
        if(m_iCurrentWave >= m_WaveDataArray.Length)
        {
            m_eGameplayPhase = GameplayPhase.PostGame;  //set to postgame - we win!
        }

        //enact gameplay phase
        switch (m_eGameplayPhase)
        {
            case GameplayPhase.Init:        //if init
                //initialise game
                Debug.Log("enactphase() switch firing Init");

                //put initialisation things here!
                m_iTickerCurrentPhase = 0;//immediately refire this function for PreGame phase
                break;

            case GameplayPhase.PreGame:     //if pregame
                Debug.Log("enactphase() switch firing PreGame");

                //pregame is mostly just a countdown to start wave1sub1

                //set timer
                m_iTickerCurrentPhase = cGenericFunctions.ConvertSecondsToTicks(m_fStartRestDuration);

                //lower pylons
                lowerPylons();
                break;

            case GameplayPhase.Subwave:
                Debug.Log("enactphase() switch firing Subwave");

                //subwaves are combat sections.

                int[] m_ThisSubWaveData = new int[3];   //temp subwave data container

                for(int i = 0; i < 3; i++) //for each enemy type (hardcoded! change i<n if adding new dudes)
                {
                    //retrieve enemy data for this subwave
                    //temp = wave data array[current wave].subwaves data.[current subwave]
                    m_ThisSubWaveData[i] = m_WaveDataArray[m_iCurrentWave].m_iSubWavesArray[m_iCurrentWaveSub, i];
                }

                //dispatch a subwave with retrieved data (and subwave element)
                DispatchSubwave(m_ThisSubWaveData[0], m_ThisSubWaveData[1], m_ThisSubWaveData[2],
                    m_WaveDataArray[m_iCurrentWave].m_eStarstoneElement);

                //set the PHASE timer really high. it doesn't matter here, and leaving it 0 would take a few extra cycles
                //(enemy death handles going to InbetweenSubwave - the usual system is skipped here.)
                m_iTickerCurrentPhase = 99999;

                //if this is the first subwave, set the WAVE timer
                if (m_iCurrentWaveSub == 0)
                {
                    m_iTickerCurrentWave = cGenericFunctions.ConvertSecondsToTicks(m_WaveDataArray[m_iCurrentWave].m_iWaveDuration);
                    raisePylons(2);
                }
                //...and make sure wave timer's ticking down
                m_bWaveTimerActive = true;
                break;

            case GameplayPhase.InbetweenSubwave:
                Debug.Log("enactphase() switch firing InbetweenSubwave");
                //this one's called when the player's just beaten a SUBwave - game's waiting to spawn next batch, wave counter's still going

                m_iTickerCurrentPhase = cGenericFunctions.ConvertSecondsToTicks(m_WaveDataArray[m_iCurrentWave].m_iSubWaveRestDuration);
                break;

            case GameplayPhase.InbetweenWave:
                Debug.Log("enactphase() switch firing InbetweenWave");
                //wave beaten, player's resting before the next wave starts.
                //turn off WAVE timer
                m_bWaveTimerActive = false;

                //set PHASE timer
                m_iTickerCurrentPhase = cGenericFunctions.ConvertSecondsToTicks(m_fInterwaveRestDuration);

                //lower pylons
                lowerPylons();
                break;

            case GameplayPhase.PostGame:
                Debug.Log("enactphase() switch firing PostGame");
                //win!
                Debug.LogWarning("gamelogic enactphase() doesn't have a win condition programmed yet! bug Ase about it");
                break;

        }
    }

    public void DispatchSubwave(int input_numLight, int input_numMedium, int input_numHeavy, StarstoneElement input_element)
    {
        //add to counter
        m_iEnemiesAlive += input_numLight + input_numMedium + input_numHeavy;

        //Dispatch wave
        oEnemyDispatch.GetComponent<GameEnemyDispatch>().DispatchSubwave(input_numLight, input_numMedium, input_numHeavy, input_element);
    }
    #endregion

    #region wave arbitrary event handling
    //called when an enemy dies
    public void WaveEventEnemyDeath()
    {
        m_iEnemiesAlive--;      //decrement counter

        if (m_iEnemiesAlive <= m_WaveDataArray[m_iCurrentWave].m_iSubWaveKillLenience)       //if no enemies left (or below kill lenience)...
        {
            m_iCurrentWaveSub++;                                    //increment subwave counter
            m_eGameplayPhase = GameplayPhase.InbetweenSubwave;      //go to wait period
            EnactPhase();                                           //enact
        }

    }
    #region timer related things
    //add time to the counter (in ticks)
    public void WaveEventExtendTimerTicks (int input_ticks)
    {
#if UNITY_EDITOR
        //error checking
        if(input_ticks <= 0)
        {
            Debug.LogError("GameLogic.WaveEventExtendTimerTicks() given input_ticks <=0!! what happened?");
            input_ticks = 1;
        }
#endif

        m_iTickerCurrentPhase += input_ticks;        //add to ticker
    }
    //add time to the counter (in seconds)
    public void WaveEventExtendTimerSeconds(int input_seconds, int input_tickrate = 60)
    {
#if UNITY_EDITOR
        //error checking
        if (input_seconds <= 0)
        {
            Debug.LogError("GameLogic.WaveEventExtendTimerSeconds() given input_seconds <=0!! what happened?");
            input_seconds = 1;
        }
#endif
        m_iTickerCurrentPhase += cGenericFunctions.ConvertSecondsToTicks(input_seconds, input_tickrate); //convert, add to ticker
    }
    #endregion
    #endregion

    #region pylon movement and logic
    //Lower all pylons
    private void lowerPylons() 
    {
        for (int i = 0; i < m_oPylonList.Count; i++)
        {
            m_oPylonList[i].GoDown();
        }
    }

    //Raise a number of randomly selected pylons
    private void raisePylons(int num)
    {
        m_oPylonList = m_oPylonList.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < num; i++)
        {
            m_oPylonList[i].GoUp();
        }
    }
    #endregion

}
