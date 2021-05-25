using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ChunkController : MonoBehaviour {
    public static Dictionary<ChunkId, Chunk> chunks;
    public GameObject chunkPrefab;

    public Transform player;

    public WorkState workState;

    private void Awake() {
        chunks = new Dictionary<ChunkId, Chunk>();
        workState = new WorkState();
    }

    private void Update() {
        UnloadChunk();
        LoadExistingChunks();
        InitNewChunks();

        List<Chunk> chunksToProcess = new List<Chunk>();
        foreach (Chunk chunk in chunks.Values) {
            if (chunk.workState.workState == workState.workState) {
                chunksToProcess.Add(chunk);
            }
        }
        ProcessChunks(chunksToProcess);
        workState.NextInLoop();
    }

    private void ProcessChunks(List<Chunk> chunksToProcess) {
        switch (workState.workState) {
            case WorkState.FILL:
                VoxelDataController.GenerateDataForChunks(chunksToProcess);
                break;
            case WorkState.MESH:
                VoxelDataController.GenerateMeshForChunks(chunksToProcess);
                break;
        }
    }

    private void LoadExistingChunks() {
        ChunkId playerChunkCoord = GetPlayerChunkCoord();
        foreach (ChunkId chunk in chunks.Keys) {
            if (Utils.Magnitude(chunk.id - playerChunkCoord.id) <= WorldSettings.RenderDistanceInChunks) {
                chunks[chunk].Load();
            }
        }
    }

    private void InitNewChunks() {
        ChunkId playerChunkCoord = GetPlayerChunkCoord();
        for (int x = 0; x < WorldSettings.RenderDistanceInChunks; x++) {
            for (int y = 0; y < WorldSettings.RenderDistanceInChunks; y++) {
                for (int z = 0; z < WorldSettings.RenderDistanceInChunks; z++) {
                    ChunkId newChunk = new ChunkId(new int3(x, y, z) + playerChunkCoord.id);
                    if (!chunks.ContainsKey(newChunk) && Utils.Magnitude(newChunk.id - playerChunkCoord.id) <= WorldSettings.RenderDistanceInChunks) {
                        GameObject chunkGameObject = Instantiate(chunkPrefab, newChunk.ToWorldCoord(), Quaternion.identity);
                        Chunk chunk = new Chunk(newChunk, chunkGameObject);
                        chunks.Add(newChunk, chunk);
                    }
                }
            }
        }
    }

    private void UnloadChunk() {
        ChunkId playerChunkCoord = GetPlayerChunkCoord();
        foreach (ChunkId chunk in chunks.Keys) {
            if (Utils.Magnitude(chunk.id - playerChunkCoord.id) > WorldSettings.RenderDistanceInChunks) {
                chunks[chunk].Unload();
            }
        }
    }

    private ChunkId GetPlayerChunkCoord() {
        return new ChunkId((int3)math.floor((float3)player.position / ((float3)WorldSettings.chunkDimension * WorldSettings.voxelSize)));
    }
}
