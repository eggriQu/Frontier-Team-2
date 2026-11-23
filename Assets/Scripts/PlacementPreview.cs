using UnityEngine;

public class PlacementPreview : MonoBehaviour
{
    [Header("Preview")]
    [SerializeField] private Material validMaterial, invalidMaterial;
    private GameObject previewObject, previewSource;
    private bool isPlacementValid;
    public GameObject PreviewObject => previewObject;
    public bool IsPlacementValid => isPlacementValid;

    //Creates the object preview
    public void CreatePreview(GameObject prefab)
    {
        if (prefab == null) return;

        DestroyPreview();

        previewSource = prefab;
        previewObject = Instantiate(prefab);
        previewObject.name = prefab.name + " Preview";

        //Removes the object outline and disables colliders on the preview object
        foreach (ObjectOutline objectOutline in previewObject.GetComponentsInChildren<ObjectOutline>())
        {
            objectOutline.RemoveOutline();
            Destroy(objectOutline);
        }

        foreach (Collider col in previewObject.GetComponentsInChildren<Collider>())
            col.enabled = false;

        SetPlacementValidity(false);
    }

    //Updates the preview position
    public void UpdatePreviewPosition(Camera buildCamera, LayerMask groundLayer, float rayDistance, GridRenderer gridRenderer)
    {
        if (previewObject == null) return;
        if (buildCamera == null || groundLayer.value == 0 ||
            !Physics.Raycast(buildCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, rayDistance))
        {
            SetPlacementValidity(false);
            return;
        }
        //Allows objects to be placed on the ground layer and on other placed objects
        bool isGround = IsInLayerMask(hit.collider.gameObject, groundLayer);
        bool isPlacedBlock = hit.collider.GetComponentInParent<PlaceBlockMarker>() != null;
        BuildSurface surface = hit.collider.GetComponentInParent<BuildSurface>();
        if (surface == null)
            previewObject.transform.SetPositionAndRotation(hit.point, previewSource.transform.rotation);
        else
        {
            //Displays the grid on the surface that the object is being placed on
            gridRenderer?.ShowGrid(surface);
            previewObject.transform.SetPositionAndRotation(hit.point,
                surface.transform.rotation * previewSource.transform.localRotation);
        }
        Renderer[] renderers = GetPreviewRenderers(out Bounds bounds);
        if (surface == null)
        {
            if (renderers.Length > 0)
                previewObject.transform.position += Vector3.up * (hit.point.y - bounds.min.y);
            SetPlacementValidity(isGround || isPlacedBlock);
            return;
        }
        //Calculates the footprint size of the object in local grid coordinates
        Vector2 footprint = GetPreviewFootprint(surface, renderers);
        Vector3 position = gridRenderer != null ? gridRenderer.SnapPosition(hit.point, surface, footprint) : hit.point;
        previewObject.transform.position = position;
        //Places the object on top of the selected surface
        if (renderers.Length > 0)
        {
            Vector3 normal = surface.transform.up;
            previewObject.transform.position += normal *
                (Vector3.Dot(position, normal) - GetLowestPoint(bounds, normal));
        }
        Vector3 local = surface.GetLocalPosition(previewObject.transform.position);
        SetPlacementValidity(surface.IsInsideSurface(
            local.x - surface.LocalGridOrigin.x, local.z - surface.LocalGridOrigin.z));
    }

    //Calculates the size of the object's footprint
    private Vector2 GetPreviewFootprint(BuildSurface surface, Renderer[] renderers)
    {
        if (renderers.Length == 0) return Vector2.one;
        Vector3 min = Vector3.one * float.PositiveInfinity;
        Vector3 max = Vector3.one * float.NegativeInfinity;
        foreach (Renderer renderer in renderers)
        {
            Bounds b = renderer.bounds;
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = new Vector3((i & 1) == 0 ? b.min.x : b.max.x,
                    (i & 2) == 0 ? b.min.y : b.max.y, (i & 4) == 0 ? b.min.z : b.max.z);
                Vector3 local = surface.GetLocalPosition(corner);
                min = Vector3.Min(min, local);
                max = Vector3.Max(max, local);
            }
        }
        return new Vector2(max.x - min.x, max.z - min.z);
    }

    //Gets the lowest point of the object that is being placed on the surface
    private float GetLowestPoint(Bounds bounds, Vector3 normal)
    {
        float lowest = float.PositiveInfinity;
        for (int i = 0; i < 8; i++)
        {
            Vector3 corner = new Vector3((i & 1) == 0 ? bounds.min.x : bounds.max.x,
                (i & 2) == 0 ? bounds.min.y : bounds.max.y, (i & 4) == 0 ? bounds.min.z : bounds.max.z);
            lowest = Mathf.Min(lowest, Vector3.Dot(corner, normal));
        }
        return lowest;
    }

    //Gets the preview renderers and calculates the combined bounds
    private Renderer[] GetPreviewRenderers(out Bounds bounds)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        bounds = new Bounds();
        if (renderers.Length == 0) return renderers;
        bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
        return renderers;
    }

    //Updates the placement validity and preview material
    private void SetPlacementValidity(bool valid)
    {
        isPlacementValid = valid;
        if (previewObject == null) return;
        Material material = valid ? validMaterial : invalidMaterial;
        if (material == null) return;
        foreach (Renderer renderer in previewObject.GetComponentsInChildren<Renderer>())
        {
            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++) materials[i] = material;
            renderer.sharedMaterials = materials;
        }
    }

    //Checks if the object is on the specified GroundLayer LayerMask
    private bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return (mask.value & (1 << obj.layer)) != 0;
    }

    //Destroys the preview object
    public void DestroyPreview()
    {
        if (previewObject != null) Destroy(previewObject);
        previewObject = previewSource = null;
        isPlacementValid = false;
    }
}
