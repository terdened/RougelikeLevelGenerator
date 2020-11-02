using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Common.AI
{
    public class StateMachineAI
    {
        public List<IState> States { get; set; }

        public StateMachineAI()
        {
            States = new List<IState>();
        }

        public string StateName => _currentState?.Name ?? "Untitled";

        private IState _currentState;

        public void Update()
        {
            CheckTransitions();

            _currentState.HandleState();
        }

        public void SetCurrentState(string name)
        {
            var stateWithName = States.FirstOrDefault(_ => _.Name == name);

            if (stateWithName != null)
            {
                if(_currentState != null)
                    _currentState.OnExit();

                _currentState = stateWithName;
                _currentState.OnEnter();
            }
        }

        private void CheckTransitions()
        {
            if (_currentState == null || _currentState.Transitions == null)
                return;

            foreach (var transition in _currentState.Transitions)
            {
                var targetState = transition.Check();

                if (targetState != null)
                {
                    SetCurrentState(targetState);
                    break;
                } 
            }
        }
    }
}
