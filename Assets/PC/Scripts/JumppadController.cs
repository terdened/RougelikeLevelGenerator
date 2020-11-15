using System;
using UnityEngine;

namespace Assets.PC.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class JumppadController : MonoBehaviour
    {
        public float TakeOffSpeed = 20f;

        void OnTriggerEnter2D(Collider2D other)
        {
            var characterController = other.GetComponent<CharacterController>();

            if (characterController != null)
            {
                characterController.JumppadTakeOffSpeed = TakeOffSpeed;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            var characterController = other.GetComponent<CharacterController>();

            if (characterController != null)
            {
                characterController.JumppadTakeOffSpeed = null;
            }
        }
    }
}
