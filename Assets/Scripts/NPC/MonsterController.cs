using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterController : NpcController
{
    public float comfort;
    public List<Node> enclosureNodes;
    public int nodesCounted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        StartCoroutine(GenerateEnclosureNodes());
    }

    // Update is called once per frame
    public override void Update()
    {
        CreatePath("");
    }
        
    public IEnumerator GenerateEnclosureNodes()
    {
        yield return new WaitForSeconds(0.2f);
        enclosureNodes.Add(currentNode);
        nodesCounted = 1;

        foreach (Node node in currentNode.connections)
        {
            if (!enclosureNodes.Contains(node))
            {
                enclosureNodes.Add(node);
                nodesCounted++;
            }

            foreach (Node neighbour in node.connections)
            {
                if (!enclosureNodes.Contains(neighbour))
                {
                    enclosureNodes.Add(neighbour);
                    nodesCounted++;
                    currentNode = neighbour;
                }
            }
        }

        availableNodes = enclosureNodes;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && path.Count < 1)
        {
            currentNode = other.GetComponent<Node>();
            path.Add(currentNode);
        }
    }
}
