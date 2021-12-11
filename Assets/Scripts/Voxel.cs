using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    private Texture2D tex;
    private Sprite mySprite;
    public SpriteRenderer sr;
    Vector2Int texSize;
    public Color mainColor;

    [SerializeField]
    public GeneratorData data;

    public float perlinVar = 1024f;
    public float perlinTreshold = -0.06f;

    public Texture2D baseTex;

    void Awake()
    {
        sr.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        transform.position = new Vector3(1.5f, 1.5f, 0.0f);

        data.scale = 1;
        data.octaves = 3;
        data.persistance = 6.9f;
        data.lacunarity = 46.99f;

        texSize = new Vector2Int(32, 32);
    }

    void Start()
    {
        tex = new Texture2D(texSize.x, texSize.y);
        tex.filterMode = FilterMode.Point;
        GetComponent<Renderer>().material.mainTexture = tex;

        FillTexture(tex);


        mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

        sr.sprite = mySprite;
    }

    void Update()
    {
        FillTexture(tex);
    }

    void FillTexture(Texture2D texture)
    {
        float[][] perlinValues = PerlinGenerator.Generate2D(data, transform.position.x, transform.position.y, texSize.x, texSize.y, perlinVar);

        bool[] colsToFill = new bool[perlinValues[0].Length];
        //for (int y = 0; y < texture.height; ++y) for (int x = 0; x < texture.width; ++x)
        //    {
        //        Color color = new Color(0, 0, 0, 0);
        //        if (perlinValues[y][x] >= perlinTreshold || colsToFill[x])
        //        {
        //            color = mainColor;
        //            colsToFill[x] = true;
        //        }
        //        texture.SetPixel(x, texSize.y - 1 - y, color);
        //    }

        for (int x = 0; x < tex.width; ++x)
        {
            int missed = 0;
            for (int y = tex.height - 1; y >= 0; --y)
            {
                if (perlinValues[y][x] >= perlinTreshold || colsToFill[x])
                {
                    colsToFill[x] = true;
                    texture.SetPixel(x, y, baseTex.GetPixel(x, y + missed));
                }
                else
                {
                    texture.SetPixel(x, y, new Color());
                    ++missed;
                }
            }
        }

        texture.Apply();
    }
}
