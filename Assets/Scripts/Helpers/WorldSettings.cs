using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public struct WorldSettings {
    // chunk and voxel data
    public static int3 chunkDimension = new int3(16, 16, 16); // how many vertices are in a chunk
    public static float voxelSize = 1f; // distance between vertices
    public static int WorldHeightInChunks = 5;
    public static float WorldHeight {
        get => WorldHeightInChunks * chunkDimension.y * voxelSize;
    }

    // noise generation
    public static int octaves = 5;
    public static float frequency = 0.01f;
    public static float lacunarity = 2f;
    public static float persistence = 0.5f;

    // mesh generation
    public static float isoLevel = 0.5f;

    // player settings
    public static int RenderDistanceInChunks = 5;
    public static float3 RenderDistance {
        get => RenderDistanceInChunks * (float3)chunkDimension * voxelSize;
    }
}
