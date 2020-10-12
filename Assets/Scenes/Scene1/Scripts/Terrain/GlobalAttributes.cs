using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GlobalAttributes : MonoBehaviour
{
    public int size;
    public int seed;
    public float scale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public float elevation;
    public Vector2 offset;
    public AnimationCurve animationCurve;
    public int viewDst = 600;
    public LevelOfDetail[] levelsOfDetail;
    public Biome[] biomes;


    protected int levelOfDetail;
    
    public Attributes getGlobalAttributes()
    {
        seed = new System.Random().Next(-10000, 10000);
        return new Attributes(size, seed, scale, octaves, persistance, lacunarity, offset, elevation, animationCurve, viewDst, levelsOfDetail, levelOfDetail,biomes);
    }

    private void Awake()
    {
        GameManager.Instance.GlobalAttributes = getGlobalAttributes();
        GameManager.Instance.viewedChunks = Mathf.RoundToInt(viewDst / size);
    }
}
