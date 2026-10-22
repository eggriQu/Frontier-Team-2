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
            _grid.TryGetValue(gridPos, out PlaceableObject placeableObj);
            return placeableObj;
        }

        /// <summary>
        /// refreshes piece at specified cell and its neighbours.
        /// </summary>
        /// <param name="gridPos"></param>
        private void NotifySelfAndNeighbours(Vector3Int gridPos)
        {
            if (_grid.TryGetValue(gridPos, out PlaceableObject placeableObj))
                placeableObj.UpdateConnections();

            NotifyNeighboursOnly(gridPos);
        }

        /// <summary>
        /// refreshes connection of neighbours only
        /// </summary>
        /// <param name="gridPos"></param>
        private void NotifyNeighboursOnly(Vector3Int gridPos)
        {
            Vector3Int[] directions = // using cardinal directions here in case we end up having camera rotation
            {
            Vector3Int.forward, // North
            Vector3Int.right, // East
            Vector3Int.back, // South
            Vector3Int.left // West
        };

            foreach (var dir in directions)
            {
                if (_grid.TryGetValue(gridPos + dir, out PlaceableObject neighbour))
                {
                    neighbour.UpdateConnections();
                }
            }
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
    }
}