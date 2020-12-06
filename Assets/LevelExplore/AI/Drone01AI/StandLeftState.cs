﻿using Assets.Common.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.LevelExplore.AI.Drone01AI
{
    public class StandLeftState : IState
    {
        public List<BaseTransition> Transitions { get; set; }

        public string Name => "StandLeft";

        private EnemyController _enemyController;

        public StandLeftState(GameObject self, EnemyController enemyController)
        {
            _enemyController = enemyController;

            Transitions = new List<BaseTransition>();

            var standLeftToAttackTransition = new StandLeftToAttackTransition(self);
            Transitions.Add(standLeftToAttackTransition);
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