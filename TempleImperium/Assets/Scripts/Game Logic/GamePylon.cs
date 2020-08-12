using UnityEngine;

//Created by Eddie and Ase

public class GamePylon : MonoBehaviour
{
    //Game Pylon script
    //What this script does:
    /*
        - Handles collision detection
        - Manages animation controller
        - Manages shader visability
    */

    #region Declarations
    Animator m_anim;            //Pylon animator
    GameLogic m_GameLogic;     //
    GameObject m_Glow;
    #endregion

    //Initalization
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_GameLogic = GameObject.Find("Game Logic").GetComponent<GameLogic>();
        m_Glow = gameObject.transform.Find("Core").Find("Outline").gameObject;
    }

    #region Public methods
    //Raise the pylon
    public void GoUp()
    {
        m_anim.SetBool("Raise", true);
        m_anim.SetBool("Lower", false);

        m_Glow.SetActive(true); //Enables through-wall outline shader
    }

    //Lower the pylon
    public void GoDown()
    {
        m_anim.SetBool("Raise", false);
        m_anim.SetBool("Lower", true);

        m_Glow.SetActive(false); //Disables through-wall outline shader
    }

    //Pylon state
    public bool GetState() 
    {
        return m_anim.GetBool("Raise"); //Raised == True    Lowered == False
    }
    #endregion

    //Collision detection
    private void OnTriggerEnter(Collider ForeignCollider)
    {
        if (m_anim.GetBool("Raise"))
        {
            //If foreign collider is the player, this pylon goes down
            if (ForeignCollider.tag == "Player")
            {
                GoDown();

                //tell gamelogic to run pylon down function
                if (m_GameLogic == null)  
                { m_GameLogic = GameObject.Find("Game Logic").GetComponent<GameLogic>(); }

                m_GameLogic.WaveEventPylonLoweredByPlayer();
            }
        }
    }
}
