using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum FaceType
{
    Top,
    Bottom,
    Side
}

public struct Face
{
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector2> uvs;

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    Mesh hexMesh;

    List<Face> faces = new List<Face>();

    public int size = 1;
    public float height = 3;
    public bool isPointy = true;

    void Awake()
    {
        hexMesh = new Mesh
        {
            name = "Hex Mesh"
        };

        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = hexMesh;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Construct() {
        ConstructFaces();
        FormMesh();
    }

    Vector3 GetPointyHexCorner(Vector3 center, int size, int i)
    {
        float angleDeg = 60 * i;
        float angleRad = Mathf.PI / 180 * angleDeg;
        return new Vector3(center.x + size * Mathf.Cos(angleRad), center.y, center.z + size * Mathf.Sin(angleRad));
    }

    Vector3 GetFlatHexCorner(Vector3 center, int size, int i)
    {
        float angleDeg = 60 * i;
        float angleRad = Mathf.PI / 180 * angleDeg;
        return new Vector3(center.x + size * Mathf.Sin(angleRad), center.y, center.z + size * Mathf.Cos(angleRad));
    }

    Face CreateFace(Vector3 center, int size, int i, FaceType faceType, bool isPointy = true)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uvs = new List<Vector2>();

        Vector3 corner1 = isPointy ? GetPointyHexCorner(center, size, i) : GetFlatHexCorner(center, size, i);
        Vector3 corner2 = isPointy ? GetPointyHexCorner(center, size, i + 1) : GetFlatHexCorner(center, size, i + 1);

        if (faceType == FaceType.Top)
        {
            vertices.Add(center);
            vertices.Add(corner1);
            vertices.Add(corner2);

            triangles.Add(0);
            triangles.Add(2);
            triangles.Add(1);

            uvs.Add(new Vector2(0.5f, 0.5f));
            uvs.Add(new Vector2(0.5f + 0.5f * Mathf.Cos(Mathf.PI / 180 * 60 * i), 0.5f + 0.5f * Mathf.Sin(Mathf.PI / 180 * 60 * i)));
            uvs.Add(new Vector2(0.5f + 0.5f * Mathf.Cos(Mathf.PI / 180 * 60 * (i + 1)), 0.5f + 0.5f * Mathf.Sin(Mathf.PI / 180 * 60 * (i + 1))));
        }
        else if (faceType == FaceType.Bottom)
        {
            vertices.Add(center);
            vertices.Add(corner2);
            vertices.Add(corner1);

            triangles.Add(0);
            triangles.Add(2);
            triangles.Add(1);

            uvs.Add(new Vector2(0.5f, 0.5f));
            uvs.Add(new Vector2(0.5f + 0.5f * Mathf.Cos(Mathf.PI / 180 * 60 * (i + 1)), 0.5f + 0.5f * Mathf.Sin(Mathf.PI / 180 * 60 * (i + 1))));
            uvs.Add(new Vector2(0.5f + 0.5f * Mathf.Cos(Mathf.PI / 180 * 60 * i), 0.5f + 0.5f * Mathf.Sin(Mathf.PI / 180 * 60 * i)));
        }
        else if (faceType == FaceType.Side)
        {
            Vector3 bottomCenter = new Vector3(center.x, 0, center.z);
            Vector3 bottomCorner1 = isPointy ? GetPointyHexCorner(bottomCenter, size, i) : GetFlatHexCorner(bottomCenter, size, i);
            Vector3 bottomCorner2 = isPointy ? GetPointyHexCorner(bottomCenter, size, i + 1) : GetFlatHexCorner(bottomCenter, size, i + 1);

            vertices.Add(corner1);
            vertices.Add(bottomCorner1);
            vertices.Add(bottomCorner2);
            vertices.Add(corner2);

            triangles.Add(0);
            triangles.Add(2);
            triangles.Add(1);
            triangles.Add(0);
            triangles.Add(3);
            triangles.Add(2);


            uvs.Add(new Vector2(i / 6.0f, 1));
            uvs.Add(new Vector2(i / 6.0f, 0));
            uvs.Add(new Vector2((i + 1) / 6.0f, 0));
            uvs.Add(new Vector2((i + 1) / 6.0f, 1));
        }

        return new Face(vertices, triangles, uvs);
    }

    void ConstructFaces()
    {
        // top faces
        for (int i = 0; i < 6; i++)
        {
            faces.Add(CreateFace(new Vector3(0, height, 0), size, i, FaceType.Top, true));
        }

        // bottom faces
        for (int i = 0; i < 6; i++)
        {
            faces.Add(CreateFace(new Vector3(0, 0, 0), size, i, FaceType.Bottom, true));
        }

        // side faces
        for (int i = 0; i < 6; i++)
        {
            faces.Add(CreateFace(new Vector3(0, height, 0), size, i, FaceType.Side, true));
        }
    }

    void FormMesh()
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uvs = new List<Vector2>();

        foreach (var face in faces)
        {
            vertices.AddRange(face.vertices);
            uvs.AddRange(face.uvs);

            int offset = vertices.Count - face.vertices.Count;
            foreach (var triangle in face.triangles)
            {
                triangles.Add(triangle + offset);
            }
        }

        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.uv = uvs.ToArray();
        hexMesh.RecalculateNormals();
    }
}
