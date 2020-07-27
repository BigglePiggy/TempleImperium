using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//written by Ase


public class HUDFlyingText : MonoBehaviour
{
    //text object that spawns onscreen, flies up (ease-in), and deletes itself.

    public uint m_uiLifetimeMax = 60;
    uint m_uiLifetime = 0;
    public float m_fEaseMultiplier = 1.01f;
    Vector2 m_vInitialXY;
    float m_fOffsetY = 1;

    void Start()
    {
        m_vInitialXY = gameObject.transform.position;   //save pos
    }

    public void Initialise(Color input_colour, string input_text, float input_easeMultiplier)
    {
        gameObject.GetComponent<Text>().color = input_colour;
        gameObject.GetComponent<Text>().text = input_text;

        m_fEaseMultiplier = input_easeMultiplier;

        //layer 8 error catcher
        if (m_fEaseMultiplier <= 1) { m_fEaseMultiplier = 1.01f; Debug.LogWarning("HUDFlyingText with easeMultiplier <=1. setting to 1.01...."); }
    }

    void Update()
    {
        //doing it this order means the object's position lags behind by one tick (but that's fine)
        //write
        gameObject.GetComponent<Text>().rectTransform.position = m_vInitialXY;
        //calc for next
        m_fOffsetY *= m_fEaseMultiplier;
        m_vInitialXY.y = m_fOffsetY;

        //lifetime
        m_uiLifetime++;
        if(m_uiLifetime >= m_uiLifetimeMax) { Destroy(gameObject); }    //die when lifetime >= max
    }
}
