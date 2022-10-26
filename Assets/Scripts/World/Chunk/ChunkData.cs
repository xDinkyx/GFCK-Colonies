﻿using UnityEngine;

namespace World
{
    public partial class ChunkData
    {
        public int x;
        public int z;
        public Vector3 origin;

        public int MaxX;
        public int MaxY;
        public int MaxZ;
        public BlockData[,,] blocks; // Data

        public ChunkMeshData meshData;
        public bool meshChanged;
    }
}
