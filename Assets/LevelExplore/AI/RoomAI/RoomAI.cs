using Assets.Common.AI;
using System;
using UnityEngine;

namespace Assets.LevelExplore.AI.RoomAI
{
    [RequireComponent(typeof(RoomController))]
    public class RoomAI : MonoBehaviour
    {
        private StateMachineAI _stateMachineAI;

        public string State;

        public void Start()
        {
            var roomController = GetComponent<RoomController>();

            _stateMachineAI = new StateMachineAI();

            var waitState = new WaitState(transform.gameObject);
            var fightState = new FightState(transform.gameObject, roomController);
            var completeState = new CompleteState();

            _stateMachineAI.States.Add(waitState);
            _stateMachineAI.States.Add(fightState);
            _stateMachineAI.States.Add(completeState);

            _stateMachineAI.SetCurrentState("Wait");
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
