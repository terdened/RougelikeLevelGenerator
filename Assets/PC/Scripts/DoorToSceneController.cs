using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.PC.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class DoorToSceneController : MonoBehaviour
    {
        public string TargetScene = "HubScene";
        private bool _onPlayerEntered = false;
        private SpriteRenderer _spriteRenderer;

        public void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && _onPlayerEntered)
            {
                SceneManager.LoadScene(TargetScene, LoadSceneMode.Single);
            }
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            var characterController = col.GetComponent<CharacterController2D>();
            if(characterController != null)
            {
                _onPlayerEntered = true;
                _spriteRenderer.color = new Color(0.74f, 0.55f, 0.37f);
            }
        }

        void OnTriggerExit2D(Collider2D col)
        {
            var characterController = col.GetComponent<CharacterController2D>();
            if (characterController != null)
            {
                _onPlayerEntered = false;
                _spriteRenderer.color = Color.white;
            }
        }
    }
}
