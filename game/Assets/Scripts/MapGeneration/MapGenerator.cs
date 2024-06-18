using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

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

    public bool useFlatShading;
    [Range(0, 6)] public int editorPreviewLOD;
    public float noiseScale = 0.3f;
    public float treeNoiseScale = 0.9f;
    public int octaves = 4;
    [Range(0, 1)] public float persistance = 0.5f;
    public float lacunarity = 2f;
    public float treeLacunarity = 20f;
    public int seed;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public Vector2 offset;

    public bool autoUpdate = true;
    public TerrainType[] regions;

    private static MapGenerator _instance;

    private Queue<MapThreadInfo<MapData, MapData>> _mapDataThreadInfoQueue = new();

    private Queue<MapThreadInfo<MeshData, MeshData>> _meshDataThreadInfoQueue = new();

    public static int MapChunkSize
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MapGenerator>();
            }

            return _instance.useFlatShading ? 95 : 239;
        }
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero, seed, noiseScale, lacunarity);
        MapData treeMapData = GenerateMapData(Vector2.zero, seed + 2, treeNoiseScale, treeLacunarity);

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
                    editorPreviewLOD, useFlatShading),
                TextureGenerator.TextureFromColorMap(mapData.ColorMap, MapChunkSize, MapChunkSize));
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData, MapData> callback)
    {
        ThreadStart threadStart = delegate { MapDataThread(center, callback); };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center, Action<MapData, MapData> callback)
    {
        MapData mapData = GenerateMapData(center, seed, noiseScale, lacunarity);
        MapData treeMapData = GenerateMapData(center, seed + 2, treeNoiseScale, treeLacunarity);
        lock (_mapDataThreadInfoQueue)
        {
            _mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData, MapData>(callback, mapData, treeMapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData, MeshData> callback)
    {
        ThreadStart threadStart = delegate { MeshDataThread(mapData, lod, callback); };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData, MeshData> callback)
    {
        MeshData meshData =
            MeshGenerator.GenerateTerrainMesh(mapData.HeightMap, meshHeightMultiplier, meshHeightCurve, lod,
                useFlatShading);
        lock (_meshDataThreadInfoQueue)
        {
            _meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData, MeshData>(callback, meshData, null));
        }
    }

    private void Update()
    {
        if (_mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < _mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData, MapData> threadInfo = _mapDataThreadInfoQueue.Dequeue();
                threadInfo.Callback(threadInfo.Parameter1, threadInfo.Parameter2);
            }
        }

        if (_meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < _meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData, MeshData> threadInfo = _meshDataThreadInfoQueue.Dequeue();
                threadInfo.Callback(threadInfo.Parameter1, threadInfo.Parameter2);
            }
        }
    }

    private MapData GenerateMapData(Vector2 center, int currentSeed, float noiseScale, float lacunarity)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(MapChunkSize + 2, MapChunkSize + 2, currentSeed, noiseScale, octaves,
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

    struct MapThreadInfo<T1, T2>
    {
        public readonly Action<T1, T2> Callback;
        public readonly T1 Parameter1;
        public readonly T2 Parameter2;

        public MapThreadInfo(Action<T1, T2> callback, T1 parameter1, T2 parameter2)
        {
            this.Callback = callback;
            this.Parameter1 = parameter1;
            this.Parameter2 = parameter2;
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