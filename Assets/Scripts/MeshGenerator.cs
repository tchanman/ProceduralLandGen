using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {
    public static MeshData GenerateTerrainMesh(float[,] noiseMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail) {
        int size = noiseMap.GetLength(0);
        float topLeftX = (size-1) /-2f;
        float topLeftZ = (size-1) / 2f;

        int meshSimplificationInc = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (size-1)/meshSimplificationInc + 1;

        MeshData meshData = new MeshData(size);
        int vertexIndex = 0;

        for(int y=0; y<size; y += meshSimplificationInc) {
            for(int x=0; x<size; x += meshSimplificationInc) {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(noiseMap[x,y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)size, y / (float)size);

                if(x < size-1 && y < size-1) {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }
                
                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    
    int triangleIndex;

    public MeshData(int size) {
        vertices = new Vector3[size * size];
        triangles = new int[(size-1)*(size-1)*6];
        uvs = new Vector2[size * size];
    }

    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex+1] = b;
        triangles[triangleIndex+2] = c;

        triangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        return mesh;
    }
}