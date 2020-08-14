using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public float IntroLength;   
    float IntroTimer;

    //Called Per Frame
    void Update()
    {
        if (IntroTimer < IntroLength)
        { IntroTimer += Time.deltaTime; }

        if (Input.GetMouseButtonDown(0) || IntroTimer >= IntroLength) 
        { LoadTemple(); }
    }

    private void LoadTemple() 
    {
        SceneManager.LoadScene("Temple");
    }
}
