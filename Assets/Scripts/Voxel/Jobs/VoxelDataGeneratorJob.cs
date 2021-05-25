using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct VoxelDataGeneratorJob : IJob {
    public ChunkId chunk;

    public NativeArray<VoxelData> voxelData;

    public void Execute() {
        for (int x = 0; x < WorldSettings.chunkDimension.x; x++) {
            for (int z = 0; z < WorldSettings.chunkDimension.z; z++) {
                float3 chunkCoord = chunk.ToWorldCoord();
                float2 terrainCoord = new float2(x * WorldSettings.voxelSize + chunkCoord.x, z * WorldSettings.voxelSize + chunkCoord.z);

                float terrainHeight = NoiseGenerator.Generate(terrainCoord) * WorldSettings.WorldHeight;

                for (int y = 0; y < WorldSettings.chunkDimension.y; y++) {
                    float height = y * WorldSettings.voxelSize + chunkCoord.y;

                    float density = math.clamp((terrainHeight - height) / WorldSettings.WorldHeight, 0, 1);

                    voxelData[Utils.CoordToIndex(new int3(x, y, z))] = new VoxelData(density, Color.white);
                }
            }
        }
    }

    public void Dispose() {
        this.voxelData.Dispose();
    }
}
