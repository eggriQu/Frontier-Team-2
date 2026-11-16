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
    [SerializeField] private Material previewMaterial;

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

        if (previewMaterial != null)
        {
            Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                Material[] materials = new Material[renderer.sharedMaterials.Length];

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = previewMaterial;
                }

                renderer.sharedMaterials = materials;
            }
        }

        Debug.Log("Selected object: " + prefab.name);
    }

    //Object placement
    private void Update()
    {
        if (previewObject == null)
            return;

        UpdatePreviewPosition();

    }

    private void UpdatePreviewPosition()
    {
        if (buildCamera == null || groundLayer.value == 0)
            return;

        if (IsPointerOverButton())
        {
            isPlacementValid = false;
            return;
        }

        // Checks if the ray hits a valid surface
        Ray ray = buildCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            isPlacementValid = false;
            return;
        }

        //Allows objects to be placed on the ground layer and on placed objects
        bool isGround = IsInLayerMask(hit.collider.gameObject, groundLayer);
        bool isPlacedBlock = hit.collider.GetComponentInParent<PlacedBlockMarker>() != null;

        if (!isGround && !isPlacedBlock)
        {
            isPlacementValid = false;
            return;
        }

        previewObject.transform.position = hit.point;
        previewObject.transform.rotation = selectedPrefab.transform.rotation;

        Collider objectCollider = selectedPrefab.GetComponentInChildren<Collider>();

        if (objectCollider != null)
        {
            float bottomOffset = hit.point.y - objectCollider.bounds.min.y;
            previewObject.transform.position += Vector3.up * bottomOffset;
        }

        isPlacementValid = true;
    }

    //Confirm object placement
    public void ConfirmPlacement()
    {
        if (previewObject == null || !isPlacementValid)
            return;

        GameObject placedObject = Instantiate(
            selectedPrefab,
            previewObject.transform.position,
            previewObject.transform.rotation
        );

        //Mark the placed object with a PlacedBlockMarker component to identify it as a valid surface
        placedObject.AddComponent<PlacedBlockMarker>();

        Debug.Log("Placed " + selectedPrefab.name + " at " + placedObject.transform.position);

        CancelPlacement();
    }

    //Cancel object placement
    public void CancelPlacement()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = null;
        selectedPrefab = null;
        isPlacementValid = false;
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
}

public class PlacedBlockMarker : MonoBehaviour
{
}
