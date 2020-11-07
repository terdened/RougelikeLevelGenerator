using System;
using UnityEngine;

namespace Assets.PC.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class JumppadController : MonoBehaviour
    {
        public float Force = 1000f;

        void OnTriggerEnter2D(Collider2D other)
        {
            var characterController = other.GetComponent<CharacterController2D>();

            if (characterController != null)
            {
                Debug.Log(transform.eulerAngles.z);
                characterController.JumppadForce = Vector2.up * Force;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            var characterController = other.GetComponent<CharacterController2D>();

            if (characterController != null)
            {
                characterController.JumppadForce = null;
            }
        }
    }
}
