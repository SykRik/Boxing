using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Boxing
{
    public class UIManager : MonoBehaviour
    {
        [Serializable]
        public class UIProfile
        {
            public GameObject root;
            public PlayerController player;
            public TextMeshProUGUI Name;
            public Slider Health;
            public Slider Power;
        }

        public static UIManager Instance;

        [SerializeField] private Button gameMode1vs1;
        [SerializeField] private Button gameMode1vs2;
        [SerializeField] private Button gameMode2vs2;
        [SerializeField] private Button gameMode25vs25;
        [SerializeField] private Button nextLevel;
        [SerializeField] private Button playAgain;
        [SerializeField] private Button QuitGame;

        [SerializeField] private UIProfile teamA_Player1;
        [SerializeField] private UIProfile teamA_Player2;
        [SerializeField] private UIProfile teamB_Player1;
        [SerializeField] private UIProfile teamB_Player2;

        [SerializeField] private GameObject HUD;
        [SerializeField] private GameObject Home;
        [SerializeField] private GameObject Result;

        [SerializeField] private TextMeshProUGUI resultText;

        private bool showWinUI;
        private bool showLoseUI;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (gameMode1vs1 != null)
                gameMode1vs1.onClick.AddListener(() => SetGameModeAndStart(GameMode.OneVsOne));
            if (gameMode1vs2 != null)
                gameMode1vs2.onClick.AddListener(() => SetGameModeAndStart(GameMode.OneVsTwo));
            if (gameMode2vs2 != null)
                gameMode2vs2.onClick.AddListener(() => SetGameModeAndStart(GameMode.TwoVsTwo));
            if (gameMode25vs25 != null)
                gameMode25vs25.onClick.AddListener(() => SetGameModeAndStart(GameMode.StressTest25));
            if (nextLevel != null)
                nextLevel.onClick.AddListener(OnNextLevelClicked);
            if (playAgain != null)
                playAgain.onClick.AddListener(OnPlayAgainClicked);
            if (QuitGame != null)
                QuitGame.onClick.AddListener(OnQuitGameClicked);

            InitializeUIProfiles();
            ShowHome();
        }

        private void SetGameModeAndStart(GameMode mode)
        {
            GameManager.Instance?.SetGameMode(mode);
            GameManager.Instance?.StartGame();
            showWinUI = false;
            showLoseUI = false;
            InitializeUIProfiles();
            ShowHUD();
        }

        public void SetPlayers(PlayerController p1A, PlayerController p2A, PlayerController p1B, PlayerController p2B)
        {
            if (teamA_Player1 != null)
            {
                teamA_Player1.player = p1A;
                teamA_Player1.root?.SetActive(p1A != null);
            }
            if (teamA_Player2 != null)
            {
                teamA_Player2.player = p2A;
                teamA_Player2.root?.SetActive(p2A != null);
            }
            if (teamB_Player1 != null)
            {
                teamB_Player1.player = p1B;
                teamB_Player1.root?.SetActive(p1B != null);
            }
            if (teamB_Player2 != null)
            {
                teamB_Player2.player = p2B;
                teamB_Player2.root?.SetActive(p2B != null);
            }
            UpdateUIProfiles();
        }

        public void SetPlayers(List<PlayerController> teamA, List<PlayerController> teamB)
        {
            if (teamA_Player1 != null)
            {
                teamA_Player1.player = teamA.Count > 0 ? teamA[0] : null;
                teamA_Player1.root?.SetActive(teamA.Count > 0);
            }
            if (teamA_Player2 != null)
            {
                teamA_Player2.player = teamA.Count > 1 ? teamA[1] : null;
                teamA_Player2.root?.SetActive(teamA.Count > 1);
            }
            if (teamB_Player1 != null)
            {
                teamB_Player1.player = teamB.Count > 0 ? teamB[0] : null;
                teamB_Player1.root?.SetActive(teamB.Count > 0);
            }
            if (teamB_Player2 != null)
            {
                teamB_Player2.player = teamB.Count > 1 ? teamB[1] : null;
                teamB_Player2.root?.SetActive(teamB.Count > 1);
            }
            UpdateUIProfiles();
        }

        private void InitializeUIProfiles()
        {
            if (teamA_Player1 != null && teamA_Player1.Health != null)
            {
                teamA_Player1.Health.minValue = 0;
                teamA_Player1.Power.minValue = 0;
            }
            if (teamA_Player2 != null && teamA_Player2.Health != null)
            {
                teamA_Player2.Health.minValue = 0;
                teamA_Player2.Power.minValue = 0;
            }
            if (teamB_Player1 != null && teamB_Player1.Health != null)
            {
                teamB_Player1.Health.minValue = 0;
                teamB_Player1.Power.minValue = 0;
            }
            if (teamB_Player2 != null && teamB_Player2.Health != null)
            {
                teamB_Player2.Health.minValue = 0;
                teamB_Player2.Power.minValue = 0;
            }
            UpdateUIProfiles();
        }

        private void UpdateUIProfiles()
        {
            if (teamA_Player1 != null && teamA_Player1.player != null)
            {
                if (teamA_Player1.Name != null)
                    teamA_Player1.Name.text = teamA_Player1.player.PlayerName ?? "Player 1";
                if (teamA_Player1.Health != null)
                {
                    teamA_Player1.Health.maxValue = teamA_Player1.player.MaxHP;
                    teamA_Player1.Health.value = teamA_Player1.player.HP;
                }
                if (teamA_Player1.Power != null)
                {
                    teamA_Player1.Power.maxValue = teamA_Player1.player.MaxMana;
                    teamA_Player1.Power.value = teamA_Player1.player.Mana;
                }
            }
            else if (teamA_Player1 != null)
            {
                if (teamA_Player1.Name != null) teamA_Player1.Name.text = "";
                if (teamA_Player1.Health != null) teamA_Player1.Health.value = 0;
                if (teamA_Player1.Power != null) teamA_Player1.Power.value = 0;
                if (teamA_Player1.root != null) teamA_Player1.root.SetActive(false);
            }

            if (teamA_Player2 != null && teamA_Player2.player != null)
            {
                if (teamA_Player2.Name != null)
                    teamA_Player2.Name.text = teamA_Player2.player.PlayerName ?? "Player 2";
                if (teamA_Player2.Health != null)
                {
                    teamA_Player2.Health.maxValue = teamA_Player2.player.MaxHP;
                    teamA_Player2.Health.value = teamA_Player2.player.HP;
                }
                if (teamA_Player2.Power != null)
                {
                    teamA_Player2.Power.maxValue = teamA_Player2.player.MaxMana;
                    teamA_Player2.Power.value = teamA_Player2.player.Mana;
                }
            }
            else if (teamA_Player2 != null)
            {
                if (teamA_Player2.Name != null) teamA_Player2.Name.text = "";
                if (teamA_Player2.Health != null) teamA_Player2.Health.value = 0;
                if (teamA_Player2.Power != null) teamA_Player2.Power.value = 0;
                if (teamA_Player2.root != null) teamA_Player2.root.SetActive(false);
            }

            if (teamB_Player1 != null && teamB_Player1.player != null)
            {
                if (teamB_Player1.Name != null)
                    teamB_Player1.Name.text = teamB_Player1.player.PlayerName ?? "Opponent 1";
                if (teamB_Player1.Health != null)
                {
                    teamB_Player1.Health.maxValue = teamB_Player1.player.MaxHP;
                    teamB_Player1.Health.value = teamB_Player1.player.HP;
                }
                if (teamB_Player1.Power != null)
                {
                    teamB_Player1.Power.maxValue = teamB_Player1.player.MaxMana;
                    teamB_Player1.Power.value = teamB_Player1.player.Mana;
                }
            }
            else if (teamB_Player1 != null)
            {
                if (teamB_Player1.Name != null) teamB_Player1.Name.text = "";
                if (teamB_Player1.Health != null) teamB_Player1.Health.value = 0;
                if (teamB_Player1.Power != null) teamB_Player1.Power.value = 0;
                if (teamB_Player1.root != null) teamB_Player1.root.SetActive(false);
            }

            if (teamB_Player2 != null && teamB_Player2.player != null)
            {
                if (teamB_Player2.Name != null)
                    teamB_Player2.Name.text = teamB_Player2.player.PlayerName ?? "Opponent 2";
                if (teamB_Player2.Health != null)
                {
                    teamB_Player2.Health.maxValue = teamB_Player2.player.MaxHP;
                    teamB_Player2.Health.value = teamB_Player2.player.HP;
                }
                if (teamB_Player2.Power != null)
                {
                    teamB_Player2.Power.maxValue = teamB_Player2.player.MaxMana;
                    teamB_Player2.Power.value = teamB_Player2.player.Mana;
                }
            }
            else if (teamB_Player2 != null)
            {
                if (teamB_Player2.Name != null) teamB_Player2.Name.text = "";
                if (teamB_Player2.Health != null) teamB_Player2.Health.value = 0;
                if (teamB_Player2.Power != null) teamB_Player2.Power.value = 0;
                if (teamB_Player2.root != null) teamB_Player2.root.SetActive(false);
            }
        }

        public void ShowWinUI()
        {
            showWinUI = true;
            showLoseUI = false;
            ShowResult("YOU WIN!");
        }

        public void ShowLoseUI()
        {
            showLoseUI = true;
            showWinUI = false;
            ShowResult("YOU LOSE!");
        }

        private void ShowHome()
        {
            if (Home != null) Home.SetActive(true);
            if (HUD != null) HUD.SetActive(false);
            if (Result != null) Result.SetActive(false);
        }

        private void ShowHUD()
        {
            if (Home != null) Home.SetActive(false);
            if (HUD != null) HUD.SetActive(true);
            if (Result != null) Result.SetActive(false);
        }

        private void ShowResult(string message)
        {
            if (Home != null) Home.SetActive(false);
            if (HUD != null) HUD.SetActive(false);
            if (Result != null) Result.SetActive(true);
            if (resultText != null) resultText.text = message;

            if (nextLevel != null) nextLevel.gameObject.SetActive(showWinUI);
            if (playAgain != null) playAgain.gameObject.SetActive(showLoseUI);
            if (QuitGame != null) QuitGame.gameObject.SetActive(true);
        }

        private void OnNextLevelClicked()
        {
            showWinUI = false;
            GameManager.Instance?.NextRound();
            ShowHUD();
        }

        private void OnPlayAgainClicked()
        {
            showLoseUI = false;
            GameManager.Instance?.StartGame();
            ShowHUD();
        }

        private void OnQuitGameClicked()
        {
            showWinUI = false;
            showLoseUI = false;
            GameManager.Instance?.QuitGame();
            ShowHome();
        }

        private void Update()
        {
            UpdateUIProfiles();
        }
    }
}