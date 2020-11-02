using Assets.Common.AI;
using UnityEngine;

namespace Assets.LevelExplore.AI.RoomAI
{
    public class FightToCompleteTransition : BaseTransition
    {
        protected override string TargetState => "Complete";

        public FightToCompleteTransition(GameObject self): base(self)
        {

        }

        protected override bool IsTriggered()
        {
            int maxColliders = 10;
            Collider2D[] hitColliders = new Collider2D[maxColliders];
            var numColliders = Physics2D.OverlapBoxNonAlloc(Self.transform.position, new Vector2(15f, 10f), 0f, hitColliders);

            var isBoxAlive = false;

            for (int i = 0; i < numColliders; i++)
            {
                var boxAI = hitColliders[i].transform.GetComponentInChildren<BoxAI.BoxAI>();

                if (boxAI != null)
                {
                    isBoxAlive = true;
                }
            }

            return !isBoxAlive;
        }
    }
}
