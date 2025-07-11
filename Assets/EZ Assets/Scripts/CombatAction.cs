namespace Boxing
{
    public class CombatAction
    {
        public string TriggerName;
        public int ManaCost;
        public int Damage;
        public float Duration;
        public string HitReactionTrigger;

        public static readonly CombatAction PunchHead = new()
        {
            TriggerName = "PunchHead",
            ManaCost = 4,
            Damage = 3,
            Duration = 1.0f,
            HitReactionTrigger = "HitHead"
        };

        public static readonly CombatAction PunchKidney = new()
        {
            TriggerName = "PunchKidney",
            ManaCost = 3,
            Damage = 2,
            Duration = 0.9f,
            HitReactionTrigger = "HitKidney"
        };

        public static readonly CombatAction PunchStomach = new()
        {
            TriggerName = "PunchStomach",
            ManaCost = 2,
            Damage = 1,
            Duration = 0.8f,
            HitReactionTrigger = "HitStomach"
        };
    }
}