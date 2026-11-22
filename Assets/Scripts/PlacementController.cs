using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlacementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera buildCamera;
    [SerializeField] private LayerMask groundLayer;

    [Header("Placement")]
    [SerializeField] private float rayDistance = 1000f;

    [Header("Preview")]
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    [Header("Grid")]
    [SerializeField] private GridRenderer gridRenderer;

    private GameObject selectedPrefab;
    private GameObject previewObject;
    private bool isPlacementValid;

    //Select the object
    public void SelectObject(GameObject prefab)
    {
        if (prefab == null)
            return;

        //Cancel the placement of the object if a new object is selected
        CancelPlacement();

        selectedPrefab = prefab;
        previewObject = Instantiate(selectedPrefab);
        previewObject.name = selectedPrefab.name + " Preview";

        foreach (Collider col in previewObject.GetComponentsInChildren<Collider>())
            col.enabled = false;

        //The selected object is not able to be placed until the placement is valid
        isPlacementValid = false;
        UpdatePreviewMaterial();

        if (gridRenderer != null)
            gridRenderer.SetGridVisibility(true);

        Debug.Log("Selected object: " + prefab.name);
    }

    //Object placement
    private void Update()
    {
        if (previewObject == null)
            return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(selectedPrefab.name + " placement cancelled.");
            CancelPlacement();
            return;
        }

        UpdatePreviewPosition();

        if (!Input.GetMouseButtonDown(0))
            return;

        if (IsPointerOverButton())
        {
            Debug.Log("Input ignored because the pointer is over a Button.");
            return;
        }

        if (isPlacementValid)
            PlaceObject();
        else
            Debug.LogWarning("Cannot place " + selectedPrefab.name + " here.");
    }

    private void UpdatePreviewPosition()
    {
        if (buildCamera == null || groundLayer.value == 0)
        {
            isPlacementValid = false;
            UpdatePreviewMaterial();
            return;
        }

        if (IsPointerOverButton())
            return;

        //Checks if the raycast hits the ground layer and checks if the placement is valid
        Ray ray = buildCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            isPlacementValid = false;
            UpdatePreviewMaterial();
            return;
        }

        //Allows objects to be placed on the ground layer and on other placed objects
        bool isGround = IsInLayerMask(hit.collider.gameObject, groundLayer);
        bool isPlacedBlock = hit.collider.GetComponentInParent<PlacedBlockMarker>() != null;

        BuildSurface surface = hit.collider.GetComponentInParent<BuildSurface>();

        isPlacementValid = isGround || isPlacedBlock || surface != null;

        if (surface == null)
        {
            previewObject.transform.position = hit.point;
            previewObject.transform.rotation = selectedPrefab.transform.rotation;

            if (TryGetPreviewBounds(out Bounds bounds))
                previewObject.transform.position += Vector3.up * (hit.point.y - bounds.min.y);

            UpdatePreviewMaterial();
            return;
        }

        //Displays the grid on the surface that the object is being placed on
        if (gridRenderer != null)
            gridRenderer.ShowGrid(surface);

        previewObject.transform.rotation =
            surface.transform.rotation * selectedPrefab.transform.localRotation;

        previewObject.transform.position = hit.point;

        //Calculates the footprint size of the object in local grid coordinates
        Vector2 footprintSize = GetPreviewFootprint(surface);

        //Places the object in the centre of the grid cell
        Vector3 snappedPosition = gridRenderer != null
            ? gridRenderer.SnapPosition(hit.point, surface, footprintSize)
            : hit.point;

        previewObject.transform.position = snappedPosition;

        //Places the object on top of the selected surface
        if (TryGetPreviewBounds(out Bounds previewBounds))
        {
            Vector3 surfaceNormal = surface.transform.up;
            float surfaceHeight = Vector3.Dot(snappedPosition, surfaceNormal);

            previewObject.transform.position += surfaceNormal *
                (surfaceHeight - GetLowestPoint(previewBounds, surface));
        }

        Vector3 localPosition = surface.GetLocalPosition(previewObject.transform.position);

        isPlacementValid = surface.IsInsideSurface(
            localPosition.x - surface.LocalGridOrigin.x,
            localPosition.z - surface.LocalGridOrigin.z
        );

        UpdatePreviewMaterial();
    }

    //Calculates the size of the object's footprint
    private Vector2 GetPreviewFootprint(BuildSurface surface)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return Vector2.one;

        Vector3 localMin = Vector3.one * float.PositiveInfinity;
        Vector3 localMax = Vector3.one * float.NegativeInfinity;

        foreach (Renderer renderer in renderers)
        {
            Bounds bounds = renderer.bounds;

            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = new Vector3(
                    (i & 1) == 0 ? bounds.min.x : bounds.max.x,
                    (i & 2) == 0 ? bounds.min.y : bounds.max.y,
                    (i & 4) == 0 ? bounds.min.z : bounds.max.z
                );

                Vector3 localCorner = surface.GetLocalPosition(corner);
                localMin = Vector3.Min(localMin, localCorner);
                localMax = Vector3.Max(localMax, localCorner);
            }
        }

        return new Vector2(localMax.x - localMin.x, localMax.z - localMin.z);
    }

    //Gets the lowest point of the object that is being placed on the surface
    private float GetLowestPoint(Bounds bounds, BuildSurface surface)
    {
        Vector3 normal = surface.transform.up;
        float lowestPoint = float.PositiveInfinity;

        for (int i = 0; i < 8; i++)
        {
            Vector3 corner = new Vector3(
                (i & 1) == 0 ? bounds.min.x : bounds.max.x,
                (i & 2) == 0 ? bounds.min.y : bounds.max.y,
                (i & 4) == 0 ? bounds.min.z : bounds.max.z
            );

            lowestPoint = Mathf.Min(lowestPoint, Vector3.Dot(corner, normal));
        }

        return lowestPoint;
    }

    //Updates the object's material depending on whether the object placement is valid or not
    private void UpdatePreviewMaterial()
    {
        if (previewObject == null)
            return;

        Material material = isPlacementValid ? validMaterial : invalidMaterial;

        if (material == null)
            return;

        foreach (Renderer renderer in previewObject.GetComponentsInChildren<Renderer>())
        {
            Material[] materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
                materials[i] = material;

            renderer.sharedMaterials = materials;
        }
    }

    //Calculates the bounds of the object
    private bool TryGetPreviewBounds(out Bounds bounds)
    {
        bounds = new Bounds();

        if (previewObject == null)
            return false;

        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return false;

        bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        return true;
    }

    //Object placement
    private void PlaceObject()
    {
        if (previewObject == null || !isPlacementValid)
            return;

        GameObject placedObject = Instantiate(
            selectedPrefab,
            previewObject.transform.position,
            previewObject.transform.rotation
        );

        //Marks the placed object with a PlacedBlockMarker component to identify it as a valid surface
        placedObject.AddComponent<PlacedBlockMarker>();

        Debug.Log("Placed " + selectedPrefab.name + " at " + placedObject.transform.position);
    }

    //Checks if the object is being placed on the specified GroundLayer LayerMask
    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return (layerMask.value & (1 << obj.layer)) != 0;
    }

    //Checks if the pointer is over the UI Buttons
    private bool IsPointerOverButton()
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponentInParent<Button>() != null)
                return true;
        }

        return false;
    }

    //Cancel the placement of the object
    public void CancelPlacement()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
        selectedPrefab = null;
        isPlacementValid = false;

        if (gridRenderer != null)
            gridRenderer.SetGridVisibility(false);
    }
}

public class PlacedBlockMarker : MonoBehaviour
{
}
