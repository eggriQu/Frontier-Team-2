using System;
using UnityEngine;

namespace Placement
{
    /// <summary>
    /// stores a placed object's grid location and selects its visual
    /// variant based on matching neighbouring objects.
    /// </summary>
    public class PlaceableObject : MonoBehaviour
    {
        public Vector3Int GridPosition { get; set; }    // the grid cell occupied by this object
        public PlaceableCategory placeableCategory = PlaceableCategory.None;   // objects only connect to neighbours with same category

        [Header("Modular visual variants (Indices 0 - 15)")]
        // each index represents one combination of the four neighbours
        [SerializeField] private GameObject[] bitmaskMeshVariants = new GameObject[16];

        private int _activeBitmask = -1;

        private void Awake()
        {
            if (bitmaskMeshVariants == null)
            {
                return;
            }
            
            for (int i = 0; i < bitmaskMeshVariants.Length; i++)
            {
                if (bitmaskMeshVariants[i] != null)
                {
                    bitmaskMeshVariants[i].SetActive(false);
                }
            }
        }

        private void OnValidate()
        {
            //  keeps array 16 elements long as it needs one slot for each possible combination
            if (bitmaskMeshVariants == null || bitmaskMeshVariants.Length != 16)
            {
                Array.Resize(ref bitmaskMeshVariants, 16);
            }
        }

        public void UpdateConnections(int bitmask)
        {
            ApplyVisualVariant(bitmask);
        }
        
        /// <summary>
        /// activates the corresponding variant child for the
        /// supplied bitmask and deactivates other variants
        /// </summary>
        /// <param name="bitmask"></param>
        private void ApplyVisualVariant(int bitmask)
        {
            if (bitmask == _activeBitmask || bitmaskMeshVariants == null || bitmaskMeshVariants.Length != 16)
            {
                return;
            }

            if (_activeBitmask >= 0 && _activeBitmask < 16 && bitmaskMeshVariants[_activeBitmask] != null)
            {
                bitmaskMeshVariants[_activeBitmask].SetActive(false);
            }

            if (bitmask >= 0 && bitmask < 16 && bitmaskMeshVariants[bitmask] != null)
            {
                bitmaskMeshVariants[bitmask].SetActive(true);
            }

            _activeBitmask = bitmask;
        }
    }

    public enum PlaceableCategory
    {
        None,
        Wall,
        Trench,
        House
    }
}
