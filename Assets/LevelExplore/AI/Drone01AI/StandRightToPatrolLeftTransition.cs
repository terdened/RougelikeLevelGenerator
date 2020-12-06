using Assets.Common.AI;
using UnityEngine;

namespace Assets.LevelExplore.AI.Drone01AI
{
    public class StandRightToPatrolLeftTransition : BaseTransition
    {
        public int Duration = 500;

        protected override string TargetState => "PatrolLeft";

        private int _counter = 0;

        public StandRightToPatrolLeftTransition(GameObject self) : base(self)
        {

        }

        protected override bool IsTriggered()
        {
            _counter++;

            return _counter >= Duration;
        }

        public override void Clear()
        {
            _counter = 0;
        }
    }
}
