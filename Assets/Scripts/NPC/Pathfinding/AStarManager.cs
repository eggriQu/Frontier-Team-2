using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Node> GeneratePath(Node startNode, Node endNode)
    {
        List<Node> openSet = new List<Node>();

        foreach (Node n in FindObjectsByType<Node>(FindObjectsSortMode.None))
        {
            n.gScore = float.MaxValue;
        }

        startNode.gScore = 0;
        startNode.hScore = Vector3.Distance(startNode.transform.position, endNode.transform.position);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            int lowestF = default;

            // Find lowest FScore Node index
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FScore() < openSet[lowestF].FScore())
                {
                    lowestF = i;
                }
            }

            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            // Check if it's the end node
            if (currentNode == endNode)
            {
                List<Node> path = new List<Node>();

                path.Insert(0, endNode);

                while(currentNode != startNode)
                {
                    currentNode = currentNode.cameFrom;
                    path.Add(currentNode);
                }

                // Trace path backwards to start node and return
                path.Reverse();
                return path;
            }

            // Check neighbour Node connections
            foreach(Node connectedNode in currentNode.connections)
            {
                float heldGScore = currentNode.gScore + Vector3.Distance(currentNode.transform.position, connectedNode.transform.position);

                if (heldGScore < connectedNode.gScore)
                {
                    connectedNode.cameFrom = currentNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = Vector3.Distance(connectedNode.transform.position, endNode.transform.position);

                    if (!openSet.Contains(connectedNode))
                    {
                        openSet.Add(connectedNode);
                    }
                }
            }
        }

        return null;
    }

    public Node[] AllNodes()
    {
        return FindObjectsByType<Node>(FindObjectsSortMode.None);
    }
}
