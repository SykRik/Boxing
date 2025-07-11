using UnityEngine;

namespace Boxing
{
    public class IdleState : PlayerState
    {
        private PlayerFSM fsm;

        public IdleState(ICombatant self) : base(self) {
            fsm = self.FSM;
        }

        public override void OnEnter()
        {
            player.TriggerAnimation("Idle");
        }

        public override void OnUpdate()
        {
            if (!player.IsAlive)
                return;
        }

        public override void OnExit()
        {
        }

    }
}
