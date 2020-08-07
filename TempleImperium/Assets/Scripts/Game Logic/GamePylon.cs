using UnityEngine;

//written by Eddie and Ase

public class GamePylon : MonoBehaviour
{
    ////Declarations
    //Private
    private GameObject oGameLogic;
    private Animator anim;

    GameObject oGlow;

    //Initalization
    void Start()
    {
        oGameLogic = GameObject.Find("Game Logic");
        anim = GetComponent<Animator>();
        oGlow = gameObject.transform.Find("Core").Find("Outline").gameObject;
    }

    //Raise the pylon
    public void GoUp()
    {
        anim.SetBool("Raise", true);
        anim.SetBool("Lower", false);

        oGlow.SetActive(true);
    }

    //Lower the pylon
    public void GoDown()
    {
        anim.SetBool("Raise", false);
        anim.SetBool("Lower", true);

        oGlow.SetActive(false);
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
            if (ForeignCollider.tag == "Player")
            {
                GoDown();

                //tell gamelogic to run pylon down function
                if (oGameLogic == null)  
                { oGameLogic = GameObject.Find("Game Logic"); }
                oGameLogic.GetComponent<GameLogic>().WaveEventPylonLoweredByPlayer();
            }
        }
    }
}
