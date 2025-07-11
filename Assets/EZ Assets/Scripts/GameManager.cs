using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Boxing
{
    public enum GameMode
    {
        None = 0,
        OneVsOne,
        OneVsTwo,
        TwoVsTwo,
        StressTest25
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private GameObject stage;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] playerSpawns;
        [SerializeField] private Transform[] opponentSpawns;
        [SerializeField] private CharacterProfiles characterProfiles;
        [SerializeField] private TextAsset characterProfilesAsset;
        [SerializeField] private GameMode gameMode = GameMode.None;

        private int currentRound = 0;
        private List<PlayerController> players = new();
        private List<PlayerController> opponents = new();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (characterProfilesAsset != null)
            {
                try
                {
                    characterProfiles = JsonUtility.FromJson<CharacterProfiles>(characterProfilesAsset.text);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to deserialize characterProfilesAsset: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("characterProfilesAsset is not assigned in the Inspector!");
            }
        }

        private void Start()
        {
            if (gameMode != GameMode.None)
            {
                StartGame();
            }
        }

        public void StartGame()
        {
            currentRound = 0;
            SpawnPlayers(currentRound);
        }

        public void NextRound()
        {
            currentRound++;
            if (currentRound >= characterProfiles.opponents.Count)
                currentRound = characterProfiles.opponents.Count - 1;

            SpawnPlayers(currentRound);
        }

        private void SpawnPlayers(int roundIndex)
        {
            foreach (var p in players) if (p != null) Destroy(p.gameObject);
            foreach (var o in opponents) if (o != null) Destroy(o.gameObject);
            players.Clear();
            opponents.Clear();

            if (playerSpawns.Length == 0 || opponentSpawns.Length == 0)
            {
                Debug.LogError("Player or opponent spawn points are not assigned!");
                return;
            }

            switch (gameMode)
            {
                case GameMode.OneVsOne:
                    stage.SetActive(true);
                    var p1A = SpawnPlayer(playerSpawns[0], true);
                    var p1B = SpawnOpponent(opponentSpawns[0], roundIndex, true);
                    if (p1A != null && p1B != null)
                    {
                        p1A.Initialize(p1B);
                        p1B.Initialize(p1A);
                        p1A.OnDefeated += OnPlayerDefeated;
                        p1B.OnDefeated += OnOpponentDefeated;
                    }
                    break;

                case GameMode.OneVsTwo:
                    stage.SetActive(true);
                    var p1A_OneVsTwo = SpawnPlayer(playerSpawns[0], true);
                    var p1B_OneVsTwo = SpawnOpponent(opponentSpawns[0], roundIndex, true);
                    var p2B_OneVsTwo = SpawnOpponent(opponentSpawns[1], roundIndex + 1, false);
                    if (p1A_OneVsTwo != null && p1B_OneVsTwo != null && p2B_OneVsTwo != null)
                    {
                        p1A_OneVsTwo.Initialize(p1B_OneVsTwo);
                        p1B_OneVsTwo.Initialize(p1A_OneVsTwo);
                        p2B_OneVsTwo.FSM?.SetState<IdleState>();
                        p1A_OneVsTwo.OnDefeated += OnPlayerDefeated;
                        p1B_OneVsTwo.OnDefeated += OnOpponentDefeated;
                        p2B_OneVsTwo.OnDefeated += OnOpponentDefeated;
                    }
                    break;

                case GameMode.TwoVsTwo:
                    stage.SetActive(true);
                    var p1A_TwoVsTwo = SpawnPlayer(playerSpawns[0], true); // Người chơi chính
                    var p2A_TwoVsTwo = SpawnPlayer(playerSpawns[1], false); // Đồng đội idle
                    var p1B_TwoVsTwo = SpawnOpponent(opponentSpawns[0], roundIndex, true); // Đối thủ 1
                    var p2B_TwoVsTwo = SpawnOpponent(opponentSpawns[1], roundIndex + 1, false); // Đối thủ 2 idle
                    if (p1A_TwoVsTwo != null && p2A_TwoVsTwo != null && p1B_TwoVsTwo != null && p2B_TwoVsTwo != null)
                    {
                        p1A_TwoVsTwo.Initialize(p1B_TwoVsTwo);
                        p1B_TwoVsTwo.Initialize(p1A_TwoVsTwo);
                        p2A_TwoVsTwo.FSM?.SetState<IdleState>();
                        p2B_TwoVsTwo.FSM?.SetState<IdleState>();
                        p1A_TwoVsTwo.OnDefeated += OnPlayerDefeated;
                        p2A_TwoVsTwo.OnDefeated += OnPlayerDefeated;
                        p1B_TwoVsTwo.OnDefeated += OnOpponentDefeated;
                        p2B_TwoVsTwo.OnDefeated += OnOpponentDefeated;
                    }
                    break;

                case GameMode.StressTest25:
                    stage.SetActive(false);
                    for (int i = 0; i < 25; i++)
                    {
                        var pos = Random.insideUnitCircle.ToV3XZ() * 5f;
                        var pos1 = Random.insideUnitCircle.ToV3XZ().normalized * 0.5f + pos;
                        var pos2 = Random.insideUnitCircle.ToV3XZ().normalized * 0.5f + pos;
                        var rot1 = Quaternion.LookRotation(Vector3.Normalize(pos2 - pos1));
                        var rot2 = Quaternion.LookRotation(Vector3.Normalize(pos1 - pos2));

                        var go1 = Instantiate(playerPrefab, pos1, rot1);
                        var go2 = Instantiate(playerPrefab, pos2, rot2);

                        var p1 = go1.GetComponent<PlayerController>();
                        var p2 = go2.GetComponent<PlayerController>();

                        p1.SetInputSource(new AIInput(p1));
                        p2.SetInputSource(new AIInput(p2));

                        p1.Initialize(p2);
                        p2.Initialize(p1);

                        players.Add(p1);
                        opponents.Add(p2);
                    }
                    break;
            }

            if (players.Count > 0 && opponents.Count > 0 && gameMode != GameMode.None)
            {
                UIManager.Instance?.SetPlayers(
                    players.Count > 0 ? players[0] : null,
                    players.Count > 1 ? players[1] : null,
                    opponents.Count > 0 ? opponents[0] : null,
                    opponents.Count > 1 ? opponents[1] : null
                );
            }
        }

        private PlayerController SpawnPlayer(Transform spawnPoint, bool active)
        {
            if (spawnPoint == null)
            {
                Debug.LogError("Spawn point is null!");
                return null;
            }
            var go = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            var player = go.GetComponent<PlayerController>();
            player.SetInputSource(active ? new UserInput(player) : null);
            players.Add(player);
            return player;
        }

        private PlayerController SpawnOpponent(Transform spawnPoint, int profileIndex, bool active)
        {
            if (spawnPoint == null)
            {
                Debug.LogError("Spawn point is null!");
                return null;
            }
            profileIndex = Mathf.Clamp(profileIndex, 0, characterProfiles.opponents.Count - 1);
            var go = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            var opponent = go.GetComponent<PlayerController>();
            var data = characterProfiles.opponents[profileIndex];
            opponent.OverrideStats(data.MaxHP, data.MaxMana, data.OpponentName);
            opponent.SetInputSource(active ? new AIInput(opponent) : null);
            opponents.Add(opponent);
            return opponent;
        }
        public void SwitchPlayer()
        {
            if (gameMode != GameMode.TwoVsTwo || players.Count < 2) return;

            var currentPlayer = players.Find(p => p.HasInputSource());
            if (currentPlayer == null) return;

            var currentPlayerIndex = players.IndexOf(currentPlayer);
            var nextPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            var nextPlayer = players[nextPlayerIndex];

            while (nextPlayer != currentPlayer && !nextPlayer.IsAlive)
            {
                nextPlayerIndex = (nextPlayerIndex + 1) % players.Count;
                nextPlayer = players[nextPlayerIndex];
            }

            if (nextPlayer != currentPlayer && nextPlayer.IsAlive)
            {
                // Hoán đổi vị trí cho team A
                var tempPlayerPosition = currentPlayer.transform.position;
                currentPlayer.transform.position = nextPlayer.transform.position;
                nextPlayer.transform.position = tempPlayerPosition;

                // Cập nhật trạng thái
                currentPlayer.SetInputSource(null);
                nextPlayer.SetInputSource(new UserInput(nextPlayer)); // UserInput cho người chơi chính

                // Cập nhật mục tiêu (giả sử đối thủ hiện tại vẫn giữ nguyên)
                var currentOpponent = opponents.Find(o => o.HasInputSource());
                if (currentOpponent != null)
                {
                    nextPlayer.Initialize(currentOpponent);
                    currentOpponent.Initialize(nextPlayer);
                }

                UIManager.Instance?.Log($"Switched player to {nextPlayer.PlayerName}");
                //UIManager.Instance?.SetPlayers(
                //    players[nextPlayerIndex],
                //    players[(nextPlayerIndex + 1) % players.Count],
                //    opponents.Count > 0 ? opponents[0] : null,
                //    opponents.Count > 1 ? opponents[1] : null
                //);
            }
            else
            {
                UIManager.Instance?.Log("No valid player to switch to!");
            }
        }

        public void SwitchOpponent()
        {
            if (gameMode != GameMode.OneVsTwo && gameMode != GameMode.TwoVsTwo || opponents.Count < 2) return;

            var currentOpponent = opponents.Find(o => o.HasInputSource());
            if (currentOpponent == null) return;

            var currentOpponentIndex = opponents.IndexOf(currentOpponent);
            var nextOpponentIndex = (currentOpponentIndex + 1) % opponents.Count;
            var nextOpponent = opponents[nextOpponentIndex];

            while (nextOpponent != currentOpponent && !nextOpponent.IsAlive)
            {
                nextOpponentIndex = (nextOpponentIndex + 1) % opponents.Count;
                nextOpponent = opponents[nextOpponentIndex];
            }

            if (nextOpponent != currentOpponent && nextOpponent.IsAlive)
            {
                // Hoán đổi vị trí cho team B
                var tempOpponentPosition = currentOpponent.transform.position;
                currentOpponent.transform.position = nextOpponent.transform.position;
                nextOpponent.transform.position = tempOpponentPosition;

                // Cập nhật trạng thái
                currentOpponent.SetInputSource(null);
                nextOpponent.SetInputSource(new AIInput(nextOpponent));

                // Cập nhật mục tiêu
                var currentPlayer = players.Find(p => p.HasInputSource());
                if (currentPlayer != null)
                {
                    currentPlayer.Initialize(nextOpponent);
                    nextOpponent.Initialize(currentPlayer);
                }

                UIManager.Instance?.Log($"Switched opponent to {nextOpponent.PlayerName}");
                //UIManager.Instance?.SetPlayers(
                //    players.Count > 0 ? players[0] : null,
                //    players.Count > 1 ? players[1] : null,
                //    opponents[nextOpponentIndex],
                //    opponents[(nextOpponentIndex + 1) % opponents.Count]
                //);
            }
            else
            {
                UIManager.Instance?.Log("No valid opponent to switch to!");
            }
        }
        private void OnPlayerDefeated(PlayerController player)
        {
            if (gameMode == GameMode.StressTest25) return;
            var allPlayersDefeated = players.Count == players.Count(x => !x.IsAlive);
            if (allPlayersDefeated)
            {
                OnFightResult(false); // Người chơi thua
            }
            else if (gameMode == GameMode.TwoVsTwo)
            {
                SwitchPlayer(); // Tự động chuyển sang người chơi còn lại nếu còn sống
            }
        }

        private void OnOpponentDefeated(PlayerController opponent)
        {
            if (gameMode == GameMode.StressTest25) return;
            var allOpponentsDefeated = opponents.Count == players.Count(x => !x.IsAlive);
            if (allOpponentsDefeated)
            {
                OnFightResult(true); // Người chơi thắng
            }
            else if (gameMode == GameMode.OneVsTwo || gameMode == GameMode.TwoVsTwo)
            {
                SwitchOpponent(); // Tự động chuyển sang đối thủ còn lại nếu còn sống
            }
        }

        public void OnFightResult(bool playerWon)
        {
            if (gameMode == GameMode.StressTest25)
            {
                UIManager.Instance?.ShowLoseUI();
                return;
            }
            if (playerWon)
                UIManager.Instance?.ShowWinUI();
            else
                UIManager.Instance?.ShowLoseUI();
        }

        public void SetGameMode(GameMode mode)
        {
            gameMode = mode;
            if (gameMode == GameMode.None)
                QuitGame();
            else
                StartGame();
        }

        public GameMode GetGameMode()
        {
            return gameMode;
        }

        public void QuitGame()
        {
            foreach (var p in players) if (p != null) Destroy(p.gameObject);
            foreach (var o in opponents) if (o != null) Destroy(o.gameObject);
            players.Clear();
            opponents.Clear();
            currentRound = 0;
            stage.SetActive(false);
        }

        public List<PlayerController> GetPlayers()
        {
            return players;
        }

        public List<PlayerController> GetOpponents()
        {
            return opponents;
        }
    }
}