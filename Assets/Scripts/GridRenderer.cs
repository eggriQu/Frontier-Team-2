using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GridRenderer : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private float heightOffset = 0.02f;

    private Dictionary<BuildSurface, List<LineRenderer>> surfaceGrids =
        new Dictionary<BuildSurface, List<LineRenderer>>();

    private Material gridMaterial;
    private bool keepGridVisible;

    private void Awake()
    {
        CreateGridMaterial();
    }

    //Creates the grid material
    private void CreateGridMaterial()
    {
        Shader gridShader = Shader.Find("Universal Render Pipeline/Unlit")
            ?? Shader.Find("Unlit/Color");

        if (gridShader == null)
            return;

        gridMaterial = new Material(gridShader);
        gridMaterial.color = Color.black;
    }

    //Creates the grid over a selected surface
    private void CreateGrid(BuildSurface surface)
    {
        if (surface == null || gridMaterial == null || cellSize <= 0f ||
            surfaceGrids.ContainsKey(surface))
            return;

        List<LineRenderer> gridLines = new List<LineRenderer>();

        int cellsX = Mathf.FloorToInt(surface.GridWidth / cellSize);
        int cellsZ = Mathf.FloorToInt(surface.GridDepth / cellSize);

        //Creates the grid lines vertically
        for (int i = 0; i <= cellsX; i++)
        {
            float x = i * cellSize;

            CreateGridLine(gridLines,
                GetGridWorldPosition(surface, x, 0f),
                GetGridWorldPosition(surface, x, cellsZ * cellSize));
        }

        //Creates the grid lines horizontally
        for (int i = 0; i <= cellsZ; i++)
        {
            float z = i * cellSize;

            CreateGridLine(gridLines,
                GetGridWorldPosition(surface, 0f, z),
                GetGridWorldPosition(surface, cellsX * cellSize, z));
        }

        surfaceGrids.Add(surface, gridLines);
    }

    //Gets the world position of a grid line
    private Vector3 GetGridWorldPosition(BuildSurface surface, float x, float z)
    {
        return surface.GetWorldPosition(x, z) +
               surface.transform.up * heightOffset;
    }

    //Creates an individual grid line
    private void CreateGridLine(
        List<LineRenderer> gridLines,
        Vector3 startPosition,
        Vector3 endPosition)
    {
        GameObject lineObject = new GameObject("Grid Line");
        lineObject.transform.SetParent(transform);

        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.positionCount = 2;
        line.SetPositions(new[] { startPosition, endPosition });

        line.startWidth = line.endWidth = lineWidth;
        line.sharedMaterial = gridMaterial;
        line.startColor = line.endColor = Color.black;

        line.alignment = LineAlignment.View;
        line.shadowCastingMode = ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.enabled = keepGridVisible;

        gridLines.Add(line);
    }

    //Displays the grid on a surface
    public void ShowGrid(BuildSurface surface)
    {
        if (surface == null)
            return;

        CreateGrid(surface);

        if (!surfaceGrids.TryGetValue(surface, out List<LineRenderer> gridLines))
            return;

        foreach (LineRenderer line in gridLines)
        {
            if (line != null)
                line.enabled = true;
        }
    }

    //Hides the grid
    public void HideGrid()
    {
        if (!keepGridVisible)
            SetAllGridsEnabled(false);
    }

    //Keeps the grids visible during object placement
    public void SetGridVisibility(bool visible)
    {
        keepGridVisible = visible;

        if (visible)
        {
            foreach (BuildSurface surface in FindObjectsByType<BuildSurface>())
            {
                CreateGrid(surface);
            }
        }

        SetAllGridsEnabled(visible);
    }

    //Enables or disables all surface grids
    private void SetAllGridsEnabled(bool enabled)
    {
        foreach (List<LineRenderer> gridLines in surfaceGrids.Values)
        {
            foreach (LineRenderer line in gridLines)
            {
                if (line != null)
                    line.enabled = enabled;
            }
        }
    }

    //Places an object to the centre of the grid cells
    public Vector3 SnapPosition(
        Vector3 position,
        BuildSurface surface,
        Vector2 footprintSize)
    {
        if (surface == null || cellSize <= 0f)
            return position;

        Vector3 localPosition = surface.GetLocalPosition(position);

        int cellsX = Mathf.FloorToInt(surface.GridWidth / cellSize);
        int cellsZ = Mathf.FloorToInt(surface.GridDepth / cellSize);

        int objectCellsX = Mathf.Max(1, Mathf.CeilToInt(footprintSize.x / cellSize));
        int objectCellsZ = Mathf.Max(1, Mathf.CeilToInt(footprintSize.y / cellSize));

        if (objectCellsX > cellsX || objectCellsZ > cellsZ)
            return position;

        int cellX = Mathf.FloorToInt(
            (localPosition.x - surface.LocalGridOrigin.x) / cellSize);

        int cellZ = Mathf.FloorToInt(
            (localPosition.z - surface.LocalGridOrigin.z) / cellSize);

        cellX = Mathf.Clamp(cellX, 0, cellsX - objectCellsX);
        cellZ = Mathf.Clamp(cellZ, 0, cellsZ - objectCellsZ);

        localPosition.x = surface.LocalGridOrigin.x +
                          (cellX + objectCellsX * 0.5f) * cellSize;

        localPosition.z = surface.LocalGridOrigin.z +
                          (cellZ + objectCellsZ * 0.5f) * cellSize;

        return surface.transform.TransformPoint(localPosition);
    }

    private void OnDestroy()
    {
        foreach (List<LineRenderer> gridLines in surfaceGrids.Values)
        {
            foreach (LineRenderer line in gridLines)
            {
                if (line != null)
                    Destroy(line.gameObject);
            }
        }

        surfaceGrids.Clear();

        if (gridMaterial != null)
            Destroy(gridMaterial);
    }
}
