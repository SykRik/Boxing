using UnityEngine;

namespace Boxing
{
    public class HitStunHeadState : PlayerState
    {
        private float timer;
        public override bool IsFinished => timer <= 0f;

        public HitStunHeadState(ICombatant self) : base(self)
        {
            timer = CombatAction.PunchHead.Duration;
        }

        public override void OnEnter()
        {
            player.TriggerAnimation(CombatAction.PunchHead.HitReactionTrigger);
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
