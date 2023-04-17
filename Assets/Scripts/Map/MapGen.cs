using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    public static MapGen instance;
    public WorldGen worldGen;
    public Vector3 mapChunkSize = new Vector3(2, 2, 1);
    public int chunkSize = 20;
    public int seed = 2137;
    public float cut = 120;
    public float scale = 0.1f;
    public Material material;
    public ChunkGen[,,] chunks;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        chunks = new ChunkGen[chunkSize, chunkSize, chunkSize];
        for (int z = (int)mapChunkSize.z - 1; z >= 0; z--)
        {
            for (int y = (int)mapChunkSize.y - 1; y >= 0; y--)
            {
                for (int x = (int)mapChunkSize.x - 1; x >= 0; x--)
                {
                    GameObject a = new GameObject();
                    a.transform.SetParent(this.transform);
                    a.transform.position = new Vector3(x, y, z) * chunkSize;
                    a.AddComponent<ChunkGen>();
                    ChunkGen cg = a.GetComponent<ChunkGen>();
                    cg.seed = seed;
                    cg.width = chunkSize;
                    cg.height = chunkSize;
                    cg.length = chunkSize;
                    cg.cut = cut;
                    cg.scale = scale;
                    cg.material = material;
                    cg.GenerateChunk(worldGen.Algorithm(chunkSize, chunkSize, chunkSize, new Vector3(x, y, z) * chunkSize));
                    chunks[x, y, z] = a.GetComponent<ChunkGen>();
                }
            }
        }
    }

    public void UpdateMap(Vector3 position, float digRadius, float delta)
    {
        Vector3 chunk = position / chunkSize;
        chunk.x = (int)chunk.x;
        chunk.y = (int)chunk.y;
        chunk.z = (int)chunk.z;

        for (int z = chunkSize; z >= 0; z--)
        {
            for (int y = chunkSize; y >= 0; y--)
            {
                for (int x = chunkSize; x >= 0; x--)
                {
                    //Debug.Log(Mathf.Sqrt(Mathf.Pow(x - position.x, 2) + Mathf.Pow(y - position.y, 2) + Mathf.Pow(z - position.z, 2)));
                    //Debug.Log(digRadius);
                    if (Mathf.Sqrt(Mathf.Pow(x - position.x, 2) + Mathf.Pow(y - position.y, 2) + Mathf.Pow(z - position.z, 2)) < digRadius)
                    {
                        chunks[(int)chunk.x, (int)chunk.y, (int)chunk.z].UpdateVertexValue(x, y, z, delta);
                        chunks[(int)chunk.x, (int)chunk.y, (int)chunk.z].enabled = false;
                        chunks[(int)chunk.x, (int)chunk.y, (int)chunk.z].enabled = true;
                        //Debug.Log(chunk);
                    }
                }
            }
        }
    }

}
