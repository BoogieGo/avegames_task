using UnityEngine;
using System.Collections;

/// <summary>
/// Здесь у нас будут лежать объекты, загруженные из бандлей
/// собственно загружаться они будут тоже тут
/// </summary>
public class LoadingManager : MonoBehaviour
{
    /// <summary>
    /// бандли лежат у меня на дропбоксе
    /// </summary>
    private string effectUrl = @"https://dl.dropboxusercontent.com/u/5503338/Bundles/smoke.unity3d";
    private string soundUrl = @"https://dl.dropboxusercontent.com/u/5503338/Bundles/sound.unity3d";

    /// <summary>
    /// Эффект, срабатывающий при взрыве шарика
    /// </summary>
    public static GameObject Smoke { get; private set; }
    /// <summary>
    /// Музыка, которая играет в игре
    /// </summary>
    public static AudioClip MarioTheme { get; private set; }
    /// <summary>
    /// Звук при взрыве шарика
    /// </summary>
    public static AudioClip PewPew { get; private set; }


    void Start()
    {
        StartCoroutine(LoadEffects());
        StartCoroutine(LoadSound());
    }
    
    /// <summary>
    /// грузим эффект
    /// </summary>
    IEnumerator LoadEffects()
    {
        WWW www = WWW.LoadFromCacheOrDownload(effectUrl, 1);
        yield return www;
        AssetBundle bundle = www.assetBundle;
        AssetBundleRequest request = bundle.LoadAsync("Smoke", typeof(GameObject));
        yield return request;
       Smoke = request.asset as GameObject;
        bundle.Unload(false);
        www.Dispose();
    }

    /// <summary>
    /// грузим музыку
    /// </summary>
    IEnumerator LoadSound()
    {
        WWW www = WWW.LoadFromCacheOrDownload(soundUrl, 1);
        yield return www;
        AssetBundle bundle = www.assetBundle;

        AssetBundleRequest request = bundle.LoadAsync("Nintendo-Super-Mario-Theme-Song", typeof(AudioClip));
        yield return request;
        MarioTheme = request.asset as AudioClip;

        request = bundle.LoadAsync("waterballoon", typeof(AudioClip));
        yield return request;
        PewPew = request.asset as AudioClip;

        bundle.Unload(false);
        www.Dispose();
    }
}
