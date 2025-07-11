using UnityEngine;

namespace Boxing
{
    public class PunchHeadState : PlayerState
    {
        private float timer;
        public override bool IsFinished => timer <= 0f;

        public PunchHeadState(ICombatant self) : base(self)
        {
            timer = CombatAction.PunchHead.Duration;
        }

        public override void OnEnter()
        {
            player.TriggerAnimation(CombatAction.PunchHead.TriggerName);
        }

        public override void OnUpdate()
        {
            timer -= Time.deltaTime;

            if (IsFinished && player.GetOpponent() is PlayerController target)
            {
                if (player.UseMana(CombatAction.PunchHead.ManaCost))
                    target.TakeDamage(CombatAction.PunchHead.Damage, CombatAction.PunchHead);
            }
        }

        public override void OnExit()
        {
        }
    }
}
