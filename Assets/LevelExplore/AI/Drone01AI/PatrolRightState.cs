using Assets.Common.AI;
using System;
using System.Collections.Generic;

namespace Assets.LevelExplore.AI.Drone01AI
{
    public class PatrolRightState : IState
    {
        public List<BaseTransition> Transitions { get; set; }

        public string Name => "PatrolRight";

        private EnemyController _enemyController;

        public PatrolRightState(EnemyController enemyController)
        {
            _enemyController = enemyController;

            Transitions = new List<BaseTransition>();
        }

        public void HandleState()
        {
        }

        public void OnEnter()
        {
            _enemyController.HorizontalMovement = 1f;
        }

        public void OnExit()
        {
        }
    }
}
