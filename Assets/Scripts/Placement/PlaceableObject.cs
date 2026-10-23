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
        public string placeableCategory = "";   // objects only connect to neighbours with same category

        [Header("Modular visual variants (Indices 0 - 15)")]
        // each index represents one combination of the four neighbours
        [SerializeField] private GameObject[] bitmaskMeshVariants = new GameObject[16];

        private void OnValidate()
        {
            //  keeps array 16 elements long as it needs one slot for each possible combination
            if (bitmaskMeshVariants == null || bitmaskMeshVariants.Length != 16)
            {
                Array.Resize(ref bitmaskMeshVariants, 16);
            }
        }

        /// <summary>
        /// checks its four neighbouring cells and activates
        /// the visual variant from the connection bitmask
        /// </summary>
        public void UpdateConnections()
        {
            int bitmask = 0;

            // each direction has its own bit value, allowing neighbour
            // combinations to map a unique index from 0 to 15.
            if (HasMatchingNeighbour(GridPosition + Vector3Int.forward)) bitmask += 1;   // north(+z)    += 1
            if (HasMatchingNeighbour(GridPosition + Vector3Int.right)) bitmask += 2;   // east(+x)     += 2
            if (HasMatchingNeighbour(GridPosition + Vector3Int.back)) bitmask += 4;   // south(-z)    += 4
            if (HasMatchingNeighbour(GridPosition + Vector3Int.left)) bitmask += 8;   // west(-x)     += 8
            // subtracting the biggest value that makes up the bitmask gives one of the directions.
            // repeat this to work out all the directions of a bitmask value.

            ApplyVisualVariant(bitmask);
        }
        /// <summary>
        /// returns whether the specified grid cell has a placeable
        /// with the same category as this object
        /// </summary>
        /// <param name="neighbourPos"></param>
        /// <returns></returns>
        private bool HasMatchingNeighbour(Vector3Int neighbourPos)
        {
            PlaceableObject neighbour = GridSystem.Instance.GetPieceAt(neighbourPos);
            return neighbour != null && neighbour.placeableCategory == this.placeableCategory;
        }

        /// <summary>
        /// activates the corresponding variant child for the
        /// supplied bitmask and deactivates other variants
        /// </summary>
        /// <param name="bitmask"></param>
        private void ApplyVisualVariant(int bitmask)
        {
            if (bitmaskMeshVariants != null && bitmaskMeshVariants.Length == 16)
            {
                GameObject targetVariant = (bitmask >= 0 && bitmask < 16) ? bitmaskMeshVariants[bitmask] : null;

                for (int i = 0; i < 16; i++)
                {
                    GameObject variant = bitmaskMeshVariants[i];
                    if (variant != null && variant != targetVariant)
                    {
                        variant.SetActive(false);
                    }
                }

                if (targetVariant != null)
                {
                    targetVariant.SetActive(true);
                }
            }
        }
    }
}
