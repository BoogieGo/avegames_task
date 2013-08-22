using UnityEngine;
using System.Collections;

/// <summary>
/// ����������� ����� ��� �������� ����������� �������
/// </summary>
public static class GradientGenerator
{
    /// <summary>
    /// �������� ������������ ��������
    /// </summary>
    public static Texture[] Textures = new Texture[8];

    /// <summary>
    /// ��������, ������� �������� �� ����(�������� ���������� ���� ���, � ������� ������������ ��� �����)
    /// </summary>
    public static Texture[] RemouteTextures = new [] { 
        new Texture(), new Texture(), new Texture(), new Texture(), 
        new Texture(), new Texture(), new Texture(), new Texture()};
    
    /// <summary>
    /// �������������� ��������
    /// </summary>
    public static void GenereteTextures()
    {
        Textures = new[]
                        {
                            Generate(32, new Color().Rand(), new Color().Rand()), 
                            Generate(32, new Color().Rand(), new Color().Rand()),
                            Generate(64, new Color().Rand(), new Color().Rand()), 
                            Generate(64, new Color().Rand(), new Color().Rand()), 
                            Generate(128, new Color().Rand(), new Color().Rand()), 
                            Generate(128, new Color().Rand(), new Color().Rand()),
                            Generate(256, new Color().Rand(), new Color().Rand()), 
                            Generate(256, new Color().Rand(), new Color().Rand())
                        };
        
        SendTextures();
        System.GC.Collect();
    }

    /// <summary>
    /// �������� ���������� � ����� ��������� �������������
    /// </summary>
    public static void SendTextures()
    {
        for (int i = 0; i < Textures.Length; i++)
        {
            Color one = (Textures[i] as Texture2D).GetPixel(0, Textures[i].height - 1);
            Color two = (Textures[i] as Texture2D).GetPixel(Textures[i].width - 1, 0);
            NetworkManager.I.ToRemote.Enqueue("texture " + i + " " +
                                              one.r + " " + one.g + " " + one.b + " " + two.r + " " + two.g + " " +
                                              two.b);

        }
    }

    /// <summary>
    /// ���������� ��������� ����������� ��������
    /// </summary>
    public static Texture Generate(int size, Color one, Color two)
    {
        Texture2D texture = new Texture2D(size, size);

        //Color one = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        //Color two = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        for (int i = 0; i < texture.width / 2; i++)
            for (int j = 0; j < texture.height; j++)
                texture.SetPixel(i, j, Color.Lerp(one, two, (float)i / j));

        for (int i = texture.width/2; i < texture.width; i++)
            for (int j = 0; j < texture.height; j++)
                texture.SetPixel(i, j, Color.Lerp(two, one, (float)j / i));

        texture.Apply();
        
        return texture;
    }

    /// <summary>
    /// ���������� �������� ������� ������� � ����������� �� �������� ����
    /// </summary>
    public static int GetTexture(float diameter)
    {
        return diameter <= .75f
                   ? Random.Range(0, 2)
                   : diameter <= 1f
                         ? Random.Range(2, 4)
                         : diameter <= 1.25f
                               ? Random.Range(4, 6)
                               : Random.Range(6, 8);
    }
}

/// <summary>
/// ����� ����� ���������� ��� ��������� ��������� ������
/// </summary>
public static class RandomColor
{
    public static Color Rand(this Color c)
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
}
