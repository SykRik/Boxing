using UnityEngine;

namespace Boxing
{
    public class PunchKidneyState : PlayerState
    {
        private float timer;
        public override bool IsFinished => timer <= 0f;

        public PunchKidneyState(ICombatant self) : base(self)
        {
            timer = CombatAction.PunchKidney.Duration;
        }

        public override void OnEnter()
        {
            player.TriggerAnimation(CombatAction.PunchKidney.TriggerName);
        }

        public override void OnUpdate()
        {
            timer -= Time.deltaTime;

            if (IsFinished && player.GetOpponent() is PlayerController target)
            {
                if (player.UseMana(CombatAction.PunchKidney.ManaCost))
                    target.TakeDamage(CombatAction.PunchKidney.Damage, CombatAction.PunchKidney);
            }
        }

        public override void OnExit()
        {
        }
    }
}
