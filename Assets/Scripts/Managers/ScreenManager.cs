using System.Net;
using UnityEngine;
using System.Collections;

public enum GameState : byte
{
    Menu,
    Game,
    Connect,
    Author,
    Loading
}

/// <summary>
/// Это Скрин Менеджер
/// </summary>
public class ScreenManager : MonoBehaviour
{
    private GameState state;

    /// <summary>
    /// Это мой ip
    /// </summary>
    private string ip = "192.168.0.104";
    private string port = "8001";

    /// <summary>
    /// если true, то в игре к нам будет приходить сетевая инфа от нас же
    /// и можно представить, что видять удаленные пользователи, наблюдая за нашей игрой
    /// </summary>
    private bool isLocalTest = true;

    /// <summary>
    /// В начале ждем пока загрузятся бандли
    /// </summary>
    void Awake()
    {
        state = GameState.Loading;
    }
    
    void Update()
    {
        if (state == GameState.Loading && LoadingManager.Smoke != null
            && LoadingManager.MarioTheme != null && LoadingManager.PewPew != null)
        {
            LoadingManager.Smoke.AddComponent<AudioSource>();
            LoadingManager.Smoke.audio.clip = LoadingManager.PewPew;
            LoadingManager.Smoke.audio.playOnAwake = true;
            LoadingManager.Smoke.audio.minDistance = 300;

            state = GameState.Menu;
        }
    }

    /// <summary>
    /// Собственно рисуем gui
    /// </summary>
    void OnGUI()
    {
        GUI.skin.box.alignment = TextAnchor.MiddleCenter;

        switch (state)
        {
            // Загрузка бандлей
            case GameState.Loading:
                GUI.Box(new Rect(Screen.width * .4f, Screen.height * .45f, Screen.width * .2f, Screen.height * .1f), 
                    "Loading resources...");
                break;

            // Главное меню
            case GameState.Menu:
                GUI.Label(new Rect(0, 0, 200, 200), "Если кнопка IsLocalTest включена, то на игровом экране будут дублироваться " + 
                    "шары значениями, приходящими по сети от нас же, так же будут переназначаться текстуры и пересчитываться игровые очки.");
                
                if (!isLocalTest)
                {
                    GUI.Label(new Rect(Screen.width * .66f, Screen.height * .45f, Screen.width * .1f, Screen.height * .05f), "   IP:");
                    GUI.Label(new Rect(Screen.width * .66f, Screen.height * .5f, Screen.width * .1f, Screen.height * .05f), "Port:");
                    ip = GUI.TextField(new Rect(Screen.width*.72f, Screen.height*.45f, Screen.width*.2f, Screen.height*.05f), ip);
                    port = GUI.TextField(new Rect(Screen.width*.72f, Screen.height*.5f, Screen.width*.2f, Screen.height*.05f), port);
                }
                isLocalTest = GUI.Toggle(new Rect(Screen.width*.72f, Screen.height*.55f, Screen.width*.2f, Screen.height*.05f),
                            isLocalTest, "IsLocalTest");
                GUILayout.BeginArea(new Rect(Screen.width*.35f, Screen.height*.35f, Screen.width * .3f, Screen.height*.3f));
                if (GUILayout.Button("Play", GUILayout.Height(Screen.height * .095f)))
                {
                    NetworkManager.I.StartTcpServer();
                    if (isLocalTest)
                    {
                        new System.Threading.Thread(() =>
                                                        {
                                                            System.Threading.Thread.Sleep(1000);
                                                            NetworkManager.I.TryConnect(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString(), "8001");
                                                        }).Start();
                        ViewManager.I.enabled = true;
                    }
                    GameManager.I.enabled = true;
                    state = GameState.Game;
                }
                if (!isLocalTest)
                {
                    if (GUILayout.Button("Connect", GUILayout.Height(Screen.height*.095f)))
                    {
                        state = GameState.Connect;
                        ViewManager.I.enabled = true;
                        new System.Threading.Thread(() => NetworkManager.I.TryConnect(ip, port)).Start();
                        NetworkManager.I.StartTcpServer();
                    }
                }
                else
                {
                    GUILayout.Space(Screen.height * .01f);
                }
                if (GUILayout.Button("Author", GUILayout.Height(Screen.height * .095f))) 
                { 
                    state = GameState.Author; 
                }
                GUILayout.EndArea();
                break;

            // Игра
            case GameState.Game:
                if (isLocalTest)
                {
                    GUI.Box(new Rect(0, Screen.height - 120, 125, 40), "Remoute Scores\n" + ViewManager.I.Points);
                }
                GUI.Box(new Rect(0, Screen.height - 80, 125, 40), "Scores\n" + GameManager.I.Points);
                GUI.Box(new Rect(0, Screen.height - 40, 125, 40), "Time\n" + GameManager.I.GetTime());
                if (GUI.Button(new Rect(Screen.width * .9f, Screen.height* .9f, Screen.width * .1f, Screen.height* .1f), "Back"))
                {
                    if (NetworkManager.I.Sock != null && NetworkManager.I.Sock.Connected)
                        NetworkManager.I.Sock.Disconnect(false);
                    if (NetworkManager.I.Listener != null)
                        NetworkManager.I.Listener.Stop();
                    if (isLocalTest)
                    {
                        ViewManager.I.Reset();
                        ViewManager.I.enabled = false;
                    }
                    GameManager.I.enabled = false;
                    GameManager.I.Reset();
                    state = GameState.Menu;
                }
                break;

            // Наблюдение
            case GameState.Connect:
                GUI.Box(new Rect(0, Screen.height - 80, 125, 40), "Scores\n" + ViewManager.I.Points);
                GUI.Box(new Rect(0, Screen.height - 40, 125, 40), "Time\n" + ViewManager.I.GetTime());
                if (GUI.Button(new Rect(Screen.width * .9f, Screen.height* .9f, Screen.width * .1f, Screen.height* .1f), "Back"))
                {
                    if (NetworkManager.I.Sock != null && NetworkManager.I.Sock.Connected)
                        NetworkManager.I.Sock.Disconnect(false);
                    if (NetworkManager.I.Listener != null)
                        NetworkManager.I.Listener.Stop();
                    ViewManager.I.enabled = false;
                    ViewManager.I.Reset();
                    state = GameState.Menu;
                }
                break;

            // About
            case GameState.Author:
                GUI.Box(new Rect(Screen.width*.3f, Screen.height*.4f, Screen.width*.4f, Screen.height*.2f), 
                    "Author: Andrey Stupak\nstupak.andr@gmail.com\nspecial for Avenue Games©\nenjoy ;)");
                if (GUI.Button(new Rect(Screen.width * .4f, Screen.height * .6f, Screen.width * .2f, Screen.height * .05f), "Back"))
                    state = GameState.Menu;
                break;
        }
    }
}