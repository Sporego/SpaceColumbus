using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindCompute : MonoBehaviour
{
    const int threadGroupSize = 1024;

    public ComputeShader FlowCompute;

    public int iterations = 50;

    public int Height;
    public int Width;

    public float decay = 0.9f;

    private int[,] FlowGridIn;

    private void Start()
    {
        Debug.Log("Recomputing");

        FlowGridIn = new int[Height, Width];

        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
            {
                FlowGridIn[i, j] = 0;
            }

        // setup
        FlowGridIn[0, 0] = 255;
        FlowGridIn[50, 50] = 255;
        FlowGridIn[50, 51] = 255;
        FlowGridIn[50, 52] = 255;
        FlowGridIn[51, 51] = 255;
        FlowGridIn[6, 5] = 255;
        FlowGridIn[6, 1] = 255;

        long time1 = DateTime.Now.Ticks;

        Compute(iterations);

        long time2 = DateTime.Now.Ticks;
        Debug.Log("it took [" + (time2 - time1) / TimeSpan.TicksPerMillisecond + "]");

        Display();
    }

    void Display()
    {
        Texture2D tex = new Texture2D(Width, Height);
        tex.anisoLevel = 0;
        tex.filterMode = FilterMode.Point;
        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
            {
                float val = 1f * FlowGridIn[i, j] / (iterations + 1);

                if (val > 1)
                    Debug.Log("found " + val);
                tex.SetPixel(j, i, new Color(val, 0, 0, 1));
            }
        tex.Apply();
        this.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", tex);
    }

    void Compute(int iterations = 1)
    {
        int count = Height * Width;

        FlowCompute.SetInt("height", Height);
        FlowCompute.SetInt("width", Width);
        FlowCompute.SetInt("count", count);
        FlowCompute.SetFloat("decay", decay);

        var buffer = new ComputeBuffer(count, FlowData.Size);

        for (int i = 0; i < iterations; i++)
            Compute(buffer, count);

        buffer.Release();
    }

    void Compute(ComputeBuffer buffer, int count)
    {
    
        FlowData[] flowData = new FlowData[count];
        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
            {
                int index = i * Width + j;
                flowData[index].amountIn = FlowGridIn[i, j];
            }

        buffer.SetData(flowData);

        FlowCompute.SetBuffer(0, "flowData", buffer);

        int threadGroupsX = Mathf.CeilToInt(count / (float)threadGroupSize);
        int threadGroupsY = Mathf.CeilToInt(threadGroupsX / (float)threadGroupSize);
        FlowCompute.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        buffer.GetData(flowData);

        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
            {
                int index = i * Width + j;
                FlowGridIn[i, j] = flowData[index].amountOut;
            }

        Debug.Log("threadGroups [" + threadGroupsX + ", " + threadGroupsY + "]");
    }

    private void OnValidate()
    {
        Start();
    }

    public struct FlowData
    {
        public int amountIn;
        public int amountOut;

        public static int Size
        {
            get
            {
                return sizeof(int) * 2;
            }
        }
    }
}


