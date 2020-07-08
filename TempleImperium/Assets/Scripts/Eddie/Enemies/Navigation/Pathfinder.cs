//////////////////////////////////////////////////                                              
//                                              //
//  Pathfinder                                  //
//  Creates a path of nodes between two points  //
//                                              //
//  Contributors : Eddie                        //
//                                              //
//////////////////////////////////////////////////        

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    ////Bespoke functions
    //Private
    private Node _findClosestNode(Vector3 target)
    {
        Node closestNode = null;
        float smallestDistance = Mathf.Infinity;

        List<Transform> nodes = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        { nodes.Add(transform.GetChild(i)); }

        for (int i = 0; i < nodes.Count; i++)
        {
            var distance = (nodes[i].transform.position - target).magnitude;
            if (distance < smallestDistance)
            {
                closestNode = nodes[i].GetComponent<Node>();
                smallestDistance = distance;
            }
        }

        return closestNode;
    }

    //Find a path between
    public Stack<Vector3> _findPathBetween(Vector3 startPosition, Vector3 endPosition)
    {
        //Initalization
        Stack<Vector3> currentPath = new Stack<Vector3>();
        var openList = new SortedList<float, Node>(new DuplicateKeyComparer<float>());
        var closedList = new List<Node>();

        //Finds closest nodes to start and end
        var currentNode = _findClosestNode(startPosition);
        var endNode = _findClosestNode(endPosition);

        //Stops execution if at destination
        if (currentNode == null || endNode == null || currentNode == endNode)
        { return null; }

        //Adds first node to open list
        openList.Add(0, currentNode);
        currentNode.previousNode = null;
        currentNode.distanceFromPrevious = 0f;

        //Whilst there are valid nodes to go to
        while (openList.Count > 0)
        {
            //Moves the node with the  cost node to closed list
            currentNode = openList.Values[0];
            openList.RemoveAt(0);

            var dist = currentNode.distanceFromPrevious;
            closedList.Add(currentNode);

            //At destination check
            if (currentNode == endNode)
            { break; }

            //Loop thorugh neighbors
            foreach (var neighbor in currentNode.neighbourNodes)
            {
                //Skip visted or to-be visted nodes
                if (closedList.Contains(neighbor) || openList.ContainsValue(neighbor))
                { continue; } //Stops current neighbor execution and goes to the next

                //Sets neighbor's previous to current so the path can be traced back
                neighbor.previousNode = currentNode;

                //Sets neighbor's distance from currentnode
                neighbor.distanceFromPrevious = dist + (neighbor.transform.position - currentNode.transform.position).magnitude;

                //Calcuales neighbor's distance from end
                var distanceToTarget = (neighbor.transform.position - endNode.transform.position).magnitude;

                //Adds neighnor and its distance from end to the open list
                openList.Add(neighbor.distanceFromPrevious + distanceToTarget, neighbor);
            }
        }

        //Path retracing
        if (currentNode == endNode)
        {
            //While not at the start node
            while (currentNode.previousNode != null)
            {
                //Adds and mvoes back througth the path
                currentPath.Push(currentNode.transform.position);
                currentNode = currentNode.previousNode;
            }
        }

        return currentPath;
    }
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
