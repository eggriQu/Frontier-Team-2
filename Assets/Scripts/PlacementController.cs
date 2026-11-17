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
    [SerializeField] private Material
    previewMaterial;
    [SerializeField] private Material
    validMaterial;
    [SerializeField] private Material
    invalidMaterial;

    private GameObject selectedPrefab;
    private GameObject previewObject;
    private GameObject footprintIndicator;
    private Renderer footprintRenderer;
    private bool isPlacementValid;

    //Object selection
    public void SelectObject(GameObject prefab)
    {
        if (prefab == null)
        {
            return;

            CancelPlacement();

            selectedPrefab = prefab;

            previewObject = Instantiate(selectedPrefab);

            previewObject.name = selectedPrefab.name + "Preview";

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
                    Material[] materials = new Material[renderer.materials.Length];

                    for (int i = 0; i < renderer.materials.Length; i++)
                    {

                    }
                }
            }
        }

        selectedPrefab = prefab;
        Debug.Log("Selected object: " + prefab.name);
    }

    //Object placement
    private void Update()
    {
        if (selectedPrefab == null || !Input.GetMouseButtonDown(0))
            return;

        if (IsPointerOverButton())
        {
            Debug.Log("Click ignored because the pointer is over a Button.");
            return;
        }

        if (buildCamera == null || groundLayer.value == 0)
        {
            return;
        }

        //Checks if the ray hits the ground layer
        Ray ray = buildCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            Debug.LogWarning("Placement ray did not hit a surface.");
            return;
        }

        //Allows objects to be placed on the ground layer and on other objects
        bool isGround = IsInLayerMask(hit.collider.gameObject, groundLayer);
        bool isPlacedBlock = hit.collider.GetComponentInParent<PlacedBlockMarker>() != null;

        if (!isGround && !isPlacedBlock)
            return;

        GameObject placedObject = Instantiate(
            selectedPrefab,
            hit.point,
            selectedPrefab.transform.rotation
        );

        Collider objectCollider = placedObject.GetComponentInChildren<Collider>();

        if (objectCollider != null)
        {
            float bottomOffset = hit.point.y - objectCollider.bounds.min.y;
            placedObject.transform.position += Vector3.up * bottomOffset;
        }

        //Marks the placed object with a PlacedBlockMarker component to verify it as a valid surface.
        placedObject.AddComponent<PlacedBlockMarker>();

        Debug.Log("Placed " + selectedPrefab.name + " at " + hit.point);
    }

    //Checks if the object is in the specified LayerMask
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
