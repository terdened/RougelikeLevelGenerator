using UnityEngine;

namespace Assets.LevelExplore.AI.BoxAI
{
    public class BoxController : MonoBehaviour
    {
        public float JumpForce = 60f;

        private Rigidbody2D _rigidbody2D;

        public void Start()
        {
            _rigidbody2D = transform.parent.gameObject.GetComponent<Rigidbody2D>();

            if (_rigidbody2D == null)
            {
                Debug.LogError("There is no Rigidbody2D in parent");
            }
        }

        public void Jump()
        {
            if (_rigidbody2D == null)
                return;

            var jumpVector = Vector2.up * JumpForce;
            _rigidbody2D.AddForce(jumpVector);
        }
    }
}
