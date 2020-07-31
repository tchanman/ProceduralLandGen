﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {
    public const float maxViewDst = 450;
    public Transform viewer;

    public static Vector2 viewerPos;

    int chunkSize;
    int chunksVisible;

    Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    void Start() {
        chunkSize = MapGenerator.chunkSize-1;
        chunksVisible = Mathf.RoundToInt(maxViewDst / chunkSize);


    }

    void Update() {
        viewerPos = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks() {

        for(int i=0; i<terrainChunksVisibleLastUpdate.Count; i++) {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkX = Mathf.RoundToInt(viewerPos.x / chunkSize);
        int currentChunkY = Mathf.RoundToInt(viewerPos.y / chunkSize);

        for(int yOffset = -chunksVisible; yOffset <= chunksVisible; yOffset++) {
            for(int xOffset = -chunksVisible; xOffset <= chunksVisible; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(currentChunkX + xOffset, currentChunkY + yOffset);
                
                if(terrainChunkDict.ContainsKey(viewedChunkCoord)) {
                    terrainChunkDict[viewedChunkCoord].UpdateTerrainChunk();
                    if(terrainChunkDict[viewedChunkCoord].IsVisible()) {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDict[viewedChunkCoord]);
                    }
                } else {
                    terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
                }
            }
        }
    }

    public class TerrainChunk {

        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        public TerrainChunk(Vector2 coord, int size, Transform parent) {
            position = coord * size;
            Vector3 position3 = new Vector3(position.x, 0, position.y);
            bounds = new Bounds(position, Vector2.one * size);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = position3;
            meshObject.transform.localScale = Vector3.one * size /10f;
            meshObject.transform.parent = parent;
            SetVisible(false);
        }

        public void UpdateTerrainChunk() {
            float viewerDstFromNearestEdge = bounds.SqrDistance(viewerPos);
            float maxViewDstSqr = maxViewDst * maxViewDst;
            bool visible = viewerDstFromNearestEdge <= maxViewDstSqr;
            SetVisible(visible);
        }

        public void SetVisible(bool visible) {
            meshObject.SetActive(visible);
        }

        public bool IsVisible() {
            return meshObject.activeSelf;
        }
    }
}