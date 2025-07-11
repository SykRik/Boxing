using UnityEngine;

namespace Boxing
{
    public interface ICombatant
    {
        int HP { get; }
        int Mana { get; }
        bool IsAlive { get; set; }
        bool IsInvincible { get; set; }

        ICombatant GetOpponent();
        bool UseMana(int amount);
        void TriggerAnimation(string trigger);
        void TakeDamage(int damage, CombatAction source);

        Animator Animator { get; }
        InputQueue InputQueue { get; }
        CombatAction CurrentAction { get; } // ✅ NEW
        PlayerFSM FSM { get; set; }
    }



}