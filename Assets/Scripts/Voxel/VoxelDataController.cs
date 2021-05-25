using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class VoxelDataController {
    public static Dictionary<ChunkId, VoxelData[]> voxelData = new Dictionary<ChunkId, VoxelData[]>();

    public static void GenerateDataForChunks(List<Chunk> chunksToProcess) {
        List<JobData<VoxelDataGeneratorJob>> jobDataList = new List<JobData<VoxelDataGeneratorJob>>();
        foreach (Chunk chunk in chunksToProcess) {
            VoxelDataGeneratorJob job = new VoxelDataGeneratorJob() {
                chunk = chunk.id,
                voxelData = new NativeArray<VoxelData>(
                    WorldSettings.chunkDimension.x * WorldSettings.chunkDimension.y * WorldSettings.chunkDimension.z, Allocator.TempJob)
            };
            jobDataList.Add(new JobData<VoxelDataGeneratorJob>(job, job.Schedule()));
        }
        for (int i = 0; i < chunksToProcess.Count; i++) {
            Chunk chunk = chunksToProcess[i];
            JobData<VoxelDataGeneratorJob> jobData = jobDataList[i];

            jobData.handle.Complete();
            if (voxelData.ContainsKey(chunk.id))
                voxelData[chunk.id] = jobData.job.voxelData.ToArray();
            else
                voxelData.Add(chunk.id, jobData.job.voxelData.ToArray());
            jobData.job.Dispose();

            chunk.workState.Next();
        }
    }

    public static void GenerateMeshForChunks(List<Chunk> chunksToProcess) {
        List<JobData<MarchingCubeJob>> jobDataList = new List<JobData<MarchingCubeJob>>();
        foreach (Chunk chunk in chunksToProcess) {
            MarchingCubeJob job = new MarchingCubeJob() {
                counter = new NativeCounter(Allocator.TempJob),
                voxelData = new NativeArray<VoxelData>(voxelData[chunk.id], Allocator.TempJob),
                vertexData = new NativeArray<VertexData>(voxelData[chunk.id].Length * 15, Allocator.TempJob),
                triangles = new NativeArray<ushort>(voxelData[chunk.id].Length * 15, Allocator.TempJob)
            };
            jobDataList.Add(new JobData<MarchingCubeJob>(job, job.Schedule()));
        }
        for (int i = 0; i < chunksToProcess.Count; i++) {
            Chunk chunk = chunksToProcess[i];
            JobData<MarchingCubeJob> jobData = jobDataList[i];

            jobData.handle.Complete();
            chunk.SetMeshData(jobData.job.counter.Count * 3, jobData.job.vertexData, jobData.job.triangles);
            jobData.job.Dispose();

            chunk.workState.Next();
            chunk.workState.Next();
        }
    }
}
