using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public enum DrawMode {NoiseMap, ColorMap, Mesh};
    public DrawMode drawMode;
    
    public const int chunkSize = 241;
     [Range(0, 6)]
    public int levelOfDetail;
    public float noiseScale;

    public int octaves;
     [Range(0,1)]
    public float persistance;
    public float lacunarity;

    
    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;
    
    public TerrainType[] regions;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, noiseScale, seed, octaves, persistance, lacunarity, offset);
        
        Color[] colorMap = new Color[chunkSize * chunkSize];
        
        for(int y=0; y<chunkSize; y++) {
            for(int x=0; x<chunkSize; x++) {
                float currentHeight = noiseMap[x,y];
                for(int i=0; i<regions.Length; i++) {
                    if(currentHeight <= regions[i].height) {
                        colorMap[x + y*chunkSize] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(drawMode == DrawMode.NoiseMap) {
            display.DrawTexture(TextureGenerator.TextureFromNoiseMap(noiseMap));
        } else if(drawMode == DrawMode.ColorMap) {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, chunkSize));
        } else if(drawMode == DrawMode.Mesh) {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, chunkSize));
        }

    }

    void OnValidate() {
        if(octaves < 1) {
            octaves = 1;
        } else if(octaves > 32) {
            octaves = 32;
        }
        if(lacunarity < 1) {
            lacunarity = 1;
        }
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}