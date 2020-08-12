using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

//Created by Eddie

public class Pathfinder : MonoBehaviour
{
    //Pathfinder script     -   This should be attached to the root of a node map
    //What this script does:
    /*
        - Locates closest node to a given point 
        - Uses A* to generate a path (Vector3 Stack) between two points through the attached node map
    */

    #region Declarations
    private Transform m_player;             //Player transform reference
    private List<Transform> m_nodes;        //Holds all the node in the map
    private List<Transform> m_coveredNodes; //Stores the nodes that give cover from the player
    #endregion

    //Initalization
    private void Start()
    {
        m_nodes = new List<Transform>();            //Initializes the list
        m_coveredNodes = new List<Transform>();     //Initializes the list   

        m_player = GameObject.FindGameObjectWithTag("Player").transform;    //Player transform reference   

        for (int i = 0; i < transform.childCount; i++) //Assigns all children (nodes) to the node list
        { m_nodes.Add(transform.GetChild(i)); }
    }

    #region Pathfinding
    //Locates the cloest node that is out of sight from the player withtin a given range
    public Node FindClosestCoveredNode(Vector3 input_target, float input_range) 
    {
        RaycastHit hit;
        m_coveredNodes.Clear();

        for (int i = 0; i < m_nodes.Count; i++) 
        {
            if (Vector3.Distance(m_nodes[i].position, m_player.position) < input_range)
            {
                if (Physics.Linecast(m_nodes[i].position, m_player.position, out hit)) 
                {
                    if (hit.collider.CompareTag("Player") == false && hit.collider.CompareTag("Enemy") == false) 
                    {
                        //if (Physics.Raycast(input_target, input_target - m_coveredNodes[i].position.normalized, out hit, 1000)) 
                        //{
                            //if (hit.collider.CompareTag("Player") == false ) 
                            //{
                                m_coveredNodes.Add(m_nodes[i]);
                            //}
                        //}                        
                    } 
                }
            }
        }

        //Puts the cloest cover node at index 0
        m_coveredNodes = m_coveredNodes.OrderBy(x => Vector3.Distance(input_target, m_player.position)).ToList();

        return m_coveredNodes[0].GetComponent<Node>(); //Returns cloest covered node
    }

    //Finds the closest node to a given position
    private Node FindClosestNode(Vector3 input_target)
    {
        Node closestNode = null;
        float smallestDistance = Mathf.Infinity;

        for (int i = 0; i < m_nodes.Count; i++)
        {
            var distance = (m_nodes[i].transform.position - input_target).magnitude;
            if (distance < smallestDistance)
            {
                closestNode = m_nodes[i].GetComponent<Node>();
                smallestDistance = distance;
            }
        }

        return closestNode;
    }

    public Stack<Vector3> FindPathBetween(Vector3 input_start, Vector3 input_end)
    {
        //Initalization
        Stack<Vector3> currentPath = new Stack<Vector3>();
        var openList = new SortedList<float, Node>(new DuplicateKeyComparer<float>());
        var closedList = new List<Node>();

        //Finds closest nodes to start and end
        var currentNode = FindClosestNode(input_start);
        var endNode = FindClosestNode(input_end);

        //Stops execution if at destination
        if (currentNode == null || endNode == null || currentNode == endNode)
        { return null; }

        //Adds first node to open list
        openList.Add(0, currentNode);
        currentNode.m_previousNode = null;
        currentNode.m_distanceFromPrevious = 0f;

        //Whilst there are valid nodes to go to
        while (openList.Count > 0)
        {
            //Moves the node with the  cost node to closed list
            currentNode = openList.Values[0];
            openList.RemoveAt(0);

            var dist = currentNode.m_distanceFromPrevious;
            closedList.Add(currentNode);

            //At destination check
            if (currentNode == endNode)
            { break; }

            //Loop thorugh neighbors
            foreach (var neighbor in currentNode.m_neighbourNodes)
            {
                //Skip visted or to-be visted nodes
                if (closedList.Contains(neighbor) || openList.ContainsValue(neighbor))
                { continue; } //Stops current neighbor execution and goes to the next

                //Sets neighbor's previous to current so the path can be traced back
                neighbor.m_previousNode = currentNode;

                //Sets neighbor's distance from currentnode
                neighbor.m_distanceFromPrevious = dist + (neighbor.transform.position - currentNode.transform.position).magnitude;

                //Calcuales neighbor's distance from end
                var distanceToTarget = (neighbor.transform.position - endNode.transform.position).magnitude;

                //Adds neighnor and its distance from end to the open list
                openList.Add(neighbor.m_distanceFromPrevious + distanceToTarget, neighbor);
            }
        }

        //Path retracing
        if (currentNode == endNode)
        {
            //While not at the start node
            while (currentNode.m_previousNode != null)
            {
                //Adds and mvoes back througth the path
                currentPath.Push(currentNode.transform.position);
                currentNode = currentNode.m_previousNode;
            }
        }

        return currentPath;
    }
    #endregion
}

//Sorted list duplicate key fix
public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
{
    public int Compare(TKey x, TKey y)
    {
        int result = x.CompareTo(y);

        if (result == 0)
        { return 1; }
        else
        { return result; }
    }
}
