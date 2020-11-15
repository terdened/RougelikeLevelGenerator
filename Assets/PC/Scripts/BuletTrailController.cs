using UnityEngine;

public class BuletTrailController : MonoBehaviour
{
    private float _currentAngle;
    private Vector2 _currentDestVector;
    private float _distanceCounter;
    private bool _destroy;

    private float _buletSpeed = 0.5f;
    private float _maxDistance = 10f;
    private float _ricochetMinAngle = 45f;
    private float _observationError = 0.001f;
    private float _damageValue = 30f;

    void Start()
    {
        this.transform.Translate(Vector3.back);
    }

    void Update()
    {
        if (_distanceCounter >= _maxDistance)
            _destroy = true;

        if (_destroy)
        {
            return;
        }

        if (_maxDistance < _distanceCounter + _buletSpeed)
        {
            transform.Translate(_currentDestVector * (_maxDistance - _distanceCounter));
        }

        // Cast a ray straight down.
        var hit = Physics2D.Raycast(transform.position, _currentDestVector, _buletSpeed, (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Enemy")));

        if (hit.collider != null)
        {
            var hitPoint = hit.point - (_currentDestVector * _observationError);
            transform.SetPositionAndRotation(hitPoint, transform.rotation);
            transform.Translate(Vector3.back);
            _distanceCounter += hit.distance;

            var normalAngle = Vector2.zero.GetAngle(hit.normal);


            var rigidbody2D = hit.transform.GetComponent<Rigidbody2D>();


            var normalAngleDiff = Vector2.Angle(_currentDestVector, -hit.normal);

            if (normalAngleDiff < _ricochetMinAngle)
            {
                _destroy = true;
                var durabilityController = hit.transform.GetComponent<DurabilityController>();
                
                if (durabilityController != null)
                {
                    durabilityController.GatherDamage(_damageValue);
                }

                if (rigidbody2D != null)
                {
                    rigidbody2D.AddForce(_currentDestVector * _damageValue);
                }

                return;
            }

            if (rigidbody2D != null)
            {
                rigidbody2D.AddForce(_currentDestVector * _damageValue * ((90 - normalAngleDiff) / 90));
            }

            _currentDestVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (normalAngle + Vector2.SignedAngle(_currentDestVector, -hit.normal))),
                Mathf.Sin(Mathf.Deg2Rad * (normalAngle + Vector2.SignedAngle(_currentDestVector, -hit.normal))));


            return;
        }

        _distanceCounter += _buletSpeed;
        transform.Translate(_currentDestVector * _buletSpeed);
    }

    public void InitBuletTrail(float currentAngle)
    {
        _currentAngle = currentAngle;
        _currentDestVector = new Vector2(Mathf.Cos(_currentAngle * Mathf.Deg2Rad), Mathf.Sin(_currentAngle * Mathf.Deg2Rad));
    }
}
