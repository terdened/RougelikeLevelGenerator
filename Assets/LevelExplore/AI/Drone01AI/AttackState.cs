using Assets.Common.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.LevelExplore.AI.Drone01AI
{
    public class AttackState : IState
    {
        public List<BaseTransition> Transitions { get; set; }

        public string Name => "Attack";


        private EnemyController _enemyController;

        public AttackState(GameObject self, EnemyController enemyController)
        {
            _enemyController = enemyController;

            Transitions = new List<BaseTransition>();

            var standLeftToPatrolRightTransition = new AttackToPatrolRightTransition(self);
            Transitions.Add(standLeftToPatrolRightTransition);
        }

        public void HandleState()
        {
        }

        public void OnEnter()
        {
            _enemyController.IsAttacking = true;
        }

        public void OnExit()
        {
            _enemyController.IsAttacking = false;
            Transitions.ForEach(_ => _.Clear());
        }
    }
}
