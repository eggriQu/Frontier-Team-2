using System;
using UnityEngine;

namespace Placement
{
    public class PlaceableObject : MonoBehaviour
    {
        public Vector3Int GridPosition { get; set; }
        public string PlaceableCategory = "";   // categories decide what connects to what

        [Header("Modular visual variants (Indices 0 - 15)")]
        [SerializeField] private GameObject[] bitmaskMeshVariants = new GameObject[16]; // limited to 16 indices with current bitmask setup

        private void OnValidate()
        {
            //  resizes array to 16 when it's changed in the inspector
            if (bitmaskMeshVariants == null || bitmaskMeshVariants.Length != 16)
            {
                Array.Resize(ref bitmaskMeshVariants, 16);
            }
        }

        //  set bitmask from the 4 directional checks, then update visual based on that bitmask result
        public void UpdateConnections()
        {
            int bitmask = 0;

            // check the 4 cardinal directions
            // add each neighbour's bitmask value to the bitmask
            if (HasMatchingNeighbour(GridPosition + Vector3Int.forward)) bitmask += 1;   // north(+z)    += 1
            if (HasMatchingNeighbour(GridPosition + Vector3Int.right)) bitmask += 2;   // east(+x)     += 2
            if (HasMatchingNeighbour(GridPosition + Vector3Int.back)) bitmask += 4;   // south(-z)    += 4
            if (HasMatchingNeighbour(GridPosition + Vector3Int.left)) bitmask += 8;   // west(-x)     += 8

            ApplyVisualVariant(bitmask);
        }

        //  takes neighbour position, gets the object and checks if its category matches
        private bool HasMatchingNeighbour(Vector3Int neighbourPos)
        {
            PlaceableObject neighbour = GridSystem.Instance.GetPieceAt(neighbourPos);
            return neighbour != null && neighbour.PlaceableCategory == this.PlaceableCategory;
        }

        //  uses bitmask to select the visual variant
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
