using UnityEngine;
using UnityEngine.Rendering;

public class ObjectOutline : MonoBehaviour
{
    private LineRenderer outline;

    private void Awake()
    {
        outline = GetComponent<LineRenderer>();

        if (outline == null)
            outline = gameObject.AddComponent<LineRenderer>();

        outline.useWorldSpace = true;
        outline.loop = false;
        outline.positionCount = 16;
        outline.startWidth = 0.025f;
        outline.endWidth = 0.025f;
        outline.startColor = Color.black;
        outline.endColor = Color.black;
        outline.shadowCastingMode = ShadowCastingMode.Off;
        outline.receiveShadows = false;
        outline.enabled = false;
    }

    public void SetMaterial(Material material)
    {
        outline.sharedMaterial = material;
    }

    private void LateUpdate()
    {
        if (outline.enabled)
            UpdateOutline();
    }

    public void ShowOutline(bool show)
    {
        outline.enabled = show;

        if (show)
            UpdateOutline();
    }

    public void RemoveOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
            Destroy(outline);
        }
    }

    private void UpdateOutline()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds();
        bool hasBounds = false;

        foreach (Renderer renderer in renderers)
        {
            if (renderer == outline)
                continue;

            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
                bounds.Encapsulate(renderer.bounds);
        }

        if (!hasBounds)
            return;

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        Vector3[] corners =
        {
            new Vector3(min.x, min.y, min.z),
            new Vector3(max.x, min.y, min.z),
            new Vector3(max.x, min.y, max.z),
            new Vector3(min.x, min.y, max.z),
            new Vector3(min.x, max.y, min.z),
            new Vector3(max.x, max.y, min.z),
            new Vector3(max.x, max.y, max.z),
            new Vector3(min.x, max.y, max.z)
        };

        int[] lines =
        {
            0, 1, 2, 3, 0, 4, 5, 6,
            7, 4, 5, 1, 2, 6, 7, 3
        };

        for (int i = 0; i < lines.Length; i++)
            outline.SetPosition(i, corners[lines[i]]);
    }
}
