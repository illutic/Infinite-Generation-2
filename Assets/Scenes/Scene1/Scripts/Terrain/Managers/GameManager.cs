﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Instance Creation
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject.FindGameObjectWithTag("Manager").AddComponent<GameManager>();
            }
            return _instance;
        }

    }
    
    //Variable Initialization
    public Attributes GlobalAttributes;
    public Dictionary<Vector2, Chunk> terrainChunks = new Dictionary<Vector2, Chunk>();
    public List<Chunk> terrainChunksVisibleLastFrame = new List<Chunk>();
    public GameObject Player;
    public Chunk chunkPrefab;
    Transform terrainParent;
    Vector2 viewerPosition;
    public int viewedChunks;



    public void updateChunks()
    {
        viewerPosition = new Vector2(Player.transform.position.x,Player.transform.position.z);
        for (int i = 0; i < terrainChunksVisibleLastFrame.Count; i++)
        {
            terrainChunksVisibleLastFrame[i].setVisible(false);
        }
        terrainChunksVisibleLastFrame.Clear();

        float currentChunkCoordX = viewerPosition.x / GlobalAttributes.size;
        float currentChunkCoordZ = viewerPosition.y / GlobalAttributes.size;

        for (int yOffset = -viewedChunks; yOffset <= viewedChunks; yOffset++)
        {
            for (int xOffset = -viewedChunks; xOffset <= viewedChunks; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(Mathf.RoundToInt(currentChunkCoordX) + xOffset, Mathf.RoundToInt(currentChunkCoordZ) + yOffset);

                if (terrainChunks.ContainsKey(viewedChunkCoord))
                {
                    terrainChunks[viewedChunkCoord].UpdateTerrainChunk();

                    if (terrainChunks[viewedChunkCoord].isVisible())
                    {
                        terrainChunksVisibleLastFrame.Add(terrainChunks[viewedChunkCoord]);
                    }
                }
                else
                {
                    Vector3 chunkPosition = new Vector3(viewedChunkCoord.x * GlobalAttributes.size -0.5f , 0, viewedChunkCoord.y * GlobalAttributes.size + 0.5f);
                    terrainChunks.Add(viewedChunkCoord, Instantiate(chunkPrefab,chunkPosition,Quaternion.identity,terrainParent));
                }
            }
        }
    }
    private void Awake()
    {
        _instance = this;
        terrainParent = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Transform>();
    }
    private void FixedUpdate()
    {
        if (ThreadManager.chunks != null)
        {
            if (ThreadManager.chunks.Count != 0)
            {
                lock (ThreadManager.chunks)
                {
                    ThreadInfo<Chunk> chunkThreadInfo = ThreadManager.chunks.Dequeue();
                    chunkThreadInfo.callback.Invoke(chunkThreadInfo.parameter);
                }
            }
        }
        updateChunks();
    }
}