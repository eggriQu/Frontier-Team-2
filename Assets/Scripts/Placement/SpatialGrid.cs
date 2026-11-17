using System.Collections.Generic;
using UnityEngine;


namespace Placement
{
    public class SpatialGrid
    {
        private Dictionary<Vector3Int, GameObject> _grid = new Dictionary<Vector3Int, GameObject>();

        /// <summary>
        /// Converts world space position to nearest grid cell.
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

        public GameObject GetObjectAt(Vector3 worldPos)
        {
            Vector3Int gridPos = WorldToGridPos(worldPos);
            if (_grid.ContainsKey(gridPos))
            {
                return _grid.TryGetValue(gridPos, out GameObject obj) ? obj : null;
            }

            return null;
        }

        public bool IsPlacementValid(Vector3 worldPos, GameObject objToPlace, Quaternion rotation, out Vector3Int gridPos)
        {
            gridPos = WorldToGridPos(worldPos);

            if (_grid.ContainsKey(gridPos))
            {
                return false;
            }

            return true;
        }

        public bool IsRemovalValid(Vector3 worldPos, out Vector3Int gridPos, out GameObject obj)
        {
            gridPos = WorldToGridPos(worldPos);
            if (!_grid.ContainsKey(gridPos))
            {
                obj = null;
                return false;
            }
            obj = _grid[gridPos];
            return true;
        }

        public void AddObject(Vector3Int gridPos, GameObject objToPlace)
        {
            _grid.Add(gridPos, objToPlace);
        }

        public void RemoveObject(Vector3Int gridPos)
        {
            _grid.Remove(gridPos);
        }
    }
}