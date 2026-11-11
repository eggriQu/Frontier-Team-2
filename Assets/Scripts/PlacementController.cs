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
            Debug.Log("Input ignored because the pointer is over a Button.");
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

        //Allows objects to be placed on the ground layer and on placed objects
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

        //Mark the placed object with a PlacedBlockMarker component to identify it as a valid surface
        placedObject.AddComponent<PlacedBlockMarker>();

        Debug.Log("Placed " + selectedPrefab.name + " at " + hit.point);
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
