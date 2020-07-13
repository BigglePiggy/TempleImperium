using UnityEngine;

//written by Eddie and Ase

public class GamePylon : MonoBehaviour
{
    ////Declarations
    //Private
    private GameObject oGameLogic;
    private Animator anim;

    //Initalization
    void Start()
    {
        oGameLogic = GameObject.Find("Game Logic");
        anim = GetComponent<Animator>();
    }

    //Raise the pylon
    public void GoUp()
    {
        anim.SetBool("Raise", true);
        anim.SetBool("Lower", false);
    }

    //Lower the pylon
    public void GoDown()
    {
        anim.SetBool("Raise", false);
        anim.SetBool("Lower", true);
    }

    //Pylon state
    public bool GetState() 
    {
        if (anim.GetBool("Raise")) 
        {
            return true;
        }
        else 
        { 
            return false; 
        }
    }

    //Collision detection
    private void OnTriggerEnter(Collider ForeignCollider)
    {
        if (anim.GetBool("Raise"))
        {
            //if foreign collider is the player, this pylon goes down
            if (ForeignCollider.tag == "Player") { GoDown(); }

            //tell gamelogic to run pylon down func
            oGameLogic.GetComponent<GameLogic>().WaveEventPylonLoweredByPlayer();
        }
    }
}
