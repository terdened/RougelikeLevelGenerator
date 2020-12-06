using Assets.Common.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.LevelExplore.AI.Drone01AI
{
    public class StandRightState : IState
    {
        public List<BaseTransition> Transitions { get; set; }

        public string Name => "StandRight";


        private EnemyController _enemyController;

        public StandRightState(GameObject self, EnemyController enemyController)
        {
            _enemyController = enemyController;

            Transitions = new List<BaseTransition>();

            var standRightToPatrolLeftTransition = new StandRightToPatrolLeftTransition(self);
            Transitions.Add(standRightToPatrolLeftTransition);
        }

        public void HandleState()
        {
        }

        public void OnEnter()
        {
            _enemyController.HorizontalMovement = 0;
        }

        public void OnExit()
        {
            Transitions.ForEach(_ => _.Clear());
        }
    }
}
