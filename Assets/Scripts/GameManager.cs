using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public float GameTime;
    public int Points;
    public List<GameObject> Spheres;

    public GameObject Smoke;

    public float SpeedMultiplier;

    private int spheresMaxCount;
    private float lastGenTime;

    private float spawnDelay = .75f;

    public static GameManager I 
    {
        get { return i; }
        private set { i = value; }
    }
    private static GameManager i;
    
    void Awake()
    {
        i = this;
    }

	void Start ()
	{
        GradientGenerator.GenereteTextures();

        Spheres = new List<GameObject>();
        GameTime = 0f;
	    Points = 0;

	    SpeedMultiplier = 1f;

	    spheresMaxCount = 5;

        GenerateSphere();
	}
	
	void Update ()
	{
        GameTime += Time.deltaTime;

        if (GameTime > 30f && GameTime - Time.deltaTime < 30f)
        {
            GradientGenerator.GenereteTextures();
            spheresMaxCount = 6; 
            SpeedMultiplier = 4 / 3f;
        }
        else if (GameTime > 60f && GameTime - Time.deltaTime < 60f)
        {
            GradientGenerator.GenereteTextures();
            spheresMaxCount = 7; 
            SpeedMultiplier = 5 / 3f;
        }
        else if (GameTime > 90f && GameTime - Time.deltaTime < 90f)
        {
            GradientGenerator.GenereteTextures();
            spheresMaxCount = 8;
            SpeedMultiplier = 2f;
        }
        
        if (GameTime - lastGenTime > spawnDelay && Spheres.Count < spheresMaxCount)
            GenerateSphere();
	}

    public void AddPoints(int points)
    {
        Points += points;
    }

    private void GenerateSphere()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.AddComponent<BallScript>();
        Spheres.Add(go);
        lastGenTime = GameTime;
    }

    public string GetTime()
    {
        return (string.Format("{0} : {1:D2}", (int) GameTime/60, ((int) GameTime%60)));
    }
}
