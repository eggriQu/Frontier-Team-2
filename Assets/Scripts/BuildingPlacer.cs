using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    // test var
    [SerializeField] private BasePlaceable[] placeables;      //  Array of placeable objects
    [SerializeField] private BasePlaceable testPlaceable;     //  single placeable object for testing purposes
    [SerializeField] private LayerMask obstacleLayer;         //  this layer is used to check overlapping objects when placing
    [SerializeField] private LayerMask groundLayer;           //  this layer is used to ray cast to the ground when placing objects
    
    private InputAction _pointerAction;
    private GameObject _currentObject;       // current object being placed

    private Quaternion _currentRotation;     // its current rotation, stored so it doesn't reset when placing a new object
    private BoxCollider _boxCollider;       // box collider of the root game object when placing

    private void Start()
    {
        _pointerAction = InputSystem.actions.FindAction("Pointer");
        SelectNextPrefab();
    }

    private void Update()
    {
        //OnClick(clickAction.ReadValue<bool>());
        OnPointer(_pointerAction.ReadValue<Vector2>());
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnMouseDown();
        }

        if (Keyboard.current != null & Keyboard.current.rKey.wasPressedThisFrame)
        {
            OnRotatePressed();
        }
    }

    private void SelectNextPrefab() // should handle logic for selecting and instantiating a placeable object
    {
        _currentObject = Instantiate(testPlaceable.Prefab); // replace with selecting next object in list.
        _boxCollider = _currentObject.GetComponent<BoxCollider>();
    }

    public void OnPointer(Vector2 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer))
        {
            Vector3 rayHit = new(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));

            _currentObject.transform.position = rayHit;
            _currentObject.transform.rotation = Quaternion.Euler(0f, _currentRotation.y, 0f);
        }
    }

    private void OnMouseDown()  // handles logic for finalising placement (not blocked, enough money, etc.)
    {
        // check for collision here
        if (IsBuildingBlocked())
        {
            Debug.Log("Placement overlapping with another object!");
            return;
        }
        
        if (gameManager.Money < testPlaceable.Cost)
        {
            Debug.Log("Not enough money!");
            return;
        }
        
        SetLayerRecursively(_currentObject, 6);
        
        
        gameManager.Money -= testPlaceable.Cost;
        //gameManager.Income += testPlaceable.Income;
        
        SelectNextPrefab();
    }

    private void SetLayerRecursively(GameObject currentObject, int layer)   // sets gameObject's and all it's children's layers
    {
        currentObject.layer = layer;
        foreach (Transform child in currentObject.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    private bool IsBuildingBlocked()
    {
        if (_boxCollider == null)
        {
            Debug.LogWarning("Box collider null!");
            return false;
        }
        
        // get world space center and half extents
        Vector3 worldCenter = _currentObject.transform.TransformPoint(_boxCollider.center);
        
        // scale half extents using lossy scale so its slightly smaller
        // so it doesn't prevent adjacent placement
        Vector3 lossyScale = _currentObject.transform.lossyScale;
        Vector3 halfExtents = Vector3.Scale(_boxCollider.size * 0.5f, lossyScale) * 0.95f;
        
        return Physics.CheckBox(
            worldCenter,
            halfExtents,
            _currentObject.transform.rotation,
            obstacleLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    private void OnRotatePressed()
    {
        _currentRotation.y += 90f;
        if (_currentRotation.y >= 360f)
            _currentRotation.y = 0f;
    }
}