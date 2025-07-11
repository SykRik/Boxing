using UnityEngine;

namespace Boxing
{
    public class HitStunStomachState : PlayerState
    {
        private float timer;
        public override bool IsFinished => timer <= 0f;

        public HitStunStomachState(ICombatant self) : base(self)
        {
            timer = CombatAction.PunchStomach.Duration;
        }

        public override void OnEnter()
        {
            player.TriggerAnimation(CombatAction.PunchStomach.HitReactionTrigger);
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
