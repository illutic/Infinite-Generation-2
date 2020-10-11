using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    float[,] noiseMap;
    Meshes[] meshes;
    Attributes ChunkAttributes;
    int previousLoD;

    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
    MeshFilter meshFilter;

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
        ThreadManager.Instance.RequestMesh(OnGenerateRequestReceived, this);
        EventManager.ChunkGenerationEvent -= Generate;
    }

    public void OnGenerateRequestReceived(Chunk chunk)
    {
        lock (meshes)
        {
            for (int i = 0; i < chunk.meshes.Length; i++)
            {
                meshes[i].mesh = new Mesh();
                meshes[i].mesh.vertices = meshes[i].meshInfo.vertices;
                meshes[i].mesh.uv = meshes[i].meshInfo.uvs;
                meshes[i].mesh.triangles = meshes[i].meshInfo.triangles;
                meshes[i].mesh.RecalculateNormals();

            }
            //meshFilter.sharedMesh = meshes[0].mesh;
        }

    }
    public void UpdateTerrainChunk()
    {
        Bounds bounds = new Bounds(transform.position, Vector2.one * ChunkAttributes.size);
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(GameManager.Instance.Player.transform.position));
        bool visible = viewerDstFromNearestEdge <= ChunkAttributes.viewDst;
        setVisible(visible);
        LODSwitch();
    }
    private void LODSwitch()
    {

        if (isVisible())
        {
            int currentLoD = -1;
            Bounds bounds = new Bounds(transform.position, Vector2.one * ChunkAttributes.size);
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(GameManager.Instance.Player.transform.position));

            if(currentLoD != previousLoD)
            {
                for (int i = 0; i < ChunkAttributes.levelsOfDetail.Length; i++)
                {
                    currentLoD++;
                    if(viewerDstFromNearestEdge < ChunkAttributes.levelsOfDetail[i].viewThreshold)
                    {
                        meshFilter.sharedMesh = meshes[i].mesh;
                        meshCollider.sharedMesh = meshes[i].mesh;
                        previousLoD = currentLoD;
                        break;
                    }
                }
            }
        }

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

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        ChunkAttributes = GameManager.Instance.GlobalAttributes;
        ChunkAttributes.offset = new Vector2(transform.position.x, transform.position.z);
        noiseMap = Noise.GenerateNoiseMap(ChunkAttributes, Noise.NormalizeMode.Global);
        meshes = new Meshes[ChunkAttributes.levelsOfDetail.Length];

        EventManager.ChunkGenerationEvent += Generate;
    }
}

