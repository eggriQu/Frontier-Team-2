using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GridRenderer : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private float heightOffset = 0.02f;

    private readonly Dictionary<BuildSurface, List<LineRenderer>> surfaceGrids =
        new Dictionary<BuildSurface, List<LineRenderer>>();

    private Material gridMaterial;
    private bool keepGridVisible;

    private void Awake()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit")
            ?? Shader.Find("Unlit/Color");

        if (shader != null)
        {
            gridMaterial = new Material(shader);
            gridMaterial.color = Color.black;
        }
    }

    //Creates the grid over a selected surface
    private void CreateGrid(BuildSurface surface)
    {
        if (surface == null || gridMaterial == null || cellSize <= 0f ||
            surfaceGrids.ContainsKey(surface))
            return;

        List<LineRenderer> lines = new List<LineRenderer>();

        int cellsX = Mathf.FloorToInt(surface.GridWidth / cellSize);
        int cellsZ = Mathf.FloorToInt(surface.GridDepth / cellSize);

        //Creates the grid lines vertically
        for (int i = 0; i <= cellsX; i++)
            CreateGridLine(lines, surface, i * cellSize, 0f,
                i * cellSize, cellsZ * cellSize, true);

        //Creates the grid lines horizontally
        for (int i = 0; i <= cellsZ; i++)
            CreateGridLine(lines, surface, 0f, i * cellSize,
                cellsX * cellSize, i * cellSize, true);

        surfaceGrids.Add(surface, lines);
    }

    //Creates an individual grid line
    private void CreateGridLine(List<LineRenderer> lines, BuildSurface surface,
        float startX, float startZ, float endX, float endZ, bool unused)
    {
        Vector3 offset = surface.transform.up * heightOffset;

        GameObject lineObject = new GameObject("Grid Line");
        lineObject.transform.SetParent(transform);

        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.positionCount = 2;
        line.SetPositions(new[]
        {
            surface.GetWorldPosition(startX, startZ) + offset,
            surface.GetWorldPosition(endX, endZ) + offset
        });

        line.startWidth = line.endWidth = lineWidth;
        line.sharedMaterial = gridMaterial;
        line.startColor = line.endColor = Color.black;
        line.alignment = LineAlignment.View;
        line.shadowCastingMode = ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.enabled = keepGridVisible;

        lines.Add(line);
    }

    //Displays the grid on a surface
    public void ShowGrid(BuildSurface surface)
    {
        if (surface == null)
            return;

        CreateGrid(surface);

        if (!surfaceGrids.TryGetValue(surface, out List<LineRenderer> lines))
            return;

        foreach (LineRenderer line in lines)
            if (line != null)
                line.enabled = true;
    }

    //Keeps the grids visible during object placement
    public void SetGridVisibility(bool visible)
    {
        keepGridVisible = visible;

        if (visible)
        {
            foreach (BuildSurface surface in FindObjectsByType<BuildSurface>())
                CreateGrid(surface);
        }

        foreach (List<LineRenderer> lines in surfaceGrids.Values)
            foreach (LineRenderer line in lines)
                if (line != null)
                    line.enabled = visible;
    }

    //Hides the grid
    public void HideGrid()
    {
        if (!keepGridVisible)
            SetGridVisibility(false);
    }

    //Places an object to the centre of the grid cells
    public Vector3 SnapPosition(Vector3 position, BuildSurface surface, Vector2 footprintSize)
    {
        if (surface == null || cellSize <= 0f)
            return position;

        Vector3 local = surface.GetLocalPosition(position);

        int cellsX = Mathf.FloorToInt(surface.GridWidth / cellSize);
        int cellsZ = Mathf.FloorToInt(surface.GridDepth / cellSize);
        int objectX = Mathf.Max(1, Mathf.CeilToInt(footprintSize.x / cellSize));
        int objectZ = Mathf.Max(1, Mathf.CeilToInt(footprintSize.y / cellSize));

        if (objectX > cellsX || objectZ > cellsZ)
            return position;

        int x = Mathf.Clamp(Mathf.FloorToInt(
            (local.x - surface.LocalGridOrigin.x) / cellSize), 0, cellsX - objectX);

        int z = Mathf.Clamp(Mathf.FloorToInt(
            (local.z - surface.LocalGridOrigin.z) / cellSize), 0, cellsZ - objectZ);

        local.x = surface.LocalGridOrigin.x + (x + objectX * 0.5f) * cellSize;
        local.z = surface.LocalGridOrigin.z + (z + objectZ * 0.5f) * cellSize;

        return surface.transform.TransformPoint(local);
    }

    private void OnDestroy()
    {
        foreach (List<LineRenderer> lines in surfaceGrids.Values)
            foreach (LineRenderer line in lines)
                if (line != null)
                    Destroy(line.gameObject);

        surfaceGrids.Clear();

        if (gridMaterial != null)
            Destroy(gridMaterial);
    }
}
