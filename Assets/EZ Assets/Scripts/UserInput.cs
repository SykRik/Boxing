namespace Boxing
{
    public class UserInput : IInputSource
    {
        private PlayerInputActions inputActions;
        private bool punchHead, punchKidney, punchStomach, block;
        private PlayerController player;


        public UserInput(PlayerController player)
        {
            this.player = player;

            inputActions = new PlayerInputActions();
            inputActions.Gameplay.Enable();

            inputActions.Gameplay.PunchHead.performed += _ => punchHead = true;
            inputActions.Gameplay.PunchKidney.performed += _ => punchKidney = true;
            inputActions.Gameplay.PunchStomach.performed += _ => punchStomach = true;
            inputActions.Gameplay.Block.performed += _ => block = true;
        }

        public void UpdateInput(InputQueue queue)
        {
            if (punchHead)
            {
                queue.Enqueue(ConditionType.IsPunchHead);
                punchHead = false;
            }

            if (punchKidney)
            {
                queue.Enqueue(ConditionType.IsPunchKidney);
                punchKidney = false;
            }

            if (punchStomach)
            {
                queue.Enqueue(ConditionType.IsPunchStomach);
                punchStomach = false;
            }

            if (block)
            {
                queue.Enqueue(ConditionType.IsBlock);
                block = false;
            }
        }
    }
}
