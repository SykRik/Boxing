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
            public Image highlight; // Làm nổi bật nhân vật trên võ đài
        }

        public static UIManager Instance;

        private Queue<string> logs = new Queue<string>();
        private const int maxLogs = 10;

        [SerializeField] private Button gameMode1vs1;
        [SerializeField] private Button gameMode1vs2;
        [SerializeField] private Button gameMode2vs2;
        [SerializeField] private Button gameMode25vs25;
        [SerializeField] private Button nextLevel;
        [SerializeField] private Button playAgain;
        [SerializeField] private Button QuitGame;
        [SerializeField] private Button BackToHome;
        [SerializeField] private Button SwitchPlayer;
        [SerializeField] private TextMeshProUGUI logText;

        [SerializeField] private UIProfile teamA_Player1;
        [SerializeField] private UIProfile teamA_Player2;
        [SerializeField] private UIProfile teamB_Player1;
        [SerializeField] private UIProfile teamB_Player2;

        [SerializeField] private GameObject HUD;
        [SerializeField] private GameObject Home;
        [SerializeField] private GameObject Result;
        [SerializeField] private TextMeshProUGUI resultText;


        [SerializeField] private Button ATK1;
        [SerializeField] private Button ATK2;
        [SerializeField] private Button ATK3;
        [SerializeField] private Button BLOCK;

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
            if (BackToHome != null)
                BackToHome.onClick.AddListener(OnBackToHomeClicked);
            if (SwitchPlayer != null)
                SwitchPlayer.onClick.AddListener(OnSwitchPlayerClicked);

            if (ATK1 != null)
                ATK1.onClick.AddListener(OnATK1Clicked);
            if (ATK2 != null)
                ATK2.onClick.AddListener(OnATK2Clicked);
            if (ATK3 != null)
                ATK3.onClick.AddListener(OnATK3Clicked);
            if (BLOCK != null)
                BLOCK.onClick.AddListener(OnBLOCKClicked);

            InitializeUIProfiles();
            ShowHome();
        }

        private void OnBLOCKClicked()
        {
            var player = null as PlayerController;
            if (teamA_Player1.player != null && teamA_Player1.player.HasInputSource())
            {
                player = teamA_Player1.player;
            }
            if (teamA_Player2.player != null && teamA_Player2.player.HasInputSource())
            {
                player = teamA_Player2.player;
            }
            if (player != null)
            {
                player.InputQueue.Enqueue(ConditionType.IsBlock);
            }    
        }

        private void OnATK3Clicked()
        {
            var player = null as PlayerController;
            if (teamA_Player1.player != null && teamA_Player1.player.HasInputSource())
            {
                player = teamA_Player1.player;
            }
            if (teamA_Player2.player != null && teamA_Player2.player.HasInputSource())
            {
                player = teamA_Player2.player;
            }
            if (player != null)
            {
                player.InputQueue.Enqueue(ConditionType.IsPunchStomach);
            }
        }

        private void OnATK2Clicked()
        {
            var player = null as PlayerController;
            if (teamA_Player1.player != null && teamA_Player1.player.HasInputSource())
            {
                player = teamA_Player1.player;
            }
            if (teamA_Player2.player != null && teamA_Player2.player.HasInputSource())
            {
                player = teamA_Player2.player;
            }
            if (player != null)
            {
                player.InputQueue.Enqueue(ConditionType.IsPunchKidney);
            }
        }

        private void OnATK1Clicked()
        {
            var player = null as PlayerController;
            if (teamA_Player1.player != null && teamA_Player1.player.HasInputSource())
            {
                player = teamA_Player1.player;
            }
            if (teamA_Player2.player != null && teamA_Player2.player.HasInputSource())
            {
                player = teamA_Player2.player;
            }
            if (player != null)
            {
                player.InputQueue.Enqueue(ConditionType.IsPunchHead);
            }
        }

        private void Update()
        {
            UpdateUIProfiles();
        }

        private void SetGameModeAndStart(GameMode mode)
        {
            GameManager.Instance?.SetGameMode(mode);
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
        }

        private void UpdateUIProfiles()
        {
            UpdateUIProfile(teamA_Player1, "Player 1");
            UpdateUIProfile(teamA_Player2, "Player 2");
            UpdateUIProfile(teamB_Player1, "Opponent 1");
            UpdateUIProfile(teamB_Player2, "Opponent 2");
        }

        private void UpdateUIProfile(UIProfile profile, string defaultName)
        {
            if (profile != null && profile.player != null)
            {
                if (profile.Name != null)
                    profile.Name.text = profile.player.PlayerName ?? defaultName;
                if (profile.Health != null)
                {
                    profile.Health.maxValue = profile.player.MaxHP;
                    profile.Health.value = profile.player.HP;
                }
                if (profile.Power != null)
                {
                    profile.Power.maxValue = profile.player.MaxMana;
                    profile.Power.value = profile.player.Mana;
                }
                if (profile.highlight != null)
                {
                    profile.highlight.enabled = profile.player.HasInputSource();
                }
                var canvasGroup = profile.root?.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = profile.player.IsAlive ? 1f : 0.5f;
                }
            }
            else if (profile != null)
            {
                if (profile.Name != null) profile.Name.text = "";
                if (profile.Health != null) profile.Health.value = 0;
                if (profile.Power != null) profile.Power.value = 0;
                if (profile.highlight != null) profile.highlight.enabled = false;
                if (profile.root != null) profile.root.SetActive(false);
                var canvasGroup = profile.root?.GetComponent<CanvasGroup>();
                if (canvasGroup != null) canvasGroup.alpha = 1f;
            }
        }

        public void Log(string message)
        {
            if (logs.Count >= maxLogs)
                logs.Dequeue();
            logs.Enqueue(message);
            UpdateLogDisplay();
        }

        private void UpdateLogDisplay()
        {
            if (logText != null)
            {
                logText.text = string.Join("\n", logs);
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

        public void ShowResult(string message)
        {
            if (Home != null) Home.SetActive(false);
            if (HUD != null) HUD.SetActive(false);
            if (Result != null) Result.SetActive(true);
            if (resultText != null) resultText.text = message;

            if (nextLevel != null) nextLevel.gameObject.SetActive(showWinUI);
            if (playAgain != null) playAgain.gameObject.SetActive(showLoseUI);
            if (QuitGame != null) QuitGame.gameObject.SetActive(true);
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

        private void OnBackToHomeClicked()
        {
            showWinUI = false;
            showLoseUI = false;
            GameManager.Instance?.QuitGame();
            ShowHome();
        }

        private void OnSwitchPlayerClicked()
        {
            GameManager.Instance?.SwitchPlayer();
            UpdateUIProfiles();
        }
    }
}