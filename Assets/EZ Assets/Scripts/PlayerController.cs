using System.Collections.Generic;
using UnityEngine;

namespace Boxing
{
    public class PlayerController : MonoBehaviour, ICombatant
    {
        public List<GameObject> prefabs = new();
        public GameObject modelHolder = null;

        public string PlayerName;
        public int MaxHP = 100;
        public int MaxMana = 100;

        public int HP { get; private set; }
        public int Mana { get; private set; }
        public bool IsAlive { get; set; } = true;
        public bool IsInvincible { get; set; } = false;
        public Animator Animator { get; private set; }

        public PlayerFSM FSM { get; set; }
        public InputQueue InputQueue { get; set; } = new InputQueue();
        public CombatAction CurrentAction { get; private set; }

        private PlayerController opponent;
        private IInputSource inputSource;

        private float gameTimer = 0f;
        private float manaTimer = 0f;

        public System.Action<PlayerController> OnDefeated;

        void Awake()
        {
            var random = Random.Range(0, 100) % prefabs.Count;
            var model = GameObject.Instantiate(prefabs[random]);
            model.transform.SetParent(modelHolder.transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;

            FSM = new PlayerFSM(this);
            Animator = GetComponentInChildren<Animator>();
            HP = MaxHP;
            Mana = MaxMana;
        }

        public void Initialize(PlayerController opponent)
        {
            this.opponent = opponent;
            FSM.RegisterAllStates();
            FSM.RegisterAllConditions();
            FSM.RegisterAllTransitions();
            FSM.SetState<IdleState>();
        }

        public void SetInputSource(IInputSource input)
        {
            inputSource = input;
            InputQueue.ClearAll();
            if (input == null)
            {
                FSM?.SetState<IdleState>();
            }
        }

        void Update()
        {
            inputSource?.UpdateInput(InputQueue);
            FSM?.Update();
            RegenerateMana();
        }

        private void RegenerateMana()
        {
            gameTimer += Time.deltaTime;
            manaTimer += Time.deltaTime;

            float interval = gameTimer < 60 ? 2f : gameTimer < 120 ? 1f : 0.5f;
            int regenAmount = gameTimer < 120 ? 1 : 2;

            if (manaTimer >= interval)
            {
                Mana = Mathf.Min(MaxMana, Mana + regenAmount);
                manaTimer = 0f;
                Debug.Log($"[{PlayerName}] +{regenAmount} mana. Mana = {Mana}");
            }
        }

        public bool UseMana(int amount)
        {
            if (Mana < amount) return false;
            Mana -= amount;
            Debug.Log($"[{PlayerName}] -{amount} mana. Mana = {Mana}");
            return true;
        }

        public void TriggerAnimation(string trigger) => Animator?.SetTrigger(trigger);

        public void TakeDamage(int damage, CombatAction source)
        {
            if (!IsAlive) return;
            if (IsInvincible)
            {
                Debug.Log($"{PlayerName} dodged the attack!");
                return;
            }
            if (FSM.CurrentState is BlockingState)
            {
                Debug.Log($"{PlayerName} blocked the attack!");
                return;
            }

            HP -= damage;
            Debug.Log($"{PlayerName} took {damage} damage. HP = {HP}");

            if (HP > 0)
            {
                if (source == CombatAction.PunchHead)
                    FSM.SetCondition(ConditionType.IsStunHead, true);
                else if (source == CombatAction.PunchKidney)
                    FSM.SetCondition(ConditionType.IsStunKidney, true);
                else if (source == CombatAction.PunchStomach)
                    FSM.SetCondition(ConditionType.IsStunStomach, true);
            }
            else
            {
                IsAlive = false;
                FSM.SetState<KnockoutState>();
                OnDefeated?.Invoke(this);
            }
        }

        public ICombatant GetOpponent() => opponent;

        public void OverrideStats(int maxHP, int maxMana, string name)
        {
            this.PlayerName = name;
            this.MaxHP = maxHP;
            this.MaxMana = maxMana;
            this.Mana = maxMana;
            this.HP = maxHP;
        }

        public bool xxx = false;

        public bool HasInputSource()
        {
            xxx = inputSource != null;
            return inputSource != null;
        }
    }
}