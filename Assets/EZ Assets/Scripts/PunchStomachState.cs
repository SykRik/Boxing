using UnityEngine;

namespace Boxing
{
    public class PunchStomachState : PlayerState
    {
        private float timer;
        public override bool IsFinished => timer <= 0f;

        public PunchStomachState(ICombatant self) : base(self)
        {
            timer = CombatAction.PunchStomach.Duration;
        }

        public override void OnEnter()
        {
            player.TriggerAnimation(CombatAction.PunchStomach.TriggerName);
        }

        public override void OnUpdate()
        {
            timer -= Time.deltaTime;

            if (IsFinished && player.GetOpponent() is PlayerController target)
            {
                if (player.UseMana(CombatAction.PunchStomach.ManaCost))
                    target.TakeDamage(CombatAction.PunchStomach.Damage, CombatAction.PunchStomach);
            }
        }

        public override void OnExit()
        {
        }
    }
}
