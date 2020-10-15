using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    float[,] noiseMap;
    Meshes[] meshes;
    public Attributes ChunkAttributes;
    List<Vector3> chunkVertices;

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
        ChunkAttributes = new Attributes(GameManager.Instance.GlobalAttributes);
        ChunkAttributes.offset = new Vector2(transform.position.x, transform.position.z);
        noiseMap = Noise.GenerateNoiseMap(ChunkAttributes, Noise.NormalizeMode.Global);
        meshes = new Meshes[ChunkAttributes.levelsOfDetail.Length];
        ThreadManager.Instance.RequestChunk(OnGenerateRequestReceived, this);

        GameObject water = Instantiate(ChunkAttributes.biomes[1].objects[0], this.transform);
        water.transform.localPosition = new Vector3(ChunkAttributes.size / 2, 6, ChunkAttributes.size / 2);
        water.transform.localScale = Vector3.one*(ChunkAttributes.size);

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
        meshFilter.mesh = meshes[meshes.Length-1].mesh;
        chunkVertices = new List<Vector3>(meshFilter.mesh.vertices);
        ThreadManager.Instance.RequestBiomeVertices(OnBiomeVerticesReceived,this);
    }

    public void GenerateBiomeVertices()
    {
        lock (chunkVertices)
        {
                UtilitiesFunctions.quickSort(chunkVertices, 0, chunkVertices.Count - 1);
                List<Vector3> tempList = new List<Vector3>();
                for (int biomeIndex = 2; biomeIndex < ChunkAttributes.biomes.Length; biomeIndex++)
                {
                    
                    while (chunkVertices.Count > 0 && chunkVertices[0].y <= ChunkAttributes.biomes[biomeIndex].elevation * ChunkAttributes.scale)
                    {
                        tempList.Add(chunkVertices[0]);
                        chunkVertices.RemoveAt(0);
                    }
                    ChunkAttributes.biomes[biomeIndex].biomeVertices = new Vector3[tempList.Count];
                    tempList.ToArray().CopyTo(ChunkAttributes.biomes[biomeIndex].biomeVertices, 0);
                    tempList.Clear();
                }
            
        }
    }
    public void OnBiomeVerticesReceived(Chunk chunk)
    {
        System.Random random = new System.Random();
        
        for (int i = 2; i < ChunkAttributes.biomes.Length; i++)
        {
            var biome = ChunkAttributes.biomes[i];
            foreach (var item in biome.objects)
            {
                int maxRecur = 10;
                if (biome.biomeVertices.Length != 0)
                {
                    
                    Vector3 randPos = biome.biomeVertices[random.Next(0, biome.biomeVertices.Length)];
                    while(GameManager.Instance.Objects.ContainsKey(randPos) && maxRecur != 0)
                    {
                        randPos = biome.biomeVertices[random.Next(0, biome.biomeVertices.Length)];
                        maxRecur--;
                    }
                    if (!GameManager.Instance.Objects.ContainsKey(randPos))
                    {
                        GameObject go = Instantiate(item, this.gameObject.transform);
                        go.transform.localPosition = randPos;
                        go.transform.localScale = Vector3.one * new System.Random().Next(5, 6);

                        
                    }
                    
                }
            }
        }
    }

    public void UpdateTerrainChunk()
    {
        Bounds bounds = new Bounds(transform.position, Vector2.one * ChunkAttributes.size);
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(GameManager.Instance.Player.transform.position));
        bool visible = viewerDstFromNearestEdge <= ChunkAttributes.viewDst;
        setVisible(visible);
        if (visible)
        {
            LODSwitch(viewerDstFromNearestEdge);
        }
    }
    private void LODSwitch(float viewerDstFromNearestEdge)
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
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.enabled = true;

        EventManager.ChunkGenerationEvent += Generate;
    }
    public void setVisible(bool visible)
    {
        this.gameObject.SetActive(visible);
    }
    public bool isVisible()
    {
        return this.gameObject.activeSelf;
    }
}

