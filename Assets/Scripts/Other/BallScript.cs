using UnityEngine;
using System.Collections;

/// <summary>
/// ������ ������ (����������)
/// </summary>
public class BallScript : MonoBehaviour
{
    public float Diameter { get; private set; }
    public float Speed { get; private set; }
    public int Points { get; private set; }

    /// <summary>
    /// id ����� ��� ����� �������� ���������� ��������
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// ������ �������� � ������� ������ ����������
    /// </summary>
    public int TexId { get; private set; }

    public static int IdCounter;
    
    /// <summary>
    /// ��������� id, �������� ������ ��������� ����, ���� �� ���� ���-�� ��������� - ���������� ��������� � �������� ����
    /// </summary>
    void Awake()
    {
        Id = IdCounter++;
        Diameter = Random.Range(.5f, 1.5f);
        Speed = .05f/Diameter * GameManager.I.SpeedMultiplier;
        Points = (int)(Speed*10);

        transform.localScale = new Vector3(Diameter, Diameter, Diameter);
        transform.position = new Vector3(Random.Range(-12f, 12f), 10f, 0f);
        
        TexId = GradientGenerator.GetTexture(Diameter);
        renderer.material.mainTexture = GradientGenerator.Textures[TexId];
        
        NetworkManager.I.ToRemote.Enqueue("create "+Id+" "+TexId+" "+Diameter+" "+Speed+" "+transform.position.x);
    }
    
    /// <summary>
    /// ������ ����, ���� ����� ������� ����� - ������������
    /// </summary>
	void Update ()
    {
	    transform.Translate(0, -Speed, 0);

        if (transform.position.y < -10f)
        {
            DestroyMe();
        }
	}

    /// <summary>
    /// ���� ����� ������� �� ����� ���� - ������������ � �������� � ������
    /// </summary>
    void OnMouseDown()
    {
        DestroyMe(true);
    }

    /// <summary>
    /// ������� �����
    /// </summary>
    /// <param name="byPlayer">true ���� ����� ���� �������</param>
    void DestroyMe(bool byPlayer = false)
    {
        if (byPlayer)
        {
            Instantiate(LoadingManager.Smoke, transform.position + new Vector3(0, 0, 1), Quaternion.identity);
            GameManager.I.AddPoints((int)(10 / Diameter));
        }
        NetworkManager.I.ToRemote.Enqueue("destroy "+Id+" "+GameManager.I.Points);
        GameManager.I.Spheres.Remove(gameObject);
        Destroy(gameObject);
    }
}
