using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public struct NoiseGenerator {
    public static float Generate(float2 pos) {
        float val = 0;
        for (int octave = 0; octave < WorldSettings.octaves; octave++) {
            val += noise.snoise(pos * WorldSettings.frequency * math.pow(WorldSettings.lacunarity, octave)) * math.pow(WorldSettings.persistence, octave);
        }
        return math.clamp((val + 1) / 2, 0, 1);
    }

    public static float Generate(float3 pos) {
        float val = 0;
        for (int octave = 0; octave < WorldSettings.octaves; octave++) {
            val += noise.snoise(pos * WorldSettings.frequency * math.pow(WorldSettings.lacunarity, octave)) * math.pow(WorldSettings.persistence, octave);
        }
        return math.clamp((val + 1) / 2, 0, 1);
    }
}
