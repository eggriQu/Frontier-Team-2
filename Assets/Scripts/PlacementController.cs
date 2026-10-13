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
    [SerializeField] private float overlapPadding = 0.01f;

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

            //Check if the object has any colliders
            Collider[] objectColliders =
                placedObject.GetComponentsInChildren<Collider>();

            if (objectColliders.Length == 0)
            {
                Destroy(placedObject);
                Debug.LogWarning("The selected object has no Collider.");
                return;
            }

            //Finds the lowest point of the object's colliders
            float lowestPoint = float.PositiveInfinity;

            foreach (Collider objectCollider in objectColliders)
            {
                if (!objectCollider.isTrigger)
                {
                    lowestPoint = Mathf.Min(
                        lowestPoint,
                        objectCollider.bounds.min.y
                    );
                }
            }

            //Remove the object if it has no solid colliders
            if (float.IsPositiveInfinity(lowestPoint))
            {
                Destroy(placedObject);
                Debug.LogWarning("The selected object has no solid Collider.");
                return;
            }

            //Moves the object vertically so that its lowest point hits the surface
            float bottomOffset = hit.point.y - lowestPoint;

            placedObject.transform.position +=
                Vector3.up * bottomOffset;

            //Tracks whether the object overlaps with an existing colliders
            bool overlaps = false;

            foreach (Collider objectCollider in objectColliders)
            {
                if (objectCollider.isTrigger)
                    continue;

                //Check for overlaps with other colliders
                Bounds bounds = objectCollider.bounds;

                Vector3 halfExtents = bounds.extents -
                    Vector3.one * overlapPadding;

                //Ensures that the half extents are not negative
                halfExtents = Vector3.Max(
                    halfExtents,
                    Vector3.one * 0.001f
                );

                //Check for overlaps with other colliders using OverlapBox
                Collider[] overlappingColliders =
                    Physics.OverlapBox(
                        bounds.center,
                        halfExtents,
                        objectCollider.transform.rotation,
                        ~groundLayer,
                        QueryTriggerInteraction.Ignore
                    );

                //Check if any of the overlapping colliders are not part of the placed object
                foreach (Collider other in overlappingColliders)
                {
                    if (other.transform.IsChildOf(placedObject.transform))
                        continue;

                    if (other == objectCollider)
                        continue;

                    if (other.transform.IsChildOf(transform))
                        continue;

                    overlaps = true;
                    break;
                }

                if (overlaps)
                    break;
            }

            //Remove the object if it overlaps with another object
            if (overlaps)
            {
                Destroy(placedObject);
                Debug.Log("Placement blocked: the object would overlap another object.");
                return;
            }

            Debug.Log("Placed " + selectedPrefab.name +
                      " at " + placedObject.transform.position);
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
