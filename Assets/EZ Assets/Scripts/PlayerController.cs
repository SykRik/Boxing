using UnityEngine;

namespace Boxing
{
    public class PlayerController : MonoBehaviour, ICombatant
    {
        public string PlayerName;
        public int MaxHP = 20;
        public int MaxMana = 100;

        public int HP { get; private set; }
        public int Mana { get; private set; }
        public bool IsAlive { get; set; } = true;
        public bool IsInvincible { get; set; } = false;
        public Animator Animator { get; private set; }

        public PlayerFSM FSM { get; set; }
        public InputQueue InputQueue { get; set; } = new InputQueue();
        public CombatAction CurrentAction { get; private set; }   // ✅ Đã implement

        private PlayerController opponent;
        private IInputSource inputSource;

        private float gameTimer = 0f;
        private float manaTimer = 0f;

        void Awake()
        {
            Animator = GetComponentInChildren<Animator>();
            HP = MaxHP;
            Mana = MaxMana;
        }

        public void Initialize(PlayerController opponent)
        {
            this.opponent = opponent;

            FSM = new PlayerFSM(this);
            FSM.RegisterAllStates();
            FSM.RegisterAllConditions();
            FSM.RegisterAllTransitions();
            FSM.SetState<IdleState>();
        }

        public void SetInputSource(IInputSource input)
        {
            inputSource = input;
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
                Debug.Log($"[{name}] +{regenAmount} mana. Mana = {Mana}");
            }
        }

        public bool UseMana(int amount)
        {
            if (Mana < amount) return false;
            Mana -= amount;
            Debug.Log($"[{name}] -{amount} mana. Mana = {Mana}");
            return true;
        }

        public void TriggerAnimation(string trigger) => Animator?.SetTrigger(trigger);

        public void TakeDamage(int damage, CombatAction source)
        {
            if (!IsAlive) return;
            if (IsInvincible)
            {
                Debug.Log($"{name} dodged the attack!");
                return;
            }
            if (FSM.CurrentState is BlockingState)
            {
                Debug.Log($"{name} blocked the attack!");
                return;
            }

            HP -= damage;
            Debug.Log($"{name} took {damage} damage. HP = {HP}");

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
    }
}
