using Assets.Common.AI;
using UnityEngine;

namespace Assets.LevelExplore.AI.BoxAI
{
    [RequireComponent(typeof(BoxController))]
    public class BoxAI : MonoBehaviour
    {
        private StateMachineAI _stateMachineAI;

        public string State;

        public void Start()
        {
            var boxController = GetComponent<BoxController>();

            _stateMachineAI = new StateMachineAI();

            var idleState = new IdleState(transform.gameObject);
            var warnState = new WarnState(transform.gameObject, boxController);

            _stateMachineAI.States.Add(idleState);
            _stateMachineAI.States.Add(warnState);

            _stateMachineAI.SetCurrentState("Idle");
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
