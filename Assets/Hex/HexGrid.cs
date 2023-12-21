using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int width = 10;
    public int length = 10;

    public int height = 10;

    public float scale = 1f;
    public int hexSize = 1;
    public int hexHeight = 3;

    public Material grassMaterial;
    public Material hexMaterial;
    public Material waterMaterial;

    void Start()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                CreateHex(x, y);
            }
        }
    }

    void CreateHex(int x, int y)
    {
        var noise = Mathf.PerlinNoise((float)x / width * scale, (float)y / length * scale);

        HexRenderer hex = new GameObject($"Hex {x}, {y}")
            .AddComponent<HexRenderer>();
        hex.size = hexSize;
        hex.height = (noise * hexHeight) + height;

        hex.GetComponent<MeshRenderer>().material = hexMaterial;

        if (noise < 0.3f)
        {
            hex.GetComponent<MeshRenderer>().material = waterMaterial;
        }
        else if (noise < 0.5f)
        {
            hex.GetComponent<MeshRenderer>().material = grassMaterial;
        }
        else
        {
            hex.GetComponent<MeshRenderer>().material.color = Color.white;
        }

        hex.transform.SetParent(transform);
        hex.transform.position = HexToWorld(x, y);

        hex.Construct();
    }

    public Vector3 HexToWorld(int x, int y)
    {
        var pos = new Vector3(x * hexSize * 1.5f, 0, y * hexSize * 1.732f);
        if (x % 2 == 1)
        {
            pos += new Vector3(0, 0, 0.866f * hexSize);
        }

        return pos;
    }
}
