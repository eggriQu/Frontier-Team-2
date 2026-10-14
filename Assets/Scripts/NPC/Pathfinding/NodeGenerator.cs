using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class NodeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    public List<Node> nodeList;
    [SerializeField] private List<float> gridColumns;
    [SerializeField] private List<float> gridRows;
    [SerializeField] private float gridSize;
    [SerializeField] private float nodeOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateNodes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateCoords()
    {
        for (int i = 0; i < gridSize; i++)
        {
            if (i == 0)
            {
                gridColumns.Add(i);
                gridRows.Add(i);
            }
            else
            {
                gridColumns.Add(i * nodeOffset);
                gridRows.Add(i * nodeOffset);
            }
        }
    }

    void GenerateNodes()
    {
        GenerateCoords();

        foreach (float xCord in gridRows)
        {
            foreach (float yCord in gridColumns)
            {
                Instantiate(nodePrefab, new Vector3(xCord, transform.position.y, yCord), Quaternion.identity, transform);
            }
        }
    }
}
