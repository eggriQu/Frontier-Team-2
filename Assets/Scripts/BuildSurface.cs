using UnityEngine;

public class BuildSurface : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float gridWidth = 10f;
    [SerializeField] private float gridDepth = 10f;
    [SerializeField] private Vector3 localGridOrigin = new Vector3(-5f, 0f, -5f);

    public float GridWidth => gridWidth;
    public float GridDepth => gridDepth;
    public Vector3 LocalGridOrigin => localGridOrigin;

    //Converts a local grid position into a world position
    public Vector3 GetWorldPosition(float x, float z)
    {
        return transform.TransformPoint(
            localGridOrigin + new Vector3(x, 0f, z)
        );
    }

    //Converts a world position into a local grid position
    public Vector3 GetLocalPosition(Vector3 worldPosition)
    {
        return transform.InverseTransformPoint(worldPosition);
    }

    //Checks whether the grid position is inside the surface
    public bool IsInsideSurface(float x, float z)
    {
        return x >= 0f &&
               z >= 0f &&
               x <= gridWidth &&
               z <= gridDepth;
    }
}
