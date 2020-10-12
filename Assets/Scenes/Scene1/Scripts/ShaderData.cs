using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderData 
{
    public static float minHeight
    {
        get
        {
            return Mathf.Pow(GameManager.Instance.GlobalAttributes.animationCurve.Evaluate(0),2) * GameManager.Instance.GlobalAttributes.scale;
        }
    }
    public static float maxHeight
    {
        get
        {
            return Mathf.Pow(GameManager.Instance.GlobalAttributes.animationCurve.Evaluate(1), 2) * GameManager.Instance.GlobalAttributes.scale;
        }
    }
    public static float[] startHeights 
    { 
        get 
        {
            float[] tempArray = new float[GameManager.Instance.GlobalAttributes.biomes.Length];
            for (int i = 0; i < GameManager.Instance.GlobalAttributes.biomes.Length; i++)
            {
                tempArray[i] = GameManager.Instance.GlobalAttributes.biomes[i].elevation;
            }
            return tempArray;
        } 
    }
    public static Color[] baseColours 
    { 
        get 
        {
            Color[] tempArray = new Color[GameManager.Instance.GlobalAttributes.biomes.Length];
            for (int i = 0; i < GameManager.Instance.GlobalAttributes.biomes.Length; i++)
            {
                tempArray[i] = GameManager.Instance.GlobalAttributes.biomes[i].color;
            }
            return tempArray;
        } 
    }
    public static int colourCount
    {
        get
        {
            return GameManager.Instance.GlobalAttributes.biomes.Length;
        }
    }
    public static float[] blends
    {
        get
        {
            float[] tempArray = new float[GameManager.Instance.GlobalAttributes.biomes.Length];
            for (int i = 0; i < GameManager.Instance.GlobalAttributes.biomes.Length; i++)
            {
                tempArray[i] = GameManager.Instance.GlobalAttributes.biomes[i].blend;
            }
            return tempArray;
        }
    }

    public static void UpdateShaderMeshHeights(Material mat)
    {
        mat.SetFloat("minHeight", minHeight);
        mat.SetFloat("maxHeight", maxHeight);
        mat.SetInt("colourCount", colourCount);
        mat.SetColorArray("colours", baseColours);
        mat.SetFloatArray("startHeights", startHeights);
        mat.SetFloatArray("blends", blends);
    }
}
