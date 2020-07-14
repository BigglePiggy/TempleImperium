//////////////////////////////////////////////////                                              
//                                              //
//  Node                                        //
//  Backbone of pathfinding algorithm           //
//                                              //
//  Contributors : Eddie                        //
//                                              //
//////////////////////////////////////////////////   

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    ////Declarations
    //Public
    public float maxConnectionDistance;
    public List<Node> neighbourNodes;

    //Properties
    public Node previousNode
    {
        get;
        set;
    }

    public float distanceFromPrevious
    {
        get;
        set;
    }

    public float distanceFromEnd
    {
        get;
        set;
    }

    //Gizmo link display
    private void OnDrawGizmos()
    {
        if (neighbourNodes.Count > 0)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < neighbourNodes.Count; i++)
            {
                try
                { Gizmos.DrawLine(transform.position, neighbourNodes[i].transform.position); }
                catch (System.Exception)
                { }
            }
        }

        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, maxConnectionDistance );

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}