using Assets.Common.AI;
using UnityEngine;

namespace Assets.LevelExplore.AI.RoomAI
{
    public class WaitToFightTransition : BaseTransition
    {
        public WaitToFightTransition(GameObject self): base(self) 
        {

        }

        protected override string TargetState => "Fight";
        
        protected override bool IsTriggered()
        {
            int maxColliders = 10;
            Collider2D[] hitColliders = new Collider2D[maxColliders];
            var numColliders = Physics2D.OverlapBoxNonAlloc(Self.transform.position, new Vector2(15f, 10f), 0f, hitColliders);

            var isWarn = false;

            for (int i = 0; i < numColliders; i++)
            {
                var boxAI = hitColliders[i].transform.GetComponentInChildren<BoxAI.BoxAI>();

                if(boxAI != null && boxAI.State == "Warn")
                {
                    isWarn = true;
                }
            }

            return isWarn;
        }

        public void Init()
        {

        }

        public override void Clear()
        {

        }
    }
}
