 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by Eddie

public class Node : MonoBehaviour
{
    //Node script 
    //What this script does:
    /*
        - Contains properties that Pathfinder utilizes
        - Stores a list of relationships to neighbour nodes
        - Displays visual aid for each node 
    */

    #region Declarations
    [Header("Node Configuration")]
    [Tooltip("Nodes withtin this will be connected by Node Connector")]
    public float maxConnectionDistance;
    [Space]

    [Tooltip("List of all nodes that have a reloations with this node")]
    public List<Node> neighbourNodes;

    //Properties
    public Node previousNode    //Node previously visited by Pathfinder's a* algorithm
    {
        get;
        set;
    }

    public float distanceFromPrevious   //Distance from previousNode
    {
        get;
        set;
    }

    public float distanceFromEnd    //Distance from end of path that Pathfinder is calculating
    {
        get;
        set;
    }
    #endregion

    #region Visual Display
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
        //Gizmos.DrawWireSphere(transform.position, maxConnectionDistance);

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
    #endregion
}