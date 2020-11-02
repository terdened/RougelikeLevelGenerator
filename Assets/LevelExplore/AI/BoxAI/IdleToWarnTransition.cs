using Assets.Common.AI;
using UnityEngine;

namespace Assets.LevelExplore.AI.BoxAI
{
    public class IdleToWarnTransition : BaseTransition
    {
        private bool _isTriggered;
        private ColliderTrigger _colliderTrigger;

        protected override string TargetState => "Warn";

        public IdleToWarnTransition(GameObject self) : base(self)
        {
            _colliderTrigger = self.GetComponent<ColliderTrigger>();

            if (_colliderTrigger == null)
            {
                Debug.LogError("There is no ColliderTrigger");
                return;
            }
        }

        protected override bool IsTriggered()
        {
            return _isTriggered;
        }

        private void OnTriggeredHandler(Collider2D col)
        {
            if (col.gameObject.tag == "Player")
                _isTriggered = true;
        }

        public void Init()
        {
            _isTriggered = false;
            _colliderTrigger.OnEnterTriggered += OnTriggeredHandler;
        }

        public override void Clear()
        {
            _isTriggered = false; 

            if(_colliderTrigger != null)
                _colliderTrigger.OnEnterTriggered -= OnTriggeredHandler;
        }
    }
}
