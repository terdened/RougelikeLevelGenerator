using Assets.Common.AI;
using System.Collections.Generic;

namespace Assets.LevelExplore.AI.Drone01AI
{
    public class StandRightState : IState
    {
        public List<BaseTransition> Transitions { get; set; }

        public string Name => "StandRight";


        private EnemyController _enemyController;

        public StandRightState(EnemyController enemyController)
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
