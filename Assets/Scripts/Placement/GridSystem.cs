using System.Collections.Generic;
using UnityEngine;

namespace Placement
{
    public class GridSystem : MonoBehaviour
    {
        public static GridSystem Instance { get; private set; } // singleton

        [SerializeField] private LayerMask obstacleLayer;         //  this layer is used to check overlapping objects when placing

        // stores placed objects in this dictionary at integer coords (integers make it a grid)
        private Dictionary<Vector3Int, PlaceableObject> grid = new Dictionary<Vector3Int, PlaceableObject>();

        private void Awake() => Instance = this;    // set up singleton

        // round world transform (float) to the grid (int).
        public Vector3Int WorldToGridPos(Vector3 worldPos)
        {
            return new Vector3Int(
                Mathf.RoundToInt(worldPos.x),
                Mathf.RoundToInt(worldPos.y),
                Mathf.RoundToInt(worldPos.z)
                );
        }

        // place a buildable prefab from world position onto the grid
        // then get itself and its neighbours to check connections
        public bool PlacePiece(Vector3 worldPos, PlaceableObject placeablePrefab, Quaternion rotation)
        {
            Vector3Int gridPos = WorldToGridPos(worldPos);  // world position to grid position

            if (grid.ContainsKey(gridPos)) return false; // grid position already occupied

            PlaceableObject newPlaceable = Instantiate(placeablePrefab, gridPos, rotation);
            newPlaceable.GridPosition = gridPos;    // set its internal grid pos
            grid.Add(gridPos, newPlaceable);    // updates the grid with the position of the new placeable

            SetLayerRecursively(newPlaceable.gameObject, 6);    //  set layer for everything on a gameObject

            // check self and neighbours for connections
            NotifySelfAndNeighbours(gridPos);
            return true;
        }

        // opposite of placing a piece
        // tries to get the placeable at grid position from the given world position
        public void RemovePiece(Vector3Int worldPos)
        {
            Vector3Int gridPos = WorldToGridPos(worldPos);
            if (!grid.TryGetValue(gridPos, out PlaceableObject placeableObj)) return;

            grid.Remove(gridPos);
            Destroy(placeableObj.gameObject);

            // make neighbours check connections
            NotifyNeighboursOnly(gridPos);
        }

        //  gets the PlaceableObject at a given grid position
        public PlaceableObject GetPieceAt(Vector3Int gridPos)
        {
            grid.TryGetValue(gridPos, out PlaceableObject placeableObj);
            return placeableObj;
        }

        //  check if connections need updating for the placeable object and its neighbouring objects
        private void NotifySelfAndNeighbours(Vector3Int gridPos)
        {
            if (grid.TryGetValue(gridPos, out PlaceableObject placeableObj))
                placeableObj.UpdateConnections();

            NotifyNeighboursOnly(gridPos);
        }

        // gets neighbours of placeable object by vector directions
        // then updates their connections
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
                if (grid.TryGetValue(gridPos + dir, out PlaceableObject neighbour))
                {
                    neighbour.UpdateConnections();
                }
            }
        }

        private void SetLayerRecursively(GameObject currentObject, int layer)   // sets gameObject's and all it's children's layers
        {
            currentObject.layer = layer;
            foreach (Transform child in currentObject.transform)
                SetLayerRecursively(child.gameObject, layer);
        }
    }
}