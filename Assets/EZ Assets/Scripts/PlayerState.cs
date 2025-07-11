namespace Boxing
{
    public abstract class PlayerState
    {
        protected readonly ICombatant player;

        public PlayerState(ICombatant player)
        {
            this.player = player;
        }

        public abstract void OnEnter();
        public abstract void OnUpdate();
        public abstract void OnExit();

        public virtual bool IsFinished => false;
    }


}