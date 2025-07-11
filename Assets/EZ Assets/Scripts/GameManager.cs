using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Boxing
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform p1Spawn;
        [SerializeField] private Transform p2Spawn;
        [SerializeField] private CharacterProfiles characterProfiles;

        private int currentRound = 0;
        private PlayerController player;
        private PlayerController opponent;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            StartGame();
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
            if (player != null) Destroy(player.gameObject);
            if (opponent != null) Destroy(opponent.gameObject);

            var pos1 = p1Spawn.position;
            var pos2 = p2Spawn.position;

            var rot1 = Quaternion.LookRotation(Vector3.Normalize(pos2 - pos1));
            var rot2 = Quaternion.LookRotation(Vector3.Normalize(pos1 - pos2));

            var go1 = Instantiate(playerPrefab, pos1, rot1);
            var go2 = Instantiate(playerPrefab, pos2, rot2);

            player = go1.GetComponent<PlayerController>();
            opponent = go2.GetComponent<PlayerController>();

            player.SetInputSource(new UserInput(player));
            opponent.SetInputSource(new AIInput(opponent));

            var opponentData = characterProfiles.opponents[roundIndex];
            opponent.OverrideStats(opponentData.MaxHP, opponentData.MaxMana, opponentData.OpponentName);

            player.Initialize(opponent);
            opponent.Initialize(player);

            UIManager.Instance?.SetPlayers(player, opponent);
        }

        public void OnFightResult(bool playerWon)
        {
            if (playerWon)
                UIManager.Instance?.ShowWinUI();
            else
                UIManager.Instance?.ShowLoseUI();
        }

        private void Init()
        {
            var pos1 = p1Spawn.position;
            var pos2 = p2Spawn.position;

            var rot1 = Quaternion.LookRotation(Vector3.Normalize(pos2 - pos1));
            var rot2 = Quaternion.LookRotation(Vector3.Normalize(pos1 - pos2));

            var go1 = Instantiate(playerPrefab, pos1, rot1);
            var go2 = Instantiate(playerPrefab, pos2, rot2);

            var p1 = go1.GetComponent<PlayerController>();
            var p2 = go2.GetComponent<PlayerController>();

            p1.SetInputSource(new UserInput(p1));
            p2.SetInputSource(new AIInput(p2));

            p1.Initialize(p2);
            p2.Initialize(p1);

            UIManager.Instance?.SetPlayers(p1, p2);
        }

        private void Test()
        {
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

                //UIManager.Instance?.SetPlayers(p1, p2);
            }
        }

    }




}
