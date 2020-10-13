using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    float[,] noiseMap;
    Meshes[] meshes;
    public Attributes ChunkAttributes;

    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
    MeshFilter meshFilter;

    List<Vector3> chunkVertices;

    public void PreGenerate()
    {
        lock (meshes)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                
                meshes[i].LevelOfDetail = ChunkAttributes.levelsOfDetail[i].LoD;
                meshes[i].meshInfo = new MeshInfo(MeshGen.vertices(ChunkAttributes.size,
                                                                   noiseMap,
                                                                   meshes[i].LevelOfDetail,
                                                                   Mathf.RoundToInt(ChunkAttributes.scale),
                                                                   ChunkAttributes.animationCurve),
                                                  MeshGen.quadsTriangles(ChunkAttributes.size, meshes[i].LevelOfDetail),
                                                  MeshGen.uvs(ChunkAttributes.size, meshes[i].LevelOfDetail));
            }
        }
    }
    public void Generate()
    {
        ChunkAttributes = new Attributes(GameManager.Instance.GlobalAttributes);
        ChunkAttributes.offset = new Vector2(transform.position.x, transform.position.z);
        noiseMap = Noise.GenerateNoiseMap(ChunkAttributes, Noise.NormalizeMode.Global);
        meshes = new Meshes[ChunkAttributes.levelsOfDetail.Length];
        ThreadManager.Instance.RequestMesh(OnGenerateRequestReceived, this);
        EventManager.ChunkGenerationEvent -= Generate;
    }

    public void OnGenerateRequestReceived(Chunk chunk)
    {
        for (int i = 0; i < chunk.meshes.Length; i++)
        {
            meshes[i].mesh = new Mesh();
            meshes[i].mesh.vertices = meshes[i].meshInfo.vertices;
            meshes[i].mesh.uv = meshes[i].meshInfo.uvs;
            meshes[i].mesh.triangles = meshes[i].meshInfo.triangles;
            meshes[i].mesh.RecalculateNormals();
        }
        
    }
    public void GenerateBiomeVertices()
    {
        chunkVertices = new List<Vector3>(meshes[0].meshInfo.vertices);
        Utilities.quickSort(chunkVertices, 0, chunkVertices.Count - 1);
        for (int biomeIndex = 0; biomeIndex < ChunkAttributes.biomes.Length; biomeIndex++)
        {
            List<Vector3> tempList = new List<Vector3>();
            while (chunkVertices.Count > 0 && chunkVertices[0].y <= ChunkAttributes.biomes[biomeIndex].elevation)
            {
                tempList.Add(chunkVertices[0]);
                chunkVertices.RemoveAt(0);
            }
            ChunkAttributes.biomes[biomeIndex].biomeVertices = tempList.ToArray();
        }
    }
    public void UpdateTerrainChunk()
    {
        
        Bounds bounds = new Bounds(transform.position, Vector2.one * ChunkAttributes.size);
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(GameManager.Instance.Player.transform.position));
        bool visible = viewerDstFromNearestEdge <= ChunkAttributes.viewDst;
        setVisible(visible);
        LODSwitch(viewerDstFromNearestEdge);
    }
    private void LODSwitch(float viewerDstFromNearestEdge)
    {

        if (isVisible())
        {
            int currentLoD = -1;
            if(currentLoD != ChunkAttributes.LevelOfDetail)
            {
                for (int i = 0; i < ChunkAttributes.levelsOfDetail.Length; i++)
                {
                    currentLoD++;
                    if(viewerDstFromNearestEdge < ChunkAttributes.levelsOfDetail[i].viewThreshold)
                    {
                        meshFilter.sharedMesh = meshes[i].mesh;
                        meshCollider.sharedMesh = meshes[i].mesh;
                        ChunkAttributes.LevelOfDetail = currentLoD;
                        break;
                    }
                }
            }
        }

    }
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        EventManager.ChunkGenerationEvent += Generate;
    }

    public void setVisible(bool visible)
    {
        meshRenderer.enabled = visible;
        meshCollider.enabled = visible;
    }
    public bool isVisible()
    {
        return meshRenderer.enabled;
    }

}

