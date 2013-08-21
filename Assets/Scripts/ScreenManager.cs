using UnityEngine;
using System.Collections;

public enum GameState : byte
{
    Menu,
    Game,
    Pause,
    BestResult,
    Author,
    Loading
}

public class ScreenManager : MonoBehaviour
{
    private GameState state;

    void Awake()
    {
        state = GameState.Game;
    }

    void Update()
    {

    }

    void OnGUI()
    {
        switch (state)
        {
            case GameState.Loading:

                break;

            case GameState.Menu:
                GUILayout.BeginArea(new Rect(Screen.width*.35f, Screen.height*.35f, Screen.width * .3f, Screen.height*.3f));
                if (GUILayout.Button("Play", GUILayout.Height(Screen.height * .095f))) { state = GameState.Game;}
                if (GUILayout.Button("Best Result", GUILayout.Height(Screen.height * .095f))) { state = GameState.BestResult; }
                if (GUILayout.Button("Author", GUILayout.Height(Screen.height * .095f))) { state = GameState.Author; }
                GUILayout.EndArea();
                break;

            case GameState.Game:
                GUI.skin.box.alignment = TextAnchor.MiddleCenter;
                GUI.Box(new Rect(0, Screen.height - 80, 125, 40), "Points\n" + GameManager.I.Points);
                GUI.Box(new Rect(0, Screen.height - 40, 125, 40), "Time\n" + GameManager.I.GetTime());
                break;
        }
    }
}