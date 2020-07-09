using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//written by Ase


public class WaveData : MonoBehaviour
{
    //this class contains data for each wave, and passes it on to GameLogic where needed.

#if UNITY_EDITOR
    public string WaveNickname = "Unnamed Wave";

    //editor warning thresholds
    int m_iSubWavesReasonableLimit = 5;

#endif

    [Header("wave settings")]
    [Tooltip("wave countdown time limit, in seconds")]
    public float m_iWaveDuration = 90;    //wave duration in seconds
    [Tooltip("time to wait, in seconds, between enough subwave enemies killed and spawning in the next subwave")]
    public float m_iSubWaveRestDuration = 3;  //gap between subwaves in seconds
    [Tooltip("subwave tolerance - start next subwave countdown when this number of enemies are left alive")]
    public int m_iSubWaveKillLenience = 1;  //number of enemies tolerance
    public GameLogic.StarstoneElement m_eStarstoneElement;
    [Space]
    [Tooltip("enemies per subwave - formatted \"X,Y,Z\" for light, medium, and heavy." +
    "\na configurable number of enemies in this subwave must be killed before the next subwave can spawn." +
    "\neach entry in this list is another subwave.")]
    public List<string> m_sSubWavesList = new List<string>();   //enemy data (string input)

    int[,] m_iSubWavesArray;    //enemy data (deserialised from str-->int)

    WaveDataObject m_PackagedWaveData;  //final data lump to be passed out in GetWaveData


    // Start is called before the first frame update
    void Start()
    {
        //redeclare subwaves with proper size
        //x = num subwaves, y = three slots for each enemy count
        m_iSubWavesArray = new int[m_sSubWavesList.Count - 1, 2];

#if UNITY_EDITOR
        //reasonable amount of subwaves warning
        if(m_sSubWavesList.Count > m_iSubWavesReasonableLimit) { Debug.LogWarning("wave \"" + WaveNickname + "\" has a lot of subwaves. are you sure?"); }
#endif

        //enemy list deserialisation from string input --> list of 1d arrays, integrity check
        for (int i = 0; i < m_sSubWavesList.Count-1; i++) //for every subwave entry
        {
            //split into numerical strings
            string[] m_sSubWaveSplitEnemyCounts = m_sSubWavesList[i].Split(',');

#if UNITY_EDITOR
            //integrity check
            //if malformed array length (.length 1based!!), alert and halt
            if (m_sSubWaveSplitEnemyCounts.Length != 3)
            {
                Debug.LogError("wave \"" + WaveNickname + "\" subwave index " + i + " has bad enemy numbers input! halting");
                Debug.Break();
            }
#endif
            //write from strings to array
            for (int j = 0; j < 2; j++)    
            {
                //Debug.Log("i " + i + "\tj " + j);
                m_iSubWavesArray[i, j] = int.Parse(m_sSubWaveSplitEnemyCounts[j]);  //parse string to int
            }

            //write all into one object
            m_PackagedWaveData = new WaveDataObject(
                m_iWaveDuration,
                m_iSubWaveRestDuration,
                m_iSubWaveKillLenience,
                m_eStarstoneElement,
                m_iSubWavesArray
                );

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //returns all wave data - called from GameLogic
    public WaveDataObject GetWaveData()
    {
        return m_PackagedWaveData;
    }
}
