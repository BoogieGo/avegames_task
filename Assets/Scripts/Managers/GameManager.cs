using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Тут у нас игровая логика
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// время игры
    /// </summary>
    public float GameTime { get; private set; }

    /// <summary>
    /// очки игрока
    /// </summary>
    public int Points { get; private set; }

    /// <summary>
    /// сферы, в данный момент живые
    /// </summary>
    public List<GameObject> Spheres { get; private set; }

    /// <summary>
    /// множитель скорости сфер, растет со временем
    /// </summary>
    public float SpeedMultiplier { get; private set; }

    /// <summary>
    /// максимальное кол-вл сфер на экране, растет со временем
    /// </summary>
    private int spheresMaxCount;

    /// <summary>
    /// сферы будут генерироваться не чаще двух раз в секуду
    /// </summary>
    private float lastGenTime;
    private float spawnDelay = .5f;

    //синглтон
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
    /// Запускаем генератор сфер
    /// </summary>
	void Start ()
	{
        Spheres = new List<GameObject>();
        Reset();
        GenerateSphere();
	}
	
    /// <summary>
    /// Считаем время, увеличиваем сложность, продолжаем генерировать сферы...
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
    /// Начиление очков за убитые сферы
    /// </summary>
    public void AddPoints(int points)
    {
        Points += points;
    }

    /// <summary>
    /// Восстанавливаем условия по умолчанию
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
    /// Создаем одну сферу
    /// </summary>
    private void GenerateSphere()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.AddComponent<BallScript>();
        Spheres.Add(go);
        lastGenTime = GameTime;
    }

    /// <summary>
    /// Возвращает время игры в строковом формате
    /// </summary>
    public string GetTime()
    {
        return (string.Format("{0:D2} : {1:D2}", (int) GameTime/60, ((int) GameTime%60)));
    }
}
