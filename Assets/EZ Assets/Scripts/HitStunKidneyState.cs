using UnityEngine;

namespace Boxing
{
    public class HitStunKidneyState : PlayerState
    {
        private float timer;
        public override bool IsFinished => timer <= 0f;

        public HitStunKidneyState(ICombatant self) : base(self)
        {
            timer = CombatAction.PunchKidney.Duration;
        }

        public override void OnEnter()
        {
            player.TriggerAnimation(CombatAction.PunchKidney.HitReactionTrigger);
        }

        public override void OnUpdate()
        {
            timer -= Time.deltaTime;
        }

        public override void OnExit()
        {
        }
    }
}
