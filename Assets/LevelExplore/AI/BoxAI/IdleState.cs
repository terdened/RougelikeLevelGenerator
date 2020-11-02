using Assets.Common.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.LevelExplore.AI.BoxAI
{
    public class IdleState : IState
    {
        public List<BaseTransition> Transitions { get; set; }

        private IdleToWarnTransition _idleToWarnTransition;

        public string Name => "Idle";

        public IdleState(GameObject self)
        {
            Transitions = new List<BaseTransition>();

            _idleToWarnTransition = new IdleToWarnTransition(self);
            Transitions.Add(_idleToWarnTransition);
        }

        public void HandleState() { }

        public void OnEnter()
        {
            _idleToWarnTransition.Init();
        }

        public void OnExit()
        {
            _idleToWarnTransition.Clear();
        }
    }
}
