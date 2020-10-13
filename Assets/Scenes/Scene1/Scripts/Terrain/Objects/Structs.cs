using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Attributes
{
    public int size;
    public int seed;
    public float scale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public Vector2 offset;
    public float elevation;
    public AnimationCurve animationCurve;
    public int viewDst;
    public LevelOfDetail[] levelsOfDetail;
    public int LevelOfDetail;
    public Biome[] biomes;
    public Attributes(int size, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, float elevation, AnimationCurve animationCurve, int viewDst, LevelOfDetail[] levelsOfDetail, int levelOfDetail, Biome[] biomes)
    {
        this.size = size;
        this.seed = seed;
        this.scale = scale;
        this.octaves = octaves;
        this.persistance = persistance;
        this.lacunarity = lacunarity;
        this.offset = offset;
        this.elevation = elevation;
        this.animationCurve = animationCurve;
        this.viewDst = viewDst;
        this.levelsOfDetail = levelsOfDetail;
        LevelOfDetail = levelOfDetail;
        this.biomes = new Biome[biomes.Length];
        biomes.CopyTo(this.biomes,0);
    }
    public Attributes(Attributes attributes)
    {
        this.size = attributes.size;
        this.seed = attributes.seed;
        this.scale = attributes.scale;
        this.octaves = attributes.octaves;
        this.persistance = attributes.persistance;
        this.lacunarity = attributes.lacunarity;
        this.offset = attributes.offset;
        this.elevation = attributes.elevation;
        this.animationCurve = attributes.animationCurve;
        this.viewDst = attributes.viewDst;
        this.levelsOfDetail = attributes.levelsOfDetail;
        LevelOfDetail = attributes.LevelOfDetail;
        this.biomes = new Biome[attributes.biomes.Length];
        attributes.biomes.CopyTo(this.biomes, 0);
    }
}


public struct Meshes
{
    public int LevelOfDetail;
    public MeshInfo meshInfo;
    public Mesh mesh;
}

public struct MeshInfo
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    public MeshInfo(Vector3[] vertices, int[] triangles, Vector2[] uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }
}

[System.Serializable]
public struct LevelOfDetail
{
    public int LoD;
    public int viewThreshold;
}

[System.Serializable]
public struct Biome
{
    public float elevation;
    public Color color;
    [Range(0,1)]
    public float blend;
    public GameObject[] objects;
    public Vector3[] biomeVertices;

}