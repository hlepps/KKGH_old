using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class ChunkGen : MonoBehaviour
{
    public int seed;
    public int width;
    public int height;
    public int length;
    public float scale;

    public float cut;

    public Material material;



    private float[,,] vertexValues;
    private float[,,] lastChanged;

    private float t = 0;
    private bool finished = false;

    /*
    void OnDrawGizmosSelected()
    {
        for(int i = 0; i < vertices.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(vertices[i], normals[i]);
        }
    }
    */


    Vector3 GetChunkPos()
    {
        return new Vector3((transform.localPosition.x / MapGen.instance.chunkSize),
            (transform.localPosition.y / MapGen.instance.chunkSize),
            (transform.localPosition.z / MapGen.instance.chunkSize));
    }

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector3> normals = new List<Vector3>();
    Mesh mesh;

    public void UpdateVertexValue(int x, int y, int z, float delta)
    {
        if (finished)
        {
            vertexValues[x, y, z] += delta;
            finished = false;
            StartCoroutine(CreateMesh());
        }
    }
    public void SetVertexValue(int x, int y, int z, float v)
    {
        if (finished)
        {
            vertexValues[x, y, z] = v;
            finished = false;
            StartCoroutine(CreateMesh());
        }
    }
    public float GetVertexValues(int x, int y, int z)
    {
        if(x < width && y < height && z < length)
        {
            return vertexValues[x, y, z];
        }
        if (x >= width && y >= height && z >= length)
        {
            if (MapGen.instance.mapChunkSize.x - 1 != GetChunkPos().x &&
                MapGen.instance.mapChunkSize.y - 1 != GetChunkPos().y &&
                MapGen.instance.mapChunkSize.z - 1 != GetChunkPos().z)
            {
                return MapGen.instance.chunks[(int)GetChunkPos().x + 1, (int)GetChunkPos().y + 1, (int)GetChunkPos().z + 1].vertexValues[0, 0, 0];
            }
            else return 0;
        }
        if (x >= width && y >= height)
        {
            if (MapGen.instance.mapChunkSize.x - 1 != GetChunkPos().x &&
                MapGen.instance.mapChunkSize.y - 1 != GetChunkPos().y)
            {
                return MapGen.instance.chunks[(int)GetChunkPos().x + 1, (int)GetChunkPos().y + 1, (int)GetChunkPos().z].vertexValues[0, 0, z];
            }
            else return 0;
        }
        if (x >= width && z >= length)
        {
            if (MapGen.instance.mapChunkSize.x - 1 != GetChunkPos().x &&
                MapGen.instance.mapChunkSize.z - 1 != GetChunkPos().z)
            {
                return MapGen.instance.chunks[(int)GetChunkPos().x + 1, (int)GetChunkPos().y, (int)GetChunkPos().z + 1].vertexValues[0, y, 0];
            }
            else return 0;
        }
        if (y >= height && z >= length)
        {
            if (MapGen.instance.mapChunkSize.y - 1 != GetChunkPos().y &&
                MapGen.instance.mapChunkSize.z - 1 != GetChunkPos().z)
            {
                return MapGen.instance.chunks[(int)GetChunkPos().x, (int)GetChunkPos().y + 1, (int)GetChunkPos().z + 1].vertexValues[x, 0, 0];
            }
            else return 0;
        }
        if (x >= width)
        {
            if (MapGen.instance.mapChunkSize.x - 1 != GetChunkPos().x)
            {
                //Debug.Log(x + " " + y + " " + z);
                //Debug.Log(MapGen.instance.mapChunkSize.x + " " + GetChunkPos().x);
                return MapGen.instance.chunks[(int)GetChunkPos().x + 1, (int)GetChunkPos().y, (int)GetChunkPos().z].vertexValues[0, y, z];
            }
            else return 0;
        }
        if (y >= height)
        {
            if (MapGen.instance.mapChunkSize.y - 1 != GetChunkPos().y)
            {
                return MapGen.instance.chunks[(int)GetChunkPos().x, (int)GetChunkPos().y + 1, (int)GetChunkPos().z].vertexValues[x, 0, z];
            }
            else return 0;
        }
        if (z >= length)
        {
            if (MapGen.instance.mapChunkSize.z - 1 != GetChunkPos().z)
            {
                return MapGen.instance.chunks[(int)GetChunkPos().x, (int)GetChunkPos().y, (int)GetChunkPos().z+1].vertexValues[x, y, 0];
            }
            else return 0;
        }

        return vertexValues[x, y, z];
    }

    
    public void GenerateChunk(float[,,] values)
    {
        mesh = new Mesh();
        SimplexNoise.Noise.Seed = seed;
        vertexValues = values;
        finished = false;
        gameObject.layer = LayerMask.NameToLayer("Terrain");


        gameObject.AddComponent<MeshRenderer>();
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = material;
        gameObject.AddComponent<MeshFilter>();
        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = mesh;
        gameObject.AddComponent<MeshCollider>();
        GetComponent<MeshCollider>().cookingOptions = MeshColliderCookingOptions.None;
        gameObject.AddComponent<Rigidbody>();
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        lastChanged = new float[width, height, length];
        for (int z = 0; z < length; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    lastChanged[x, y, z] = 0;
                }
            }
        }

        StartCoroutine(CreateMesh());
    }

    IEnumerator CreateMesh()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        normals = new List<Vector3>();
        for (int z = 0; z < length; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //Debug.Log(vertexValues[x, y, z] + " " + lastChanged[x, y, z]);

                    int index = 0;
                    if (GetVertexValues(x, y, z) < cut) index += 1;
                    if (GetVertexValues(x + 1, y, z) < cut) index += 2;
                    if (GetVertexValues(x + 1, y, z + 1) < cut) index += 4;
                    if (GetVertexValues(x, y, z + 1) < cut) index += 8;
                    if (GetVertexValues(x, y + 1, z) < cut) index += 16;
                    if (GetVertexValues(x + 1, y + 1, z) < cut) index += 32;
                    if (GetVertexValues(x + 1, y + 1, z + 1) < cut) index += 64;
                    if (GetVertexValues(x, y + 1, z + 1) < cut) index += 128;

                    if (MarchingCubes.edgeTable[index] == 0)
                        continue;

                    Vector3[] v = new Vector3[12];
                    if (MarchingCubes.IsBitSet(MarchingCubes.edgeTable[index], 0))
                        v[0] = MarchingCubes.VertexLerp(cut, new Vector3(x, y, z), new Vector3(x + 1, y, z), GetVertexValues(x, y, z), GetVertexValues(x + 1, y, z));
                    if (MarchingCubes.IsBitSet(MarchingCubes.edgeTable[index], 1))
                        v[1] = MarchingCubes.VertexLerp(cut, new Vector3(x + 1, y, z), new Vector3(x + 1, y, z + 1), GetVertexValues(x + 1, y, z), GetVertexValues(x + 1, y, z + 1));
                    if (MarchingCubes.IsBitSet(MarchingCubes.edgeTable[index], 2))
                        v[2] = MarchingCubes.VertexLerp(cut, new Vector3(x + 1, y, z + 1), new Vector3(x, y, z + 1), GetVertexValues(x + 1, y, z + 1), GetVertexValues(x, y, z + 1));
                    if (MarchingCubes.IsBitSet(MarchingCubes.edgeTable[index], 3))
                        v[3] = MarchingCubes.VertexLerp(cut, new Vector3(x, y, z + 1), new Vector3(x, y, z), GetVertexValues(x, y, z + 1), GetVertexValues(x, y, z));
                    if (MarchingCubes.IsBitSet(MarchingCubes.edgeTable[index], 4))
                        v[4] = MarchingCubes.VertexLerp(cut, new Vector3(x, y + 1, z), new Vector3(x + 1, y + 1, z), GetVertexValues(x, y + 1, z), GetVertexValues(x + 1, y + 1, z));
                    if (MarchingCubes.IsBitSet(MarchingCubes.edgeTable[index], 5))
                        v[5] = MarchingCubes.VertexLerp(cut, new Vector3(x + 1, y + 1, z), new Vector3(x + 1, y + 1, z + 1), GetVertexValues(x + 1, y + 1, z), GetVertexValues(x + 1, y + 1, z + 1));
                    if (MarchingCubes.IsBitSet(MarchingCubes.edgeTable[index], 6))
                        v[6] = MarchingCubes.VertexLerp(cut, new Vector3(x + 1, y + 1, z + 1), new Vector3(x, y + 1, z + 1), GetVertexValues(x + 1, y + 1, z + 1), GetVertexValues(x, y + 1, z + 1));
                    if (MarchingCubes.IsBitSet(MarchingCubes.edgeTable[index], 7))
                        v[7] = MarchingCubes.VertexLerp(cut, new Vector3(x, y + 1, z + 1), new Vector3(x, y + 1, z), GetVertexValues(x, y + 1, z + 1), GetVertexValues(x, y + 1, z));
                    if (MarchingCubes.IsBitSet(MarchingCubes.edgeTable[index], 8))
                        v[8] = MarchingCubes.VertexLerp(cut, new Vector3(x, y, z), new Vector3(x, y + 1, z), GetVertexValues(x, y, z), GetVertexValues(x, y + 1, z));
                    if (MarchingCubes.IsBitSet(MarchingCubes.edgeTable[index], 9))
                        v[9] = MarchingCubes.VertexLerp(cut, new Vector3(x + 1, y, z), new Vector3(x + 1, y + 1, z), GetVertexValues(x + 1, y, z), GetVertexValues(x + 1, y + 1, z));
                    if (MarchingCubes.IsBitSet(MarchingCubes.edgeTable[index], 10))
                        v[10] = MarchingCubes.VertexLerp(cut, new Vector3(x + 1, y, z + 1), new Vector3(x + 1, y + 1, z + 1), GetVertexValues(x + 1, y, z + 1), GetVertexValues(x + 1, y + 1, z + 1));
                    if (MarchingCubes.IsBitSet(MarchingCubes.edgeTable[index], 11))
                        v[11] = MarchingCubes.VertexLerp(cut, new Vector3(x, y, z + 1), new Vector3(x, y + 1, z + 1), GetVertexValues(x, y, z + 1), GetVertexValues(x, y + 1, z + 1));

                    for (int i = 0; MarchingCubes.triTable[index, i] != -1; i += 3)
                    {
                        int a = 0, b = 0, c = 0;
                        if (vertices.Contains(v[MarchingCubes.triTable[index, i]]))
                        {
                            a = vertices.IndexOf(v[MarchingCubes.triTable[index, i]]);
                        }
                        else
                        {
                            vertices.Add(v[MarchingCubes.triTable[index, i]]);
                            normals.Add(Vector3.Cross(v[MarchingCubes.triTable[index, i + 2]] - v[MarchingCubes.triTable[index, i + 1]], v[MarchingCubes.triTable[index, i]] - v[MarchingCubes.triTable[index, i + 1]]).normalized);

                            a = vertices.Count - 1;
                        }
                        if (vertices.Contains(v[MarchingCubes.triTable[index, i + 1]]))
                        {
                            b = vertices.IndexOf(v[MarchingCubes.triTable[index, i + 1]]);
                        }
                        else
                        {
                            vertices.Add(v[MarchingCubes.triTable[index, i + 1]]);
                            normals.Add(Vector3.Cross(v[MarchingCubes.triTable[index, i + 2]] - v[MarchingCubes.triTable[index, i + 1]], v[MarchingCubes.triTable[index, i]] - v[MarchingCubes.triTable[index, i + 1]]).normalized);

                            b = vertices.Count - 1;
                        }
                        if (vertices.Contains(v[MarchingCubes.triTable[index, i + 2]]))
                        {
                            c = vertices.IndexOf(v[MarchingCubes.triTable[index, i + 2]]);
                        }
                        else
                        {
                            vertices.Add(v[MarchingCubes.triTable[index, i + 2]]);
                            normals.Add(Vector3.Cross(v[MarchingCubes.triTable[index, i + 2]] - v[MarchingCubes.triTable[index, i + 1]], v[MarchingCubes.triTable[index, i]] - v[MarchingCubes.triTable[index, i + 1]]).normalized);

                            c = vertices.Count - 1;
                        }


                        triangles.Add(a);
                        triangles.Add(b);
                        triangles.Add(c);
                    }


                }
                RenderChunk();
                yield return null;
            }
        }
        finished = true;

    }

    public void RenderChunk()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        if (finished)
            UpdateCollider();
    }
    void UpdateCollider()
    {
        mesh.RecalculateBounds();
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        
    }

}
