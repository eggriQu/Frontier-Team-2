using Placement;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
    public class VoxelSystem : MonoBehaviour
    {
        public static VoxelSystem Instance { get; private set; }

        [SerializeField] private GridSystem buildingGrid;

        private Dictionary<Vector3Int, GameObject> _voxels = new Dictionary<Vector3Int, GameObject>();
        private void Awake() => Instance = this;    // singleton instance setup

        private void Start()
        {
            
        }




    }
}