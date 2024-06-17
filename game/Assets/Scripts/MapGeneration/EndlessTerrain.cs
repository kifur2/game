using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EndlessTerrain : MonoBehaviour
{
    private const float ViewerMoveThresholdForChunkUpdate = 25f;

    private const float SquareViewerMoveThresholdForChunkUpdate =
        ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate;

    private const float Scale = 2f;

    public LODInfo[] detailLevels;
    public static float MaxViewDistance;
    public static float MaxBakeNavDistance;
    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 ViewerPosition;
    public static Vector2 ViewerPositionOld;
    public static MapGenerator MapGenerator;

    private static int _chunkSize;
    private int _chunksVisibleInViewDistance;

    private static Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private static HashSet<TerrainChunk> _terrainChunksVisibleLastUpdate = new HashSet<TerrainChunk>();

    private void Start()
    {
        MapGenerator = FindObjectOfType<MapGenerator>();
        _chunkSize = MapGenerator.MapChunkSize - 1;
        MaxViewDistance = detailLevels[^1].visibleDistanceTreshold;
        MaxBakeNavDistance = MaxViewDistance / 4;
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
        foreach (var terrainChunk in _terrainChunksVisibleLastUpdate)
        {
            terrainChunk.SetVisible(false);
        }

        _terrainChunksVisibleLastUpdate.Clear();

        var currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / _chunkSize);
        var currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / _chunkSize);

        for (var yOffset = -_chunksVisibleInViewDistance; yOffset <= _chunksVisibleInViewDistance; yOffset++)
        {
            for (var xOffset = -_chunksVisibleInViewDistance; xOffset <= _chunksVisibleInViewDistance; xOffset++)
            {
                var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (_terrainChunkDictionary.TryGetValue(viewedChunkCoord, out var chunk))
                {
                    chunk.UpdateTerrainChunk();
                }
                else
                {
                    var newChunk = new TerrainChunk(viewedChunkCoord, _chunkSize, detailLevels, transform, mapMaterial);
                    _terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                }
            }
        }
    }

    public float GetHeightAtPosition(Vector2 position)
    {
        int currentChunkCoordX = Mathf.RoundToInt(position.x / _chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(position.y / _chunkSize);

        Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX, currentChunkCoordY);
        if (_terrainChunkDictionary.TryGetValue(viewedChunkCoord, out var chunk))
        {
            return chunk.GetHeightAtPosition(position);
        }
        else
        {
            throw new KeyNotFoundException("No current chunk in dictionary");
        }
    }

    private void OnDestroy()
    {
        _terrainChunkDictionary.Clear();
        _terrainChunksVisibleLastUpdate.Clear();
    }

    public class TerrainChunk
    {
        private GameObject _meshObject;
        private Vector2 _position;
        private Bounds _bounds;
        private Vector2 _coords;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private LODInfo[] _detailLevels;
        private LODMesh[] _lodMeshes;
        private LODMesh _collisionLODMesh;

        public MapData MapData;
        public MapData TreeMapData;
        private bool _mapDataReceived;
        private int _previousLODIndex = -1;
        private NavMeshSurface _navMeshSurface;
        private static List<(Vector2, Vector2)> _createdLinks = new List<(Vector2, Vector2)>();

        public TerrainChunk(Vector2 coords, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            _detailLevels = detailLevels;
            _position = coords * size;
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

            _lodMeshes = new LODMesh[_detailLevels.Length];
            for (var i = 0; i < _detailLevels.Length; i++)
            {
                _lodMeshes[i] = new LODMesh(_detailLevels[i].lod, UpdateTerrainChunk);
                if (detailLevels[i].useForCollider)
                {
                    _collisionLODMesh = _lodMeshes[i];
                }
            }

            AddNavMeshSurfaceToChunk();
            MapGenerator.RequestMapData(_position, OnMapDataReceived);
        }

        void AddNavMeshSurfaceToChunk()
        {
            _navMeshSurface = _meshObject.AddComponent<NavMeshSurface>();

            _navMeshSurface.agentTypeID = 0;
            _navMeshSurface.collectObjects = CollectObjects.Children;
            _navMeshSurface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
            _navMeshSurface.layerMask = LayerMask.GetMask("Default");
            _navMeshSurface.overrideTileSize = true;
            _navMeshSurface.tileSize = 256;
            _navMeshSurface.overrideVoxelSize = true;
            _navMeshSurface.voxelSize = 0.1f;
        }

        private void CreateNavMeshLinks(TerrainChunk chunk, Vector2 viewedChunkCoord)
        {
            var adjacentCoords = new Vector2[]
            {
                new(viewedChunkCoord.x + 1, viewedChunkCoord.y),
                new(viewedChunkCoord.x - 1, viewedChunkCoord.y),
                new(viewedChunkCoord.x, viewedChunkCoord.y + 1),
                new(viewedChunkCoord.x, viewedChunkCoord.y - 1)
            };

            foreach (var adjacentCoord in adjacentCoords)
            {
                if (!_terrainChunkDictionary.TryGetValue(adjacentCoord, out var adjacentChunk)) continue;
                if (LinkExists(viewedChunkCoord, adjacentCoord)) continue;
                chunk.CreateNavMeshLink(adjacentChunk, _meshObject.transform.parent);
                _createdLinks.Add((viewedChunkCoord, adjacentCoord));
                _createdLinks.Add((adjacentCoord, viewedChunkCoord));
            }
        }

        private bool LinkExists(Vector2 coord1, Vector2 coord2)
        {
            return _createdLinks.Contains((coord1, coord2)) || _createdLinks.Contains((coord2, coord1));
        }

        public void CreateNavMeshLink(TerrainChunk adjacentChunk, Transform parent)
        {
            if (adjacentChunk != null)
            {
                AddNavMeshLink(this, adjacentChunk, parent);
            }
        }

        public void AddNavMeshLink(TerrainChunk fromChunk, TerrainChunk toChunk, Transform parent)
        {
            NavMeshLink navMeshLink = new GameObject("NavMeshLink").AddComponent<NavMeshLink>();
            navMeshLink.transform.parent = parent;

            Vector3 fromPosition = fromChunk._meshObject.transform.localPosition;
            Vector3 toPosition = toChunk._meshObject.transform.localPosition;

            navMeshLink.startPoint = new Vector3(fromPosition.x,
                GetHeightAtPosition(new Vector2(fromPosition.x, fromPosition.z)), fromPosition.z);
            navMeshLink.endPoint = new Vector3(toPosition.x,
                GetHeightAtPosition(new Vector2(toPosition.x, toPosition.z)), toPosition.z);
            navMeshLink.width = 200f;
            navMeshLink.costModifier = -1;
            navMeshLink.bidirectional = true;
            navMeshLink.area = 0;
            navMeshLink.agentTypeID = 0;
        }

        void OnMapDataReceived(MapData mapData, MapData treeMapData)
        {
            MapData = mapData;
            TreeMapData = treeMapData;
            _mapDataReceived = true;

            Texture2D texture2D = TextureGenerator.TextureFromColorMap(mapData.ColorMap, MapGenerator.MapChunkSize,
                MapGenerator.MapChunkSize);
            _meshRenderer.material.mainTexture = texture2D;

            UpdateTerrainChunk();
        }

        public float GetHeightAtPosition(Vector2 position)
        {
            float percentX = (position.x - _position.x) / _chunkSize;
            float percentY = (position.y - _position.y) / _chunkSize;

            int x = Mathf.Clamp(Mathf.RoundToInt(percentX * (MapData.HeightMap.GetLength(0) - 1)), 0,
                MapData.HeightMap.GetLength(0) - 1);
            int y = Mathf.Clamp(Mathf.RoundToInt(percentY * (MapData.HeightMap.GetLength(1) - 1)), 0,
                MapData.HeightMap.GetLength(1) - 1);

            return MapGenerator.meshHeightCurve.Evaluate(MapData.HeightMap[x, y]) * MapGenerator.meshHeightMultiplier;
        }

        public void BuildNavMesh()
        {
            if (_navMeshSurface.navMeshData == null)
            {
                _navMeshSurface.navMeshData = new NavMeshData();
            }

            _navMeshSurface.BuildNavMesh();
        }

        public void UpdateNavMesh()
        {
            _navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);
        }

        public void UpdateTerrainChunk()
        {
            if (!_mapDataReceived) return;

            float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));
            bool visible = viewerDistanceFromNearestEdge <= MaxViewDistance;

            int lodIndex = 0;
            if (visible)
            {
                for (int i = 0; i < _detailLevels.Length - 1; i++)
                {
                    if (viewerDistanceFromNearestEdge > _detailLevels[i].visibleDistanceTreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != _previousLODIndex)
                {
                    LODMesh lodMesh = _lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        _previousLODIndex = lodIndex;
                        _meshFilter.mesh = lodMesh.mesh;
                        _meshCollider.sharedMesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(MapData);
                    }
                }

                if (lodIndex == 0)
                {
                    if (_collisionLODMesh.hasMesh)
                    {
                        _meshCollider.sharedMesh = _collisionLODMesh.mesh;
                    }
                    else if (!_collisionLODMesh.hasRequestedMesh)
                    {
                        _collisionLODMesh.RequestMesh(MapData);
                    }
                }

                _terrainChunksVisibleLastUpdate.Add(this);
            }

            SetVisible(visible);

            if (visible && lodIndex==0)
            {
                CreateNavMeshLinks(this, _coords);
                BuildNavMesh();
            }
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

        void OnMeshDataReceived(MeshData meshdata, MeshData treeMeshData)
        {
            mesh = meshdata.CreateMesh();
            hasMesh = true;
            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            MapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
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