using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlacementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera buildCamera;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GridRenderer gridRenderer;
    [SerializeField] private PlacementIndicator placementPreview;
    [SerializeField] private BlockSelection placeBlockSelection;
    [Header("Placement")]
    [SerializeField] private float rayDistance = 1000f;
    private GameObject selectedPrefab, movingObject;

    //Select the object
    public void SelectObject(GameObject prefab)
    {
        if (prefab == null) return;
        CancelPlacement();
        placeBlockSelection.ClearSelection();
        selectedPrefab = prefab;
        placementPreview.CreatePreview(prefab);
        gridRenderer?.SetGridVisibility(true);
        Debug.Log("Selected object: " + prefab.name);
    }

    //Object placement and selection
    private void Update()
    {
        bool overButton = IsPointerOverButton();
        if (placementPreview.PreviewObject != null)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log(selectedPrefab.name + " placement cancelled.");
                CancelPlacement();
                return;
            }
            if (!overButton)
                placementPreview.UpdatePreviewPosition(buildCamera, groundLayer, rayDistance, gridRenderer);
            if (Input.GetMouseButtonDown(0) && !overButton)
            {
                if (placementPreview.IsPlacementValid) PlaceObject();
                else Debug.LogWarning("Cannot place " + selectedPrefab.name + " here.");
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            placeBlockSelection.RemoveSelectedObject();
            return;
        }
        if (Input.GetMouseButtonDown(0) && !overButton)
        {
            BlockMarker marker = placeBlockSelection.HandleObjectSelection(buildCamera, rayDistance);
            if (marker != null) StartMovingObject(marker);
        }
    }

    //Allows a selected object to be moved again
    private void StartMovingObject(BlockMarker marker)
    {
        movingObject = marker.gameObject;
        selectedPrefab = movingObject;
        placementPreview.CreatePreview(movingObject);
        movingObject.SetActive(false);
        gridRenderer?.SetGridVisibility(true);
        Debug.Log("Moving object: " + movingObject.name);
    }

    //Object placement
    private void PlaceObject()
    {
        GameObject preview = placementPreview.PreviewObject;
        if (preview == null || !placementPreview.IsPlacementValid) return;
        if (movingObject != null)
        {
            movingObject.transform.SetPositionAndRotation(preview.transform.position, preview.transform.rotation);
            movingObject.SetActive(true);
            BlockMarker marker = movingObject.GetComponent<BlockMarker>();
            placementPreview.DestroyPreview();
            movingObject = selectedPrefab = null;
            gridRenderer?.SetGridVisibility(false);
            placeBlockSelection.SetSelectedObject(marker);
            return;
        }
        GameObject placed = Instantiate(selectedPrefab, preview.transform.position, preview.transform.rotation);
        if (placed.GetComponent<BlockMarker>() == null) placed.AddComponent<BlockMarker>();
        Debug.Log("Placed " + selectedPrefab.name + " at " + placed.transform.position);
        CancelPlacement();
    }

    //Checks if the pointer is over the UI Buttons
    private bool IsPointerOverButton()
    {
        if (EventSystem.current == null) return false;
        PointerEventData data = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        foreach (RaycastResult result in results)
            if (result.gameObject.GetComponentInParent<Button>() != null) return true;
        return false;
    }

    //Cancel the placement of the object
    public void CancelPlacement()
    {
        if (placementPreview != null) placementPreview.DestroyPreview();
        if (movingObject != null) movingObject.SetActive(true);
        movingObject = selectedPrefab = null;
        gridRenderer?.SetGridVisibility(false);
    }
}
