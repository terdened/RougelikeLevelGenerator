using Assets.Common.AI;
using System;
using UnityEngine;

namespace Assets.LevelExplore.AI.Drone01AI
{
    public class PatrolRightToStandRightTransition : BaseTransition
    {
        public Vector2 FloorRaycastOffset = new Vector2(0.7f, -0.7f);
        public float FloorRaycastLength = 0.5f;

        public Vector2 WallRaycastOffset = new Vector2(0.7f, -0.7f);
        public float WallRaycastLength = 0.3f;


        private RaycastHit2D[] hitBuffer = new RaycastHit2D[16]; 
        protected override string TargetState => "StandRight";

        public PatrolRightToStandRightTransition(GameObject self) : base(self)
        {

        }

        protected override bool IsTriggered()
        {
            // Cast floor
            var floorRaycastStartPosition = (Vector2)Self.transform.position + FloorRaycastOffset;
            Debug.DrawLine(floorRaycastStartPosition, floorRaycastStartPosition + Vector2.down * FloorRaycastLength);

            var hitCount = Physics2D.RaycastNonAlloc(floorRaycastStartPosition, Vector2.down, hitBuffer, FloorRaycastLength, 1 << LayerMask.NameToLayer("Ground"));
            var thereIsFloor = false;

            for (var i = 0; i < hitCount; i++)
            {
                thereIsFloor = true;
            }


            // Cast wall
            var wallRaycastStartPosition = (Vector2)Self.transform.position + WallRaycastOffset;
            Debug.DrawLine(wallRaycastStartPosition, wallRaycastStartPosition + Vector2.right * WallRaycastLength);

            hitCount = Physics2D.RaycastNonAlloc(wallRaycastStartPosition, Vector2.right, hitBuffer, FloorRaycastLength, 1 << LayerMask.NameToLayer("Ground"));
            var thereIsWall = false;

            for (var i = 0; i < hitCount; i++)
            {
                thereIsWall = true;
            }

            return thereIsWall || !thereIsFloor;
        }
    }
}
