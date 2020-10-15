using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void GenerateChunk();
    public static event GenerateChunk ChunkGenerationEvent;

    private void Update()
    {
        ChunkGenerationEvent?.Invoke();
        
    }
}
