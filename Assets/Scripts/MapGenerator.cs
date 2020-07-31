using System;
using System.Threading;
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

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();


    public void DrawMapInEditor() {
        MapData mapData = GenerateMapData();

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(drawMode == DrawMode.NoiseMap) {
            display.DrawTexture(TextureGenerator.TextureFromNoiseMap(mapData.noiseMap));
        } else if(drawMode == DrawMode.ColorMap) {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, chunkSize));
        } else if(drawMode == DrawMode.Mesh) {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(mapData.colorMap, chunkSize));
        }
    }

    public void RequestMapData(Action<MapData> callback) {
        ThreadStart threadStart = delegate {
            MapDataThread(callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Action<MapData> callback) {
        MapData mapData = GenerateMapData();
        lock(mapDataThreadInfoQueue) {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, Action<MeshData> callback) {
        ThreadStart threadStart = delegate {
            MeshDataThread(mapData, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, Action<MeshData> callback) {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);
        lock(meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    void Update() {
        while(mapDataThreadInfoQueue.Count > 0) {
            lock(mapDataThreadInfoQueue) {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        while(meshDataThreadInfoQueue.Count > 0) {
            lock(meshDataThreadInfoQueue) {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData() {
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

        return new MapData(noiseMap, colorMap);
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

    struct MapThreadInfo<T> {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter) {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}

public struct MapData {
    public readonly float[,] noiseMap;
    public readonly Color[] colorMap;

    public MapData(float[,] noiseMap, Color[] colorMap) {
        this.noiseMap = noiseMap;
        this.colorMap = colorMap;
    }

}