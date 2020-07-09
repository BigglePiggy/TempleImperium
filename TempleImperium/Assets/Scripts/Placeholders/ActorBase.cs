using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;


//written by Ase



public class ActorBase : MonoBehaviour
{

    //basic enemy AI actor class - placeholder for development, will be using Ed's instead

    /// <summary>
    /// delay, in game update ticks, before recalculating path to player. i can get expensive if set too low!
    /// </summary>
    public int m_iRepathTickDelay = 30;         //^
    int m_iRepathTicker = 0;                    //private ticker to be measured; pathfind and reset from public var on 0

    //instance configs
    GameLogic.StarstoneElement m_eStarstoneElement;     //what flavour of starstone energy does this enemy utilise?

    NavMeshAgent oNavMeshAgent;                 //this actor's AI agent
    GameObject oPlayer;                         //player object

    void Start()
    {
        Debug.LogError("error: deprecated script! use Ed's AI actors!"); //this script is placeholder - alert!
        Debug.LogError("error: enemy with base actor script! i contain no specialised code and won't do much!"); //actorbase shouldnt be instantiated


        oPlayer = GameObject.Find("Player");                    //establish reference to Player
        oNavMeshAgent = this.GetComponent<NavMeshAgent>();      //establish reference to this actor's AI agent
    }

    /// <summary>
    /// init step. pass in instantiation variables here!
    /// </summary>
    /// <param name="input_starstoneElement"></param>
    public virtual void Initialise(GameLogic.StarstoneElement input_starstoneElement)
    {
        
    }

    void Update()
    {
        if(m_iRepathTicker > 0) //if ticker nonzero..
        {
            m_iRepathTicker--;      //decrement.
        }
        else                    //otherwise..
        {
            _Path();                                //pathfind,
            m_iRepathTicker = m_iRepathTickDelay;   //reset ticker
        }
    }

    /// <summary>
    /// pathfind to target
    /// </summary>
    public virtual void _Path()
    {
        oNavMeshAgent.SetDestination(oPlayer.transform.position);   //begin pathing to Player position
    }
}
