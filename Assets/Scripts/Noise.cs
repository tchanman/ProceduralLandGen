using UnityEngine;
using System.Collections;

public static class Noise 
{

    public static float[,] GenerateNoiseMap(int size, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset) {
        float[,] noiseMap = new float[size, size];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i=0; i<octaves; i++) {
            float offsetX = prng.Next(-100000,100000) + offset.x;
            float offsetY = prng.Next(-100000,100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if(scale <= 0) {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfSize = size / 2;

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i=0; i<octaves; i++){
                    float sampleX = (x-halfSize) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y-halfSize) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                
                if(noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if(noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x,y] = noiseHeight;
            }
        }

        for(int y=0; y<size; y++) {
            for(int x=0; x<size; x++) {
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
            }
        }

        return noiseMap;
    }
}