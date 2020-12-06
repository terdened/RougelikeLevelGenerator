using Assets.Common.AI;
using UnityEngine;

namespace Assets.LevelExplore.AI.Drone01AI
{
    [RequireComponent(typeof(EnemyController))]
    public class Drone01AI : MonoBehaviour
    {
        private StateMachineAI _stateMachineAI;

        public string State;

        public void Start()
        {
            var enemyController = GetComponent<EnemyController>();

            _stateMachineAI = new StateMachineAI();

            var patrolLeftState = new PatrolLeftState(transform.gameObject, enemyController);
            var patrolRightState = new PatrolRightState(transform.gameObject, enemyController);
            var standLeftState = new StandLeftState(transform.gameObject, enemyController);
            var standRightState = new StandRightState(transform.gameObject, enemyController);
            var attackState = new AttackState(transform.gameObject, enemyController);

            _stateMachineAI.States.Add(patrolLeftState);
            _stateMachineAI.States.Add(patrolRightState);
            _stateMachineAI.States.Add(standLeftState);
            _stateMachineAI.States.Add(standRightState);
            _stateMachineAI.States.Add(attackState);

            _stateMachineAI.SetCurrentState("PatrolLeft");
        }

        public void Update()
        {
            if (_stateMachineAI == null)
                return;

            _stateMachineAI.Update();

            State = _stateMachineAI.StateName;
        }
    }
}
