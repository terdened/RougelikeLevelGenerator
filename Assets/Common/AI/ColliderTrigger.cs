using UnityEngine;

namespace Assets.Common.AI
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ColliderTrigger : MonoBehaviour
    {
        public delegate void TriggerHandler(Collider2D col);

        public event TriggerHandler OnEnterTriggered;

        public event TriggerHandler OnExitTriggered;

        void OnTriggerEnter2D(Collider2D col)
        {
            OnEnterTriggered?.Invoke(col);
        }

        void OnTriggerExit2D(Collider2D col)
        {
            OnExitTriggered?.Invoke(col);
        }
    }
}
