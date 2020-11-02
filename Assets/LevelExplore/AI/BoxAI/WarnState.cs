using Assets.Common.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.LevelExplore.AI.BoxAI
{
    public class WarnState : IState
    {
        private int _counter = 300;
        private int _maxCounter = 300;

        private BoxController _boxController;
        private WarnToIdleTransition _warnToIdleTransition;

        public List<BaseTransition> Transitions { get; set; }

        public string Name => "Warn";

        public WarnState(GameObject self, BoxController boxController)
        {
            Transitions = new List<BaseTransition>();
            _warnToIdleTransition = new WarnToIdleTransition(self);
            Transitions.Add(_warnToIdleTransition);

            _boxController = boxController;
        }

        public void HandleState() 
        {
            _counter++;
            
            if (_counter >= _maxCounter)
            {
                _counter = 0;
                _boxController.Jump();
            }
        }

        public void OnEnter()
        {
            _warnToIdleTransition.Init();
        }

        public void OnExit()
        {
            _warnToIdleTransition.Clear();
        }
    }
}
