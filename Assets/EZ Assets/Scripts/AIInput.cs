using UnityEngine;

namespace Boxing
{
    public interface IInputSource
    {
        void UpdateInput(InputQueue queue);
    }

    public class AIInput : IInputSource
    {
        private PlayerController player;
        private float cooldown = 2f;
        private float timer = 0f;

        public AIInput(PlayerController player)
        {
            this.player = player;
        }

        public void UpdateInput(InputQueue queue)
        {
            timer -= Time.deltaTime;
            if (timer > 0f || !player.IsAlive) return;

            timer = cooldown;

            var hp = player.HP;
            var maxHP = player.MaxHP;
            var mana = player.Mana;

            // Nếu máu thấp, ưu tiên né
            if (hp < maxHP * 0.3f)
            {
                queue.Enqueue(ConditionType.IsBlock); // Dodge
            }
            else if (Random.value > 0.7f && mana > CombatAction.PunchHead.ManaCost)
            {
                queue.Enqueue(ConditionType.IsPunchHead); // PunchHead
            }
            else if (Random.value > 0.7f && mana > CombatAction.PunchKidney.ManaCost)
            {
                queue.Enqueue(ConditionType.IsPunchStomach); // PunchKidney
            }
            else if (Random.value > 0.7f && mana > CombatAction.PunchStomach.ManaCost)
            {
                queue.Enqueue(ConditionType.IsPunchKidney); // PunchStomach
            }
            else if (Random.value > 0.7f)
            {
                queue.Enqueue(ConditionType.IsBlock); // Block
            }
        }
    }
}