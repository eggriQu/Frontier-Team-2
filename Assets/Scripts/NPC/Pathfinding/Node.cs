using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node cameFrom;
    private NodeGenerator _nodeGenerator;
    public List<Node> connections;

    public bool hasAttraction;
    public bool hasBeenVisited;
    public float gScore;
    public float hScore;

    public float FScore()
    {
        return gScore + hScore;
    }

    public void Awake()
    {
        _nodeGenerator = GameObject.Find("NodeParent").GetComponent<NodeGenerator>();

        float randomFloat = Random.Range(0, 20);
        if (randomFloat == 6 || randomFloat == 7)
        {
            hasAttraction = true;
        }
    }

    public void DestroyNode()
    {
        foreach (Node node in connections)
        {
            node.connections.Remove(this);

            for (int i = 0;  i < node.connections.Count; i++)
            {
                if (node.connections[i] == null)
                {
                    node.connections.RemoveAt(i);
                }
            }
        }

        _nodeGenerator.nodeList.Remove(this);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.whiteSmoke;

        if (connections.Count > 0)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                Gizmos.DrawLine(transform.position, connections[i].transform.position);
            }
        }

        if (hasAttraction)
        {
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Node>() != null)
        {
            connections.Add(other.gameObject.GetComponent<Node>());
        }

        if (other.CompareTag("Wall"))
        {
            DestroyNode();
        }
    }
}