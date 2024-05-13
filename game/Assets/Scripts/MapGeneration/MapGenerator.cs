using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.Serialization;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColorMap,
        Mesh
    }

    public DrawMode drawMode;
    public Noise.NormalizeMode normalizeMode;

    public const int MapChunkSize = 239;
    [Range(0, 6)] public int editorPreviewLOD;
    public float noiseScale = 0.3f;
    public int octaves = 4;
    [Range(0, 1)] public float persistance = 0.5f;
    public float lacunarity = 2f;
    public int seed;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public Vector2 offset;

    public bool autoUpdate = true;
    public TerrainType[] regions;

    private Queue<MapThreadInfo<MapData>> _mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    private Queue<MapThreadInfo<MeshData>> _meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.HeightMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.ColorMap, MapChunkSize, MapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(
                MeshGenerator.GenerateTerrainMesh(mapData.HeightMap, meshHeightMultiplier, meshHeightCurve,
                    editorPreviewLOD),
                TextureGenerator.TextureFromColorMap(mapData.ColorMap, MapChunkSize, MapChunkSize));
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate { MapDataThread(center, callback); };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center);
        lock (_mapDataThreadInfoQueue)
        {
            _mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate { MeshDataThread(mapData, lod, callback); };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData =
            MeshGenerator.GenerateTerrainMesh(mapData.HeightMap, meshHeightMultiplier, meshHeightCurve, lod);
        lock (_meshDataThreadInfoQueue)
        {
            _meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {
        if (_mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < _mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = _mapDataThreadInfoQueue.Dequeue();
                threadInfo.Callback(threadInfo.Parameter);
            }
        }

        if (_meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < _meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = _meshDataThreadInfoQueue.Dequeue();
                threadInfo.Callback(threadInfo.Parameter);
            }
        }
    }

    private MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(MapChunkSize + 2, MapChunkSize + 2, seed, noiseScale, octaves,
            persistance, lacunarity, center + offset, normalizeMode);

        Color[] colorMap = new Color[MapChunkSize * MapChunkSize];
        for (int y = 0; y < MapChunkSize; y++)
        {
            for (int x = 0; x < MapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight >= regions[i].height)
                    {
                        colorMap[y * MapChunkSize + x] = regions[i].color;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap);
    }

    private void OnValidate()
    {
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> Callback;
        public readonly T Parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.Callback = callback;
            this.Parameter = parameter;
        }
    }
}

[Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public struct MapData
{
    public readonly float[,] HeightMap;
    public readonly Color[] ColorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.HeightMap = heightMap;
        this.ColorMap = colorMap;
    }
}