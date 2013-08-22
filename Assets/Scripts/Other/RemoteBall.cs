using UnityEngine;
using System.Collections;

/// <summary>
/// Тут логика шарика, который инстансируется значениями, полученными из сети
/// </summary>
public class RemoteBall : MonoBehaviour
{
    public int Id { get; set; }

    public int TexId
    {
        get { return texId; }
        set { texId = value; if (GradientGenerator.RemouteTextures[texId] != null)
            renderer.material.mainTexture = GradientGenerator.RemouteTextures[texId];
        }
    }
    private int texId;

    public float Diameter
    {
        get { return diameter; } 
        set { diameter = value;  transform.localScale = new Vector3(diameter, diameter, diameter);}
    }
    private float diameter;

    public float Speed { get; set; }
    
    void Update()
    {
        transform.Translate(new Vector3(0, -Speed, 0));
    }
}
