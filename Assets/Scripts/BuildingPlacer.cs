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
    private GameObject _currentPreviewObject;       // current object being placed
    private PlaceableObject _previewPlaceableComponent;

    private Quaternion _currentRotation = Quaternion.identity;     // its current rotation, stored so it doesn't reset when placing a new object
    private BoxCollider _boxCollider;       // box collider of the root game object when placing

    private void Start()
    {
        _pointerAction = InputSystem.actions.FindAction("Pointer");
        //SelectNextPrefab();
        SpawnPreview();
    }

    private void Update()
    {
        _pointerAction.Enable();
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

    //private void SelectNextPrefab() // should handle logic for selecting and instantiating a placeable object
    //{
    //    _currentObject = Instantiate(testPlaceable.Prefab); // replace with selecting next object in list.
    //    _boxCollider = _currentObject.GetComponent<BoxCollider>();
    //}

    private void SpawnPreview()
    {
        if (testPlaceable == null || testPlaceable.Prefab == null) return;
        
        //  instantiate the prefab from the BasePlaceable scriptable object
        _currentPreviewObject = Instantiate(testPlaceable.Prefab);
        _boxCollider = _currentPreviewObject.GetComponent<BoxCollider>();
        _previewPlaceableComponent = _currentPreviewObject.GetComponent<PlaceableObject>();
        
        // apply current rotation to the preview
        _currentPreviewObject.transform.rotation = _currentRotation;
        
        if (_previewPlaceableComponent != null)
        {
            _previewPlaceableComponent.GridPosition = new Vector3Int(
                Mathf.RoundToInt(_currentPreviewObject.transform.position.x),
                Mathf.RoundToInt(_currentPreviewObject.transform.position.y),
                Mathf.RoundToInt(_currentPreviewObject.transform.position.z)
            );
            
            _previewPlaceableComponent.UpdateConnections();
        }
    }

    public void OnPointer(Vector2 mousePos)
    {
        if (_currentPreviewObject == null) return;
        
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer))
        {
            //  round transform to nearest int for grid coords
            Vector3 rayHit = new(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));

            //_currentObject.transform.position = rayHit;
            //_currentObject.transform.rotation = Quaternion.Euler(0f, _currentRotation.y, 0f);
            _currentPreviewObject.transform.position = rayHit;

            Vector3Int currentGridPos = new Vector3Int(
                Mathf.RoundToInt(rayHit.x),
                Mathf.RoundToInt(rayHit.y),
                Mathf.RoundToInt(rayHit.z));

            if (_previewPlaceableComponent != null)
            {
                _previewPlaceableComponent.GridPosition = currentGridPos;
                _previewPlaceableComponent.UpdateConnections();
            }
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
        
        if (gameManager != null && gameManager.Money < testPlaceable.Cost)
        {
            Debug.Log("Not enough money!");
            return;
        }

        Vector3 placementPos = _currentPreviewObject.transform.position;

        PlaceableObject placeableComponent = testPlaceable.Prefab.GetComponent<PlaceableObject>();
        if (placeableComponent == null)
        {
            Debug.LogError("Prefab assinged in BasePlaceable is missing the PlaceableObject component.");
            return;
        }
        
        // destroy preview ghost
        Destroy(_currentPreviewObject);
        
        // register the placement with the GridSystem
        bool success = GridSystem.Instance.PlacePiece(placementPos, placeableComponent, _currentRotation);

        if (success)
        {
            if (gameManager != null)
                gameManager.Money -= testPlaceable.Cost;
            
            SpawnPreview(); // spawn next preview ghost
        }

        else
        {
            Debug.Log("Position already occupied.");
            SpawnPreview();
        }
    }

    // moved to GridSystem.cs
    // private void SetLayerRecursively(GameObject currentObject, int layer)   // sets gameObject's and all it's children's layers
    // {
    //     currentObject.layer = layer;
    //     foreach (Transform child in currentObject.transform)
    //         SetLayerRecursively(child.gameObject, layer);
    // }

    private bool IsBuildingBlocked()
    {
        if (_boxCollider == null || _currentPreviewObject == null) return false;
        
        // get world space center and half extents
        Vector3 worldCenter = _currentPreviewObject.transform.TransformPoint(_boxCollider.center);
        
        // scale half extents using lossy scale so its slightly smaller
        // so it doesn't prevent adjacent placement
        Vector3 lossyScale = _currentPreviewObject.transform.lossyScale;
        Vector3 halfExtents = Vector3.Scale(_boxCollider.size * 0.5f, lossyScale) * 0.95f;
        
        return Physics.CheckBox(
            worldCenter,
            halfExtents,
            _currentRotation,
            obstacleLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    private void OnRotatePressed()
    {
        //_currentRotation.y += 90f;
        //if (_currentRotation.y >= 360f)
        //    _currentRotation.y = 0f;

        _currentRotation *= Quaternion.Euler(0f, 90f, 0f);

        if (_currentPreviewObject != null)
            _currentPreviewObject.transform.rotation = _currentRotation;
    }
}