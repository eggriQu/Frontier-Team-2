using System.Collections.Generic;
using UnityEngine;

namespace Placement
{
    /// <summary>
    /// Tracks placed objects by grid position, and updates connection state
    /// when pieced are added or removed.
    /// </summary>
    public class GridSystem : MonoBehaviour
    {
        public static GridSystem Instance { get; private set; } // shared access point for the active grid system

        [SerializeField] private LayerMask obstacleLayer;         //  this layer is used to check overlapping objects when placing

        // maps each occupied grid cell to the object placed there
        private Dictionary<Vector3Int, PlaceableObject> _grid = new Dictionary<Vector3Int, PlaceableObject>();

        private PlaceableObject _previewPlaceableObject;
        private Vector3Int _previewGridPosition;
        private bool _hasPreview;

        private static readonly Vector3Int[] CardinalDirections =
        {
            Vector3Int.forward, //  North   (+z)    bit 1
            Vector3Int.right, //    East    (+x)    bit 2
            Vector3Int.back, //     South   (-z)    bit 4
            Vector3Int.left //      West    (-x)    bit 8
        };

        private void Awake() => Instance = this;    // set up singleton
        
        /// <summary>
        /// converts world space position to nearest grid cell.
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public static Vector3Int WorldToGridPos(Vector3 worldPos)
        {
            return new Vector3Int(
                Mathf.RoundToInt(worldPos.x),
                Mathf.RoundToInt(worldPos.y),
                Mathf.RoundToInt(worldPos.z)
                );
        }

        /// <summary>
        /// creates a placeable at the grid cell rounded from given world position.
        /// returns false if cell already occupied.
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="placeablePrefab"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public bool PlacePiece(Vector3 worldPos, PlaceableObject placeablePrefab, Quaternion rotation)
        {
            Vector3Int gridPos = WorldToGridPos(worldPos);  // world position to grid position

            if (_grid.ContainsKey(gridPos)) return false; // grid position already occupied

            PlaceableObject newPlaceable = Instantiate(placeablePrefab, gridPos, rotation);
            newPlaceable.GridPosition = gridPos;    // set its internal grid pos
            _grid.Add(gridPos, newPlaceable);    // updates the grid with the position of the new placeable

            SetLayerRecursively(newPlaceable.gameObject, 6);    //  set layer for everything on a gameObject

            // check self and neighbours for connections
            NotifySelfAndNeighbours(gridPos);
            return true;
        }

        /// <summary>
        /// removes piece at specified grid cell, if one is present.
        /// then refreshes neighbours' connections.
        /// </summary>
        /// <param name="gridPos"></param>
        public void RemovePiece(Vector3Int gridPos)
        {
            if (!_grid.TryGetValue(gridPos, out PlaceableObject placeableObj)) return;

            _grid.Remove(gridPos);
            Destroy(placeableObj.gameObject);

            // make neighbours check connections
            NotifyNeighboursOnly(gridPos);
        }

        /// <summary>
        /// Returns the piece at specified grid cell
        /// Or returns null if cell is empty.
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        public PlaceableObject GetPieceAt(Vector3Int gridPos)
        {
            if (_grid.TryGetValue(gridPos, out PlaceableObject placeableObj))
            {
                return placeableObj;
            }

            if (_hasPreview && _previewGridPosition == gridPos)
            {
                return _previewPlaceableObject;
            }

            return null;
        }

        /// <summary>
        /// refreshes piece at specified cell and its neighbours.
        /// </summary>
        /// <param name="gridPos"></param>
        private void NotifySelfAndNeighbours(Vector3Int gridPos)
        {
            if (_grid.TryGetValue(gridPos, out PlaceableObject placeableObj))
            {
                RefreshPieceConnections(gridPos, placeableObj);
            }

            NotifyNeighboursOnly(gridPos);
        }

        /// <summary>
        /// refreshes connection of neighbours only
        /// </summary>
        /// <param name="gridPos"></param>
        private void NotifyNeighboursOnly(Vector3Int gridPos)
        {
            for (int i = 0; i < CardinalDirections.Length; i++)
            {
                Vector3Int neighbourPos = gridPos + CardinalDirections[i];
                if (_grid.TryGetValue(neighbourPos, out PlaceableObject neighbour))
                {
                    RefreshPieceConnections(neighbourPos, neighbour);
                }
                else if (_hasPreview && _previewGridPosition == neighbourPos && _previewPlaceableObject != null)
                {
                    RefreshPieceConnections(neighbourPos, _previewPlaceableObject);
                }
            }
        }

        /// <summary>
        /// public method to notify neighbours only
        /// </summary>
        /// <param name="gridPos"></param>
        public void NotifyNeighbours(Vector3Int gridPos)
        {
            NotifyNeighboursOnly(gridPos);
        }

        /// <summary>
        /// sets specified layer on a GameObject and all its child transforms.
        /// </summary>
        /// <param name="currentObject"></param>
        /// <param name="layer"></param>
        private void SetLayerRecursively(GameObject currentObject, int layer)   // sets gameObject's and all it's children's layers
        {
            currentObject.layer = layer;
            foreach (Transform child in currentObject.transform)
                SetLayerRecursively(child.gameObject, layer);
        }
        
        public void SetPreviewAt(Vector3Int gridPos, PlaceableObject preview)
        {
            _previewPlaceableObject = preview;
            _previewGridPosition = gridPos;
            _hasPreview = preview != null;
        }

        public void ClearPreview()
        {
            _previewPlaceableObject = null;
            _hasPreview = false;
        }

        public void RefreshPieceConnections(Vector3Int gridPos, PlaceableObject target)
        {
            if (target == null) return;

            int bitmask = 0;
            for (int i = 0; i < CardinalDirections.Length; i++)
            {
                PlaceableObject neighbour = GetPieceAt(gridPos + CardinalDirections[i]);
                if (neighbour != null && neighbour.placeableCategory == target.placeableCategory)
                {
                    bitmask |= 1 << i;
                }
            }
            target.UpdateConnections(bitmask);
        }
        
        public bool IsCellOccupied(Vector3Int gridPos) => _grid.ContainsKey(gridPos);

        /// <summary>
        /// Debug method to print every value in the _grid dictionary
        /// </summary>
        private void PrintGridDict()
        {
            foreach (var obj in _grid)
            {
                Debug.Log($"Obj key = {obj.Key}\n");
            }
            Debug.Log("End");
        }
    }
}