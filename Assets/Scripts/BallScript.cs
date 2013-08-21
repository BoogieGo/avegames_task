using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour
{
    public float Diameter { get; private set; }
    public float Speed { get; private set; }
    public int Points { get; private set; }
    
    void Awake()
    {
        Diameter = Random.Range(.5f, 1.5f);
        Speed = .05f/Diameter * GameManager.I.SpeedMultiplier;
        Points = (int)(Speed*10);

        transform.localScale = new Vector3(Diameter, Diameter, Diameter);
        transform.position = new Vector3(Random.Range(-12f, 12f), 10f, 0f);

        TexSize size = Diameter <= .75f
                           ? TexSize.S
                           : Diameter <= 1f
                                 ? TexSize.M
                                 : Diameter <= 1.25f
                                       ? TexSize.L
                                       : TexSize.XL;
        renderer.material.mainTexture = GradientGenerator.GetTexture(size);
    }

	void Start () 
    {
	    
	}
	
	void Update ()
    {
	    transform.Translate(0, -Speed, 0);

        if (transform.position.y < -10f)
        {
            DestroyMe();
        }
	}

    void OnMouseDown()
    {
        DestroyMe(true);
    }

    void DestroyMe(bool byPlayer = false)
    {
        if (byPlayer)
        {
            Instantiate(GameManager.I.Smoke, transform.position, Quaternion.identity);
            GameManager.I.AddPoints((int)(10 / Diameter));
        }
        GameManager.I.Spheres.Remove(gameObject);
        Destroy(gameObject);
    }
}
