//////////////////////////////////////////////////                                              
//                                              //
//  NodeConnector                               //
//  Connects associated nodes                   //
//                                              //
//  Contributors : Eddie                        //
//                                              //
//////////////////////////////////////////////////    

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeConnector : MonoBehaviour
{
    ////Declarations
    //Private
    private List<Transform> nodes;

    //Initalization
    void Start()
    {
        nodes = new List<Transform>();

        //Variables
        for (int i = 0; i < transform.childCount; i++)
        { nodes.Add(transform.GetChild(i)); }

        _connectNodes();
    }

    ////Bespoke Fucntions
    //Private
    private void _connectNodes()
    {
        for (int i = 0; i < nodes.Count;)
        {
            for (int e = 1; e < nodes.Count; e++)
            {
                if (Vector3.Distance(nodes[i].position, nodes[e].position) < nodes[i].GetComponent<Node>().maxConnectionDistance || Vector3.Distance(nodes[i].position, nodes[e].position) < nodes[e].GetComponent<Node>().maxConnectionDistance)
                {
                    if (Physics.Linecast(nodes[i].position, nodes[e].position) == false)
                    {
                        nodes[i].GetComponent<Node>().neighbourNodes.Add(nodes[e].GetComponent<Node>());
                        nodes[e].GetComponent<Node>().neighbourNodes.Add(nodes[i].GetComponent<Node>());
                    }
                }
            }
            nodes.RemoveAt(i);
        }
    }
}