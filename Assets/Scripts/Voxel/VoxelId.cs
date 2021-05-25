using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public struct VoxelId {
    public int3 id;

    public VoxelId(int3 id) {
        this.id = id;
    }

    public List<ChunkId> OverlappingChunks(ChunkId originalChunk) {
        List<ChunkId> chunks = new List<ChunkId>();
        
        if (!IsBoundary())
            return chunks;

        for (int b = 1; b <= 0b111; b++) {
            int changed = 1;
            ChunkId newChunk = new ChunkId(originalChunk.id);
            for (int i = 0; i < 3; i++) {
                if (id[i] == 0)
                    newChunk[i] -= 1;
                else if (id[i] == WorldSettings.chunkDimension[i] - 1)
                    newChunk[i] += 1;
                else
                    changed &= ~(1 << i);
            }
            if (changed == b)
                chunks.Add(newChunk);
        }

        return chunks;
    }

    public VoxelId Translate(ChunkId originalChunk, ChunkId newChunk) {
        int3 offset = originalChunk.id - newChunk.id;
        return new VoxelId(id - offset * (WorldSettings.chunkDimension - new int3(1, 1, 1)));
    }

    public float3 ToWorldCoord(ChunkId chunk) {
        return (float3)id * WorldSettings.voxelSize + chunk.ToWorldCoord();
    }

    public bool IsBoundary() {
        for (int i = 0; i < 3; i++) {
            if (id[i] == 0 || id[i] == WorldSettings.chunkDimension[i] - 1)
                return true;
        }
        return false;
    }

    public override bool Equals(object obj) {
        VoxelId other = (VoxelId)obj;
        return other.id.Equals(other.id);
    }

    public override int GetHashCode() {
        return id.GetHashCode();
    }
}
