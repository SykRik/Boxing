using System;
using System.Collections.Generic;

namespace Boxing
{
    public class PlayerFSM
    {
        private PlayerState currentState;
        private ICombatant owner;

        private readonly Dictionary<Type, List<Transition>> transitions = new();
        private List<Transition> currentTransitions = new();
        private static readonly List<Transition> emptyTransitions = new();

        private readonly Dictionary<Type, PlayerState> cachedStates = new();
        private readonly Dictionary<ConditionType, Func<bool>> conditions = new();

        public PlayerState CurrentState => currentState;

        public PlayerFSM(ICombatant owner) => this.owner = owner;

        public void RegisterState<T>() where T : PlayerState
        {
            var type = typeof(T);
            if (!cachedStates.ContainsKey(type))
                cachedStates[type] = (T)Activator.CreateInstance(type, owner);
        }

        public void RegisterCondition(ConditionType type, Func<bool> condition)
        {
            if (!conditions.ContainsKey(type))
                conditions[type] = condition;
        }

        public void RegisterTransition<TFrom, TTo>(ConditionType cond)
            where TFrom : PlayerState
            where TTo : PlayerState
        {
            var from = typeof(TFrom);
            if (!transitions.ContainsKey(from))
                transitions[from] = new();
            transitions[from].Add(new Transition(() => Get<TTo>(), () => conditions.TryGetValue(cond, out var c) && c()));
        }

        private T Get<T>() where T : PlayerState => (T)cachedStates[typeof(T)];

        public void SetState<T>() where T : PlayerState
        {
            SetState(Get<T>());
        }

        public void SetState(PlayerState state)
        {
            currentState?.OnExit();
            currentState = state;
            currentTransitions = transitions.TryGetValue(state.GetType(), out var found) ? found : emptyTransitions;
            currentState?.OnEnter();
        }

        private readonly Dictionary<ConditionType, bool> runtimeFlags = new();

        public void SetCondition(ConditionType type, bool value)
        {
            runtimeFlags[type] = value;
        }


        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
            {
                SetState(transition.To());
                runtimeFlags.Clear();
            }

            currentState?.OnUpdate();
        }


        private Transition GetTransition()
        {
            foreach (var t in currentTransitions)
                if (t.Condition()) return t;
            return null;
        }

        public void RegisterAllStates()
        {
            // Register các State
            RegisterState<IdleState>();
            RegisterState<BlockingState>();
            RegisterState<KnockoutState>();

            RegisterState<PunchHeadState>();
            RegisterState<PunchStomachState>();
            RegisterState<PunchKidneyState>();

            RegisterState<HitStunHeadState>();
            RegisterState<HitStunStomachState>();
            RegisterState<HitStunKidneyState>();

        }

        public void RegisterAllConditions()
        {
            RegisterCondition(ConditionType.StateFinished, () => CurrentState?.IsFinished ?? false);
            RegisterCondition(ConditionType.IsBlock, () => owner.InputQueue.PeekAndConsume(ConditionType.IsBlock));
            RegisterCondition(ConditionType.IsKO, () => !owner.IsAlive);
            RegisterCondition(ConditionType.IsPunchHead, () => owner.InputQueue.PeekAndConsume(ConditionType.IsPunchHead));
            RegisterCondition(ConditionType.IsPunchKidney, () => owner.InputQueue.PeekAndConsume(ConditionType.IsPunchKidney));
            RegisterCondition(ConditionType.IsPunchStomach, () => owner.InputQueue.PeekAndConsume(ConditionType.IsPunchStomach));
            RegisterCondition(ConditionType.IsStunHead, () => runtimeFlags.TryGetValue(ConditionType.IsStunHead, out var v) && v);
            RegisterCondition(ConditionType.IsStunKidney, () => runtimeFlags.TryGetValue(ConditionType.IsStunKidney, out var v) && v);
            RegisterCondition(ConditionType.IsStunStomach, () => runtimeFlags.TryGetValue(ConditionType.IsStunStomach, out var v) && v);
        }

        public void RegisterAllTransitions()
        {

            RegisterTransition<IdleState, BlockingState>(ConditionType.IsBlock);
            RegisterTransition<IdleState, KnockoutState>(ConditionType.IsKO);
            RegisterTransition<IdleState, PunchHeadState>(ConditionType.IsPunchHead);
            RegisterTransition<IdleState, PunchKidneyState>(ConditionType.IsPunchKidney);
            RegisterTransition<IdleState, PunchStomachState>(ConditionType.IsPunchStomach);
            RegisterTransition<IdleState, HitStunHeadState>(ConditionType.IsStunHead);
            RegisterTransition<IdleState, HitStunKidneyState>(ConditionType.IsStunKidney);
            RegisterTransition<IdleState, HitStunStomachState>(ConditionType.IsStunStomach);

            RegisterTransition<PunchHeadState, IdleState>(ConditionType.StateFinished);
            RegisterTransition<PunchKidneyState, IdleState>(ConditionType.StateFinished);
            RegisterTransition<PunchStomachState, IdleState>(ConditionType.StateFinished);
            RegisterTransition<BlockingState, IdleState>(ConditionType.StateFinished);
            RegisterTransition<HitStunHeadState, IdleState>(ConditionType.StateFinished);
            RegisterTransition<HitStunKidneyState, IdleState>(ConditionType.StateFinished);
            RegisterTransition<HitStunStomachState, IdleState>(ConditionType.StateFinished);


            RegisterTransition<PunchHeadState, HitStunHeadState>(ConditionType.IsStunHead);
            RegisterTransition<PunchKidneyState, HitStunHeadState>(ConditionType.IsStunHead);
            RegisterTransition<PunchStomachState, HitStunHeadState>(ConditionType.IsStunHead);
            RegisterTransition<PunchHeadState, HitStunKidneyState>(ConditionType.IsStunKidney);
            RegisterTransition<PunchKidneyState, HitStunKidneyState>(ConditionType.IsStunKidney);
            RegisterTransition<PunchStomachState, HitStunKidneyState>(ConditionType.IsStunKidney);
            RegisterTransition<PunchHeadState, HitStunStomachState>(ConditionType.IsStunStomach);
            RegisterTransition<PunchKidneyState, HitStunStomachState>(ConditionType.IsStunStomach);
            RegisterTransition<PunchStomachState, HitStunStomachState>(ConditionType.IsStunStomach);

        }

        private class Transition
        {
            public Func<PlayerState> To;
            public Func<bool> Condition;

            public Transition(Func<PlayerState> to, Func<bool> condition)
            {
                To = to;
                Condition = condition;
            }
        }
    }

    // === ConditionType.cs ===
    public enum ConditionType
    {
        IsPunchHead,
        IsPunchKidney,
        IsPunchStomach,
        IsBlock,
        IsKO,
        IsStunHead,
        IsStunKidney,
        IsStunStomach,
        HasNoMana,
        StateFinished
    }
}
