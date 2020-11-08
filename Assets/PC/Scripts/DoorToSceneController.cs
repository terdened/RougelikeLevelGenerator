using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.PC.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class DoorToSceneController : MonoBehaviour
    {
        public string TargetScene = "HubScene";
        public int Index = 0;
        public int IndexTo = 0;
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
                LevelHolder.PortalIndex = IndexTo;
            }
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            var characterController = col.GetComponent<CharacterController2D>();
            if(characterController != null)
            {
                _onPlayerEntered = true;

                if(_spriteRenderer != null)
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
