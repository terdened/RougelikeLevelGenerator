using Assets.Common.AI;
using System;
using System.Collections.Generic;

namespace Assets.LevelExplore.AI.Drone01AI
{
    public class StandLeftState : IState
    {
        public List<BaseTransition> Transitions { get; set; }

        public string Name => "StandLeft";

        private EnemyController _enemyController;

        public StandLeftState(EnemyController enemyController)
        {
            _enemyController = enemyController;

            Transitions = new List<BaseTransition>();
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
        }
    }
}
