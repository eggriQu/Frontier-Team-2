using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Placement
{
    /// <summary>
    /// Handles player input, preview placeable positioning, placement validation,
    /// and delegating piece placement to GridSystem
    /// </summary>
    public class BuildingPlacer : MonoBehaviour
    {
        [Header("Dependencies")] [SerializeField]
        private GridSystem gridSystem;

        [SerializeField] private GameManager gameManager;
        [SerializeField] private BasePlaceable placeable;

        [Header("Collision Layers")] [SerializeField]
        private LayerMask obstacleLayer; //  this layer is used to check overlapping objects when placing

        [SerializeField]
        private LayerMask groundLayer; //  this layer is used to ray cast to the ground when placing objects

        private Camera _mainCamera;
        private InputAction _pointerAction;


        private GameObject _currentPreviewObject; // current object being placed
        private PlaceableObject _previewPlaceableComponent;
        private BoxCollider _previewCollider; // box collider of the root game object when placing
        private Vector3Int? _lastPreviewGridPos;

        private Quaternion
            _currentRotation =
                Quaternion.identity; // its current rotation, stored so it doesn't reset when placing a new object

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (gridSystem == null)
            {
                gridSystem = GridSystem.Instance;
            }

            _pointerAction = InputSystem.actions.FindAction("Pointer");
        }

        private void Start()
        {
            SpawnPreview();
        }

        private void OnEnable()
        {
            _pointerAction?.Enable();
        }

        private void OnDisable()
        {
            _pointerAction?.Disable();
        }

        private void Update()
        {
            if (_pointerAction != null)
            {
                UpdatePreviewPosition(_pointerAction.ReadValue<Vector2>());
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                TryPlaceCurrentPiece();
            }

            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                RotatePreview();
            }
        }

        public void SetPlaceable(GameObject placeableGameObject)
        {
            if (placeableGameObject == null || !placeableGameObject.TryGetComponent(out BasePlaceable newPlaceable))
            {
                return;
            }

            placeable = newPlaceable;
            DestroyPreview();
            SpawnPreview();
        }

        private void SpawnPreview()
        {
            if (placeable == null || placeable.Prefab == null)
            {
                return;
            }

            //  create the preview from the prefab configured on the selected placeable.
            _currentPreviewObject = Instantiate(placeable.Prefab, Vector3.zero, _currentRotation);
            _previewCollider = _currentPreviewObject.GetComponent<BoxCollider>();
            _previewPlaceableComponent = _currentPreviewObject.GetComponent<PlaceableObject>();
            _lastPreviewGridPos = null;
        }

        private void DestroyPreview()
        {
            gridSystem.ClearPreview();
            if (_currentPreviewObject != null)
            {
                Destroy(_currentPreviewObject);
                _currentPreviewObject = null;
                _previewPlaceableComponent = null;
                _previewCollider = null;
            }

            _lastPreviewGridPos = null;
        }

        private void UpdatePreviewPosition(Vector2 mousePos)
        {
            if (_currentPreviewObject == null || _mainCamera == null)
            {
                return;
            }

            Ray ray = _mainCamera.ScreenPointToRay(mousePos);
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                return;
            }

            Vector3Int currentGridPos = GridSystem.WorldToGridPos(hit.point);
            if (_lastPreviewGridPos == currentGridPos)
            {
                return;
            }

            Vector3Int? previousGridPos = _lastPreviewGridPos;
            _lastPreviewGridPos = currentGridPos;
            _currentPreviewObject.transform.position = currentGridPos;

            if (_previewPlaceableComponent != null)
            {
                _previewPlaceableComponent.GridPosition = currentGridPos;
                gridSystem.SetPreviewAt(currentGridPos, _previewPlaceableComponent);

                if (previousGridPos.HasValue)
                {
                    gridSystem.NotifyNeighbours(previousGridPos.Value);
                }

                gridSystem.NotifyNeighbours(currentGridPos);
                gridSystem.RefreshPieceConnections(currentGridPos, _previewPlaceableComponent);
            }
        }

        private void TryPlaceCurrentPiece()
        {
            if (_currentPreviewObject == null || placeable == null || !_lastPreviewGridPos.HasValue)
            {
                return;
            }

            if (!placeable.Prefab.TryGetComponent(out PlaceableObject prefabPlaceable))
            {
                Debug.LogError($"Prefab assigned in BasePlaceable is missing a PlaceableObject component");
                return;
            }

            Vector3Int targetGridPos = _lastPreviewGridPos.Value;
            if (!CanPlaceAt(targetGridPos))
            {
                return;
            }

            gridSystem.ClearPreview();

            if (gridSystem.PlacePiece(targetGridPos, prefabPlaceable, _currentRotation))
            {
                if (gameManager != null)
                {
                    gameManager.Money -= placeable.Cost;
                }
            }

            _lastPreviewGridPos = null;
        }

        private bool CanPlaceAt(Vector3Int gridPos)
        {
            if (gridSystem.IsCellOccupied(gridPos))
            {
                return false;
            }

            if (IsBlockedByObstacle())
            {
                return false;
            }

            if (gameManager != null && gameManager.Money < placeable.Cost)
            {
                return false;
            }

            return true;
        }

        private bool IsBlockedByObstacle()
        {
            if (_previewCollider == null || _currentPreviewObject == null)
            {
                return false;
            }

            Vector3 worldCenter = _currentPreviewObject.transform.TransformPoint(_previewCollider.center);
            Vector3 halfExtents =
                Vector3.Scale(_previewCollider.size * 0.5f, _currentPreviewObject.transform.lossyScale) * 0.95f;

            return Physics.CheckBox(worldCenter, halfExtents, _currentRotation, obstacleLayer,
                QueryTriggerInteraction.Ignore);
        }

        private void RotatePreview()
        {
            _currentRotation *= Quaternion.Euler(0f, 90f, 0f);
            if (_currentPreviewObject != null)
            {
                _currentPreviewObject.transform.rotation = _currentRotation;
            }
        }
    }
}