using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by Eddie

public class NodeConnector : MonoBehaviour
{
    //Node Connector script 
    //What this script does:
    /*
        - Is attached to root of unbuilt node map
        - Links nodes that have direct line of sight to each other
          & are withtin each others minimum link range
    */

    //Initalization
    void Start()
    {
        List<Transform> nodes = new List<Transform>();

        //Gets all nodes
        for (int i = 0; i < transform.childCount; i++)
        { nodes.Add(transform.GetChild(i)); }

        //Connects nodes
        for (int i = 0; i < nodes.Count;)
        {
            for (int e = 1; e < nodes.Count; e++)
            {
                if (Vector3.Distance(nodes[i].position, nodes[e].position) < nodes[i].GetComponent<Node>().maxConnectionDistance || Vector3.Distance(nodes[i].position, nodes[e].position) < nodes[e].GetComponent<Node>().maxConnectionDistance)
                {
                    if (Physics.Linecast(nodes[i].position, nodes[e].position) == false)
                    {
                        nodes[i].GetComponent<Node>().m_neighbourNodes.Add(nodes[e].GetComponent<Node>());
                        nodes[e].GetComponent<Node>().m_neighbourNodes.Add(nodes[i].GetComponent<Node>());
                    }
                }
            }
            nodes.RemoveAt(i);
        }
    }
}