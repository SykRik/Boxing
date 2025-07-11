using System.Collections.Generic;
using UnityEngine;

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
            foreach (var p in players) Destroy(p.gameObject);
            foreach (var o in opponents) Destroy(o.gameObject);
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
                    {
                        stage.SetActive(true);
                        var p1A = SpawnPlayer(playerSpawns[0], true);
                        var p1B = SpawnOpponent(opponentSpawns[0], roundIndex);
                        p1A.Initialize(p1B);
                        p1B.Initialize(p1A);
                    }

                    break;

                case GameMode.OneVsTwo:
                    {
                        stage.SetActive(true);
                        var p1A = SpawnPlayer(playerSpawns[0], true);
                        var p1B = SpawnOpponent(opponentSpawns[0], roundIndex);
                        var p2B = SpawnOpponent(opponentSpawns[1], roundIndex + 1);
                        p1A.Initialize(p1B);
                        p1B.Initialize(p1A);
                    }
                    break;

                case GameMode.TwoVsTwo:
                    {
                        stage.SetActive(true);
                        var p1A = SpawnPlayer(playerSpawns[0], true);
                        var p2A = SpawnPlayer(playerSpawns[1], false);
                        var p1B = SpawnOpponent(opponentSpawns[0], roundIndex);
                        var p2B = SpawnOpponent(opponentSpawns[1], roundIndex + 1);
                        p1A.Initialize(p1B);
                        p1B.Initialize(p1A);
                    }
                    break;

                case GameMode.StressTest25:
                    {
                        stage.SetActive(false);
                        for (int i = 0; i < 25; i++)
                        {
                            var pos = Random.insideUnitCircle.ToV3XZ() * 10f;
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
                        }
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

        private PlayerController SpawnPlayer(Transform spawnPoint, bool isUser)
        {
            if (spawnPoint == null)
            {
                Debug.LogError("Spawn point is null!");
                return null;
            }
            var go = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            var player = go.GetComponent<PlayerController>();
            player.SetInputSource(isUser ? new UserInput(player) : new AIInput(player));
            players.Add(player);

            return player;
        }

        private PlayerController SpawnOpponent(Transform spawnPoint, int profileIndex)
        {
            if (spawnPoint == null)
            {
                Debug.LogError("Spawn point is null!");
                return null;
            }
            profileIndex = Mathf.Clamp(profileIndex, 0, characterProfiles.opponents.Count - 1);
            var go = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            var opponent = go.GetComponent<PlayerController>();
            opponent.SetInputSource(new AIInput(opponent));
            var data = characterProfiles.opponents[profileIndex];
            opponent.OverrideStats(data.MaxHP, data.MaxMana, data.OpponentName);
            opponents.Add(opponent);

            return opponent;
        }

        public void OnFightResult(bool playerWon)
        {
            if (gameMode == GameMode.StressTest25)
            {
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
            foreach (var p in players) Destroy(p.gameObject);
            foreach (var o in opponents) Destroy(o.gameObject);
            players.Clear();
            opponents.Clear();
            currentRound = 0;
            stage.SetActive(false);
        }
    }
}