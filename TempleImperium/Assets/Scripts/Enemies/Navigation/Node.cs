 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
    public List<Node> m_neighbourNodes;

    //Properties
    public Node m_previousNode    //Node previously visited by Pathfinder's a* algorithm
    {
        get;
        set;
    }

    public float m_distanceFromPrevious   //Distance from previousNode
    {
        get;
        set;
    }

    public float m_distanceFromEnd    //Distance from end of path that Pathfinder is calculating
    {
        get;
        set;
    }
    #endregion

#if UNITY_EDITOR
    #region Visual Display
    private void OnDrawGizmos()
    {
        //if (Selection.Contains(gameObject) || Selection.Contains(transform.root.gameObject))
        //{
            if (m_neighbourNodes.Count > 0)
            {
                Gizmos.color = Color.white;
                for (int i = 0; i < m_neighbourNodes.Count; i++)
                {
                    try
                    { Gizmos.DrawLine(transform.position, m_neighbourNodes[i].transform.position); }
                    catch (System.Exception)
                    { }
                }
            }
          
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(transform.position, 0.1f);
        //}

        if (Selection.Contains(gameObject)) 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxConnectionDistance); 
        }
    }
    #endregion
#endif
}