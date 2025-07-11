using System.Collections.Generic;
using UnityEngine;

namespace Boxing
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        private Queue<string> logs = new();
        private const int maxLogs = 10;
        private Vector2 scrollPos;

        private PlayerController player1;
        private PlayerController player2;

        private bool showWinUI;
        private bool showLoseUI;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        public void SetPlayers(PlayerController p1, PlayerController p2)
        {
            player1 = p1;
            player2 = p2;
        }

        public void Log(string message)
        {
            if (logs.Count >= maxLogs)
                logs.Dequeue();
            logs.Enqueue(message);
        }

        public void ShowWinUI() => showWinUI = true;
        public void ShowLoseUI() => showLoseUI = true;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 500, 300), GUI.skin.box);

            if (player1 != null && player2 != null)
            {
                GUILayout.Label($"<b>Player 1</b> HP: {player1.HP} / {player1.MaxHP}   |   Mana: {player1.Mana} / {player1.MaxMana}");
                GUILayout.Label($"<b>Player 2</b> HP: {player2.HP} / {player2.MaxHP}   |   Mana: {player2.Mana} / {player2.MaxMana}");
                GUILayout.Space(10);
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (var log in logs)
                GUILayout.Label(log);
            GUILayout.EndScrollView();

            GUILayout.EndArea();

            if (showWinUI)
            {
                GUILayout.BeginArea(new Rect(Screen.width / 2f - 100, Screen.height / 2f - 60, 200, 120), GUI.skin.box);
                GUILayout.Label("<b>YOU WIN!</b>");
                if (GUILayout.Button("Next Round"))
                {
                    showWinUI = false;
                    GameManager.Instance?.NextRound();
                }
                GUILayout.EndArea();
            }

            if (showLoseUI)
            {
                GUILayout.BeginArea(new Rect(Screen.width / 2f - 100, Screen.height / 2f - 60, 200, 120), GUI.skin.box);
                GUILayout.Label("<b>YOU LOSE!</b>");
                if (GUILayout.Button("Play Again"))
                {
                    showLoseUI = false;
                    GameManager.Instance?.StartGame();
                }
                GUILayout.EndArea();
            }
        }
    }
}
