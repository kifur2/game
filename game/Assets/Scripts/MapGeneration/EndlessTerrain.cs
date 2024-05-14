using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    private const float ViewerMoveThresholdForChunkUpdate = 25f;

    private const float SquareViewerMoveThresholdForChunkUpdate =
        ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate;

    private const float Scale = 2f;

    public LODInfo[] detailLevels;
    public static float MaxViewDistance;
    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 ViewerPosition;
    public static Vector2 ViewerPositionOld;
    private static MapGenerator _mapGenerator;

    private int _chunkSize;
    private int _chunksVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    private static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();


    private void Start()
    {
        _mapGenerator = FindObjectOfType<MapGenerator>();
        MaxViewDistance = detailLevels[^1].visibleDistanceTreshold;
        _chunkSize = MapGenerator.mapChunkSize - 1;
        _chunksVisibleInViewDistance = Mathf.RoundToInt(MaxViewDistance / _chunkSize);
        UpdateVisibleChunks();
    }

    private void Update()
    {
        ViewerPosition = new Vector2(viewer.position.x, viewer.position.z) / Scale;
        if ((ViewerPositionOld - ViewerPosition).sqrMagnitude > SquareViewerMoveThresholdForChunkUpdate)
        {
            ViewerPositionOld = ViewerPosition;
            UpdateVisibleChunks();
        }
    }


    void UpdateVisibleChunks()
    {
        foreach (var terrainChunk in terrainChunksVisibleLastUpdate)
        {
            terrainChunk.SetVisible(false);
        }

        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / _chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / _chunkSize);

        for (int yOffset = -_chunksVisibleInViewDistance; yOffset <= _chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -_chunksVisibleInViewDistance; xOffset <= _chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (_terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    _terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    _terrainChunkDictionary.Add(viewedChunkCoord,
                        new TerrainChunk(viewedChunkCoord, _chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        private GameObject _meshObject;
        private Vector2 _position;
        private Bounds _bounds;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private LODInfo[] detailLevels;
        private LODMesh[] _lodMeshes;
        private LODMesh _collisionLODMesh;

        private MapData _mapData;
        private bool _mapDataReceived;
        private int previousLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;
            _position = coord * size;
            _bounds = new Bounds(_position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(_position.x, 0, _position.y);

            _meshObject = new GameObject("TerrainChunk");

            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshCollider = _meshObject.AddComponent<MeshCollider>();
            _meshRenderer.material = material;
            _meshObject.transform.position = positionV3 * Scale;
            _meshObject.transform.parent = parent;
            _meshObject.transform.localScale = Vector3.one * Scale;
            SetVisible(false);

            _lodMeshes = new LODMesh[this.detailLevels.Length];
            for (var i = 0; i < this.detailLevels.Length; i++)
            {
                _lodMeshes[i] = new LODMesh(this.detailLevels[i].lod, UpdateTerrainChunk);
                if (detailLevels[i].useForCollider)
                {
                    _collisionLODMesh = _lodMeshes[i];
                }
            }

            _mapGenerator.RequestMapData(_position, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData)
        {
            _mapData = mapData;
            _mapDataReceived = true;

            Texture2D texture2D = TextureGenerator.TextureFromColorMap(mapData.ColorMap, MapGenerator.mapChunkSize,
                MapGenerator.mapChunkSize);
            _meshRenderer.material.mainTexture = texture2D;

            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (!_mapDataReceived) return;
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));
            bool visible = viewerDistanceFromNearestEdge <= MaxViewDistance;
            if (visible)
            {
                int lodIndex = 0;
                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDistanceFromNearestEdge > detailLevels[i].visibleDistanceTreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = _lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        _meshFilter.mesh = lodMesh.mesh;
                        _meshCollider.sharedMesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(_mapData);
                    }
                }

                if (lodIndex == 0)
                {
                    if (_collisionLODMesh.hasMesh)
                    {
                        _meshCollider.sharedMesh = _collisionLODMesh.mesh;
                    }else if (!_collisionLODMesh.hasRequestedMesh)
                    {
                        _collisionLODMesh.RequestMesh(_mapData);
                    }
                }
                
                terrainChunksVisibleLastUpdate.Add(this);
            }

            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            _meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return _meshObject.activeSelf;
        }
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        private int lod;
        private Action updateCallback;

        public LODMesh(int lod, Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshdata)
        {
            mesh = meshdata.CreateMesh();
            hasMesh = true;
            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            _mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }
    }

    [Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDistanceTreshold;
        public bool useForCollider;
    }
}