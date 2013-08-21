using UnityEngine;
using System.Collections;

public enum TexSize : byte
{
    S,
    M,
    L,
    XL
}

public static class GradientGenerator
{
    private static Texture[] _32;
    private static Texture[] _64;
    private static Texture[] _128;
    private static Texture[] _256;

    public static void GenereteTextures()
    {
        _32 = new [] { Generate(32), Generate(32) };
        _64 = new [] { Generate(64), Generate(64) };
        _128 = new [] { Generate(128), Generate(128) };
        _256 = new [] { Generate(256), Generate(256) };
        System.GC.Collect();
    }

    private static Texture Generate(int size)
    {
        Texture2D texture = new Texture2D(size, size);

        Color one = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        Color two = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        for (int i = 0; i < texture.width / 2; i++)
            for (int j = 0; j < texture.height; j++)
                texture.SetPixel(i, j, Color.Lerp(one, two, (float)i / j));

        for (int i = texture.width/2; i < texture.width; i++)
            for (int j = 0; j < texture.height; j++)
                texture.SetPixel(i, j, Color.Lerp(two, one, (float)j / i));

        texture.Apply();
        
        return texture;
    }

    public static Texture GetTexture(TexSize size)
    {
        int rnd = Random.Range(0, 2);

        switch (size)
        {
             case TexSize.S:
                return _32[rnd];
             case TexSize.M:
                return _64[rnd];
             case TexSize.L:
                return _128[rnd];
             default:
                return _256[rnd];
        }
    }
}
