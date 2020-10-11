﻿using System.Collections;
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
    public Vector2 offset;
    public float elevation;
    public AnimationCurve animationCurve;
    public int viewDst = 600;
    public LevelOfDetail[] levelsOfDetail;
    protected int levelOfDetail;
    public Attributes getGlobalAttributes()
    {
        return new Attributes(size, seed, scale, octaves, persistance, lacunarity, offset, elevation, animationCurve, viewDst, levelsOfDetail, levelOfDetail);
    }

    private void Awake()
    {
        GameManager.Instance.GlobalAttributes = getGlobalAttributes();
        GameManager.Instance.viewedChunks = Mathf.RoundToInt(viewDst / size);
    }
}
