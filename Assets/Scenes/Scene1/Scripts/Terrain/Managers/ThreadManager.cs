using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class ThreadManager : MonoBehaviour
{
    private static ThreadManager _instance;
    public static ThreadManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject.FindGameObjectWithTag("Manager").AddComponent<ThreadManager>();
            }
            return _instance;
        }

    }

    public static Queue<ThreadInfo<Chunk>> chunks;

    public void RequestMesh(Action<Chunk> callback, Chunk parameter)
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
        lock (chunks)
        {
            chunks.Enqueue(new ThreadInfo<Chunk>(callback, parameter));
        }
    }

    private void Awake()
    {
        chunks = new Queue<ThreadInfo<Chunk>>();
        _instance = this;
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