using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Assets.PC.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    class CharacterController2D : MonoBehaviour
    {
        public float JumpForce;
        public float MoveForce;

        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded

        private Rigidbody2D _rigidbody2D;
        private bool _grounded;
        private Vector3 _velocity = Vector3.zero;

        private bool _jumpPressed = false;

        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y - 0.5f), k_GroundedRadius);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    _grounded = true;
                }
            }

            HandleMovement();
            HandleJump();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _jumpPressed = true;
            }
        }

        void HandleJump()
        {
            if (_jumpPressed && _grounded)
            {
                _jumpPressed = false;

                var vector = Vector2.up * JumpForce;
                _rigidbody2D.AddForce(vector);
            }
        }

        private void HandleMovement()
        {
            var move = 0f;

            if (Input.GetKey(KeyCode.A))
            {
                move = -MoveForce;
            }

            if (Input.GetKey(KeyCode.D))
            {
                move = MoveForce;
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, _rigidbody2D.velocity.y);

            var movementSmoothing = _grounded ? 0.05f : 1f;

            // And then smoothing it out and applying it to the character
            _rigidbody2D.velocity = Vector3.SmoothDamp(_rigidbody2D.velocity, targetVelocity, ref _velocity, movementSmoothing);
        }
    }
}
