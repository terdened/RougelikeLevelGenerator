using Assets.Common.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.LevelExplore.AI.RoomAI
{
    public class WaitState : IState
    {
        public List<BaseTransition> Transitions { get; set; }

        private WaitToFightTransition _waitToFightTransition;

        public string Name => "Wait";

        public WaitState(GameObject self)
        {
            Transitions = new List<BaseTransition>();

            _waitToFightTransition = new WaitToFightTransition(self);
            Transitions.Add(_waitToFightTransition);
        }

        public void HandleState() { }

        public void OnEnter()
        {
            _waitToFightTransition.Init();
        }

        public void OnExit()
        {
            _waitToFightTransition.Clear();
        }
    }
}
