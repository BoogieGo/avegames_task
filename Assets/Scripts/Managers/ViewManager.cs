using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

/// <summary>
/// Отвечает за создание\уничтожение "сетевых" шаров
/// </summary>
public class ViewManager : MonoBehaviour
{
    private List<RemoteBall> remoteBalls = new List<RemoteBall>();
    public float GameTime = 0;
    public int Points = 0;

    //синглтон
    public static ViewManager I { get { return i; } set { i = value; } }
    private static ViewManager i;

    void Awake()
    {
        i = this;
        enabled = false;
    }
    
    /// <summary>
    /// Удаление "сетевого" шара. Если новые очки больше наших, значит шар убит игроком - рисуем эффект.
    /// </summary>
    public void DestroyOne(int id, int points)
    {
        RemoteBall rBall = remoteBalls.Find(ball => ball.Id == id);
        if (points > Points)
        {
            Points = points;
            if (rBall != null)
                Instantiate(LoadingManager.Smoke, rBall.gameObject.transform.position + new Vector3(0, 0, 1), Quaternion.identity);
        }
        if (rBall != null)
            Destroy(rBall.gameObject);
    }

    /// <summary>
    /// Создание нового "сетевого" шара
    /// </summary>
    public void CreateOne(int id, int texId, float diam, float speed, float x)
    {
        RemoteBall rb = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<RemoteBall>();
        rb.Id = id;
        rb.TexId = texId;
        rb.Diameter = diam;
        rb.Speed = speed;
        rb.gameObject.transform.position = new Vector3(x, 10f, 0f);
        remoteBalls.Add(rb);
    }
    
    /// <summary>
    /// Чистит мусор когда мы выходим из игры
    /// </summary>
    public void Reset()
    {
        foreach (RemoteBall ball in remoteBalls)
        {
            Destroy(ball);
        }
        remoteBalls.Clear();
    }

    public string GetTime()
    {
        return (string.Format("{0:D2} : {1:D2}", (int)GameTime / 60, ((int)GameTime % 60)));
    }

    void OnDisable()
    {
        Reset();
    }
}
