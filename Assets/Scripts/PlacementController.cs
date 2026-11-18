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

    private GameObject selectedPrefab;
    private GameObject previewObject;
    private bool isPlacementValid;

    //Object selection
    public void SelectObject(GameObject prefab)
    {
        if (prefab == null)
            return;

        CancelPlacement();

        selectedPrefab = prefab;

        previewObject = Instantiate(selectedPrefab);
        previewObject.name = selectedPrefab.name + " Preview";

        Collider[] colliders = previewObject.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        isPlacementValid = false;
        UpdatePreviewMaterial();

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

        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverButton())
            {
                Debug.Log("Input ignored because the pointer is over a Button.");
                return;
            }

            if (isPlacementValid)
            {
                PlaceObject();
            }
            else
            {
                Debug.LogWarning("Cannot place " + selectedPrefab.name + " here.");
            }
        }
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

        previewObject.transform.position = hit.point;
        previewObject.transform.rotation = selectedPrefab.transform.rotation;

        Bounds bounds;

        if (TryGetPreviewBounds(out bounds))
        {
            float bottomOffset = hit.point.y - bounds.min.y;
            previewObject.transform.position += Vector3.up * bottomOffset;
        }

        isPlacementValid = isGround || isPlacedBlock;

        UpdatePreviewMaterial();
    }

    //Updates the preview object's material whether the placement is valid or not
    private void UpdatePreviewMaterial()
    {
        if (previewObject == null)
            return;

        Material material = isPlacementValid ? validMaterial : invalidMaterial;

        if (material == null)
            return;

        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }

            renderer.sharedMaterials = materials;
        }
    }

    //Calculates the bounds of the preview object
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
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

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
    }
}

public class PlacedBlockMarker : MonoBehaviour
{
}
