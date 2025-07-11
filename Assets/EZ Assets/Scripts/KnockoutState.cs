using UnityEngine;

namespace Boxing
{
    public class KnockoutState : PlayerState
    {
        private float timer = 3f;
        public override bool IsFinished => timer <= 0f;

        public KnockoutState(ICombatant p) : base(p) { }

        public override void OnEnter()
        {
            player.TriggerAnimation("Knockout");
            player.IsAlive = false; // Assuming this field is settable, or call a GameOver method
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