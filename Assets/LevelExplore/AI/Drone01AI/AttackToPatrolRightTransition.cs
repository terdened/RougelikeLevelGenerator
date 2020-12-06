using Assets.Common.AI;
using UnityEngine;

namespace Assets.LevelExplore.AI.Drone01AI
{
    public class AttackToPatrolRightTransition : BaseTransition
    {
        public int Duration = 250;

        protected override string TargetState => "PatrolRight";

        private int _counter = 0;

        public AttackToPatrolRightTransition(GameObject self) : base(self)
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
