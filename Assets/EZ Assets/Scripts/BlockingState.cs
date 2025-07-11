using UnityEngine;

namespace Boxing
{
    public class BlockingState : PlayerState
    {
        private float duration = 0.6f;

        public override bool IsFinished => duration <= 0f;

        public BlockingState(ICombatant p) : base(p) { }

        public override void OnEnter()
        {
            if (!player.UseMana(1))
            {
                player.TriggerAnimation("Fail");
                duration = 0f;
                return;
            }

            player.TriggerAnimation("Block");
            duration = 0.6f;
        }

        public override void OnUpdate()
        {
            duration -= Time.deltaTime;
        }

        public override void OnExit()
        {
        }
    }

}