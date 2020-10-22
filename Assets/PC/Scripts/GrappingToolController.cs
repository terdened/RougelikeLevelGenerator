using UnityEngine;

namespace Assets.PC.Scripts
{
    class GrappingToolController : MonoBehaviour
    {
        public GameObject LineRendererPrefab;

        private Vector2 _mousePosition;
        private SpringJoint2D _joint;
        private GameObject _lineRenderer;

        private void Update()
        {
            HandleMousePosition();
            HandleUp();

            if (Input.GetMouseButtonDown(1))
            {
                Grap();
            }

            if (Input.GetMouseButtonUp(1))
            {
                Release();
            }
            UpdateLineRenderer();
        }

        private void Grap()
        {
            if (_joint != null)
                return;

            transform.gameObject.AddComponent(typeof(SpringJoint2D));

            _joint = GetComponent<SpringJoint2D>();
            _joint.connectedAnchor = _mousePosition;
            _joint.dampingRatio = 1f;

            CreateLine();
        }

        private void Release()
        {
            if (_joint == null)
                return;

            Destroy(_joint);
            _joint = null;
            
            DeleteLine();
        }

        private void CreateLine()
        {
            _lineRenderer = Instantiate(LineRendererPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);

            var lineRenderer = _lineRenderer.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material.color = Color.red;
        }

        private void DeleteLine()
        {
            Destroy(_lineRenderer);
            _lineRenderer = null;
        }

        private void UpdateLineRenderer()
        {
            if (_lineRenderer == null || _joint == null)
                return;

            var lineRenderer = _lineRenderer.GetComponent<LineRenderer>();

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, _joint.connectedAnchor);
        }

        private void HandleMousePosition()
        {
            _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void HandleUp()
        {
            if (_joint == null)
                return;

            if(Input.GetMouseButton(1))
            {
                _joint.distance -= 0.1f;
            }
        }
    }
}
