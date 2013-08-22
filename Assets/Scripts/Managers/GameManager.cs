using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// ��� � ��� ������� ������
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// ����� ����
    /// </summary>
    public float GameTime { get; private set; }

    /// <summary>
    /// ���� ������
    /// </summary>
    public int Points { get; private set; }

    /// <summary>
    /// �����, � ������ ������ �����
    /// </summary>
    public List<GameObject> Spheres { get; private set; }

    /// <summary>
    /// ��������� �������� ����, ������ �� ��������
    /// </summary>
    public float SpeedMultiplier { get; private set; }

    /// <summary>
    /// ������������ ���-�� ���� �� ������, ������ �� ��������
    /// </summary>
    private int spheresMaxCount;

    /// <summary>
    /// ����� ����� �������������� �� ���� ���� ��� � ������
    /// </summary>
    private float lastGenTime;
    private float spawnDelay = .5f;

    //��������
    public static GameManager I 
    {
        get { return i; }
        private set { i = value; }
    }
    private static GameManager i;
    
    void Awake()
    {
        i = this;
        enabled = false;
    }

    /// <summary>
    /// ��������� ��������� ����
    /// </summary>
	void Start ()
	{
        Spheres = new List<GameObject>();
        Reset();
        GenerateSphere();
	}
	
    /// <summary>
    /// ������� �����, ����������� ���������, ���������� ������������ �����...
    /// </summary>
	void Update ()
    {
        if (!audio.isPlaying) audio.Play();

        int difficulty = (int)GameTime / 30;
        GameTime += Time.deltaTime;

        if ((int)GameTime / 30 != difficulty)
        {
            GradientGenerator.GenereteTextures();
            spheresMaxCount++;
            SpeedMultiplier += 1/3f;
        }
        
        if (GameTime - lastGenTime > spawnDelay && Spheres.Count < spheresMaxCount)
            GenerateSphere();
	}

    /// <summary>
    /// ��������� ����� �� ������ �����
    /// </summary>
    public void AddPoints(int points)
    {
        Points += points;
    }

    /// <summary>
    /// ��������������� ������� �� ���������
    /// </summary>
    public void Reset()
    {
        foreach (GameObject sphere in Spheres)
        {
            Destroy(sphere);
        }
        Spheres.Clear();

        audio.clip = LoadingManager.MarioTheme;
        audio.Stop();

        GradientGenerator.GenereteTextures();

        lastGenTime = 0f;
        GameTime = 0f;
        Points = 0;
        SpeedMultiplier = 1f;
        spheresMaxCount = 5;
    }

    /// <summary>
    /// ������� ���� �����
    /// </summary>
    private void GenerateSphere()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.AddComponent<BallScript>();
        Spheres.Add(go);
        lastGenTime = GameTime;
    }

    /// <summary>
    /// ���������� ����� ���� � ��������� �������
    /// </summary>
    public string GetTime()
    {
        return (string.Format("{0:D2} : {1:D2}", (int) GameTime/60, ((int) GameTime%60)));
    }
}
