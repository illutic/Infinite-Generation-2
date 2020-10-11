using UnityEngine;

public static class MeshGen
{
    public static int[] quadsTriangles(int size, int levelOfDetail)
    {
        if (levelOfDetail <= 0)
        {
            levelOfDetail = 1;
        }

        //Level Of Detail
        int meshSimplificationIncrement = levelOfDetail * 2;
        int simplifiedVI = Mathf.RoundToInt((size-1) / meshSimplificationIncrement + 1);
        // At sizes greater than 502 LoD = 5 breaks the program (WHY?!)
        //calculating triangles
        int[] triangles = new int[(simplifiedVI-1) * (simplifiedVI-1) * 6];

        //try
        {
            for (int y = 0, ti = 0, vi = 0; y < size; y += meshSimplificationIncrement)
            {
                for (int x = 0; x < size; x += meshSimplificationIncrement, vi++) // ti = triangle Index, vi = Vertex Index
                {
                    if (x < size - meshSimplificationIncrement && y < size - meshSimplificationIncrement) // we dont have to draw triangles outside the boundaries
                    {
                        triangles[ti] = vi;
                        triangles[ti + 1] = vi + simplifiedVI + 1;
                        triangles[ti + 2] = vi + simplifiedVI;
                        triangles[ti + 3] = vi + simplifiedVI + 1;
                        triangles[ti + 4] = vi;
                        triangles[ti + 5] = vi + 1;
                        ti += 6;
                    }
                }
            }
        }
        //catch (System.Exception e){}

        return triangles;
    }

    public static Vector3[] vertices(int size, float[,] noise, int levelOfDetail, int scale, AnimationCurve heightEvaluator)
    {
        //Level Of Detail
        if (levelOfDetail <= 0)
        {
            levelOfDetail = 1;
        }
        int meshSimplificationIncrement = levelOfDetail * 2;
        int simplifiedVI = (size - 1) / meshSimplificationIncrement + 1;

        //Calculating Vertices, we need an index to tell us where we are
        //While we are at it we calculate the UVs of the mesh as well
        Vector3[] vertices = new Vector3[(simplifiedVI) * (simplifiedVI)];

        for (int x = 0, vertexIndex = 0; x < size; x += meshSimplificationIncrement)
        {
            for (int y = 0; y < size; y += meshSimplificationIncrement, vertexIndex++)
            {
                lock (heightEvaluator)
                {
                    vertices[vertexIndex] = new Vector3(x, heightEvaluator.Evaluate(noise[x, y]) * scale, y);
                }
            }
        }
        return vertices;
    }

    public static Vector2[] uvs(int size, int levelOfDetail)
    {
        if (levelOfDetail <= 0)
        {
            levelOfDetail = 1;
        }

        //Level Of Detail
        int meshSimplificationIncrement = levelOfDetail * 2;
        int simplifiedVI = (size - 1) / meshSimplificationIncrement + 1;

        //Calculating Vertices, we need an index to tell us where we are
        //While we are at it we calculate the UVs of the mesh as well
        Vector2[] uvs = new Vector2[simplifiedVI * simplifiedVI];

        for (int x = 0, vertexIndex = 0; x < size; x += meshSimplificationIncrement)
        {
            for (int y = 0; y < size; y += meshSimplificationIncrement, vertexIndex++)
            {
                uvs[vertexIndex] = new Vector2(x / (float)size, y / (float)size);
            }
        }
        return uvs;
    }

}