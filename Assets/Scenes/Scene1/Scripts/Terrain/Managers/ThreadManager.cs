using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class ThreadManager : MonoBehaviour
{
    public static ThreadManager Instance { get; } = GameObject.FindGameObjectWithTag("Manager").AddComponent<ThreadManager>();

    public static Queue<ThreadInfo<Chunk>> chunks;

    public void RequestChunk(Action<Chunk> callback, Chunk parameter)
    {
        ThreadStart threadStart = delegate
        {
            MeshThread(callback, parameter);
        };

        new Thread(threadStart).Start();
    }
    private void MeshThread(Action<Chunk> callback, Chunk parameter)
    {
        parameter.PreGenerate();
        parameter.GenerateBiomeVertices();
        lock (chunks)
        {
            chunks.Enqueue(new ThreadInfo<Chunk>(callback, parameter));
        }
    }

    private void Awake()
    {
        chunks = new Queue<ThreadInfo<Chunk>>();
    }

}
public struct ThreadInfo<T>
{
    public readonly Action<T> callback;
    public readonly T parameter;

    public ThreadInfo(Action<T> callback, T parameter)
    {
        this.callback = callback;
        this.parameter = parameter;
    }
}