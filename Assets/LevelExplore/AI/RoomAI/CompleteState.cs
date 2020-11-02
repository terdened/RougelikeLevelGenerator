using Assets.Common.AI;
using System.Collections.Generic;

namespace Assets.LevelExplore.AI.RoomAI
{
    public class CompleteState : IState
    {
        public List<BaseTransition> Transitions { get; set; }

        public string Name => "Complete";

        public CompleteState()
        {
            Transitions = new List<BaseTransition>();
        }

        public void HandleState() { }

        public void OnEnter()
        {

        }

        public void OnExit()
        {

        }
    }
}
