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

    private GameObject selectedPrefab;

    //Object selection
    public void SelectObject(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("No build prefab was assigned.");
            return;
        }

        selectedPrefab = prefab;
        Debug.Log("Selected object: " + prefab.name);
    }

    //Object placement
    private void Update()
    {
        if (selectedPrefab == null)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        //Debugging
        if (IsPointerOverButton())
        {
            Debug.Log("Click ignored because the pointer is over a Button.");
            return;
        }

        if (buildCamera == null)
        {
            Debug.LogError("PlacementController: Build Camera is not assigned.");
            return;
        }

        if (groundLayer.value == 0)
        {
            Debug.LogError("PlacementController: Ground Layer is set to Nothing.");
            return;
        }

        //Check if the ray hits the ground layer
        Ray ray = buildCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit,
            rayDistance, groundLayer))
        {
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

            Debug.Log("Placed " + selectedPrefab.name +
                      " at " + hit.point);
        }
        else
        {
            Debug.LogWarning(
                "Placement ray did not hit the Ground layer. " +
                "Check the Plane's Collider and Layer."
            );
        }
    }
    //Check if the pointer is over the UI Buttons
    private bool IsPointerOverButton()
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData pointerData =
            new PointerEventData(EventSystem.current);

        pointerData.position = Input.mousePosition;

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
