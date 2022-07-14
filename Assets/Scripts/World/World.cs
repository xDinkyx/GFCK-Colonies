using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class World : MonoBehaviour
{
    public int worldChunksX = 4;
    public int worldChunksZ = 4;
    public Vector3 worldOrigin = Vector3.zero;

    public int chunkSize = 16;
    public int maxY = 16;

    public float baseNoise = 0.02f;
    public float baseNoiseHeight = 4;
    public float frequency = 0.005f;

    public NoiseBase[] noisePatterns;

    public int maxWorkers = 4;
    List<ChunkGenerator> toDoWorkers = new List<ChunkGenerator>();
    List<ChunkGenerator> currentWorkers = new List<ChunkGenerator>();

    public Material material;

    private void Start()
    {
        CreateWorld();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            CreateWorld();
        }

        UpdateGeneratorThreads();
    }

    private void UpdateGeneratorThreads()
    {
        int i = 0;
        while (i < currentWorkers.Count)
        {
            if (currentWorkers[i].GenerationCompleted)
            {
                currentWorkers[i].NotifyCompleted();
                currentWorkers.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        if (toDoWorkers.Count > 0
        && currentWorkers.Count < maxWorkers)
        {
            ChunkGenerator generator = toDoWorkers[0];
            toDoWorkers.RemoveAt(0);
            currentWorkers.Add(generator);

            Thread workerThread = new Thread(generator.GenerateWorld);
            workerThread.Start();
        }
    }

    public void CreateWorld()
    {
        Reset();

        CreateChunks();
    }

    private void CreateChunks()
    {
        for (int x = 0; x < worldChunksX; x++)
        {
            for (int z = 0; z < worldChunksZ; z++)
            {
                Vector3 chunkPos = worldOrigin;
                chunkPos.x += x * chunkSize;
                chunkPos.z += z * chunkSize;

                RequestWorldChunkGeneration(chunkPos);
            }
        }
    }

    public void RequestWorldChunkGeneration(Vector3 position)
    {
        ChunkGenerator generator = new(CreateChunkStats(position), LoadChunkData);
        toDoWorkers.Add(generator);
    }

    private void LoadChunkData(BlockGrid grid, ChunkMeshData data)
    {
        LoadChunkMeshData(data);
    }

    public void LoadChunkMeshData(ChunkMeshData data)
    {
        Mesh chunkMesh = new Mesh()
        {
            vertices = data.vertices.ToArray(),
            uv = data.uv.ToArray(),
            triangles = data.triangles.ToArray()
        };

        chunkMesh.RecalculateNormals();

        // Create new chunk object
        GameObject newChunkObject = new GameObject("Chunk" + data.origin.ToString());
        newChunkObject.isStatic = true;
        newChunkObject.transform.parent = transform;
        newChunkObject.transform.position = data.origin;

        MeshFilter meshFilter = newChunkObject.AddComponent<MeshFilter>();
        meshFilter.mesh = chunkMesh;

        MeshRenderer renderer = newChunkObject.AddComponent<MeshRenderer>();
        renderer.material = material;

        newChunkObject.AddComponent<MeshCollider>();
    }

    private ChunkStats CreateChunkStats(Vector3 position)
    {
        return new ChunkStats
        {
            chunkSize = this.chunkSize,
            maxY = this.maxY,
            baseNoise = this.baseNoise,
            baseNoiseHeight = this.baseNoiseHeight,
            frequency = this.frequency,
            origin = position,
            noisePatterns = this.noisePatterns,
        };
    }

    private void Reset()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
