using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    private MovingPlatformMotor2D _mpMotor;

    public float Speed;
    private LevelElevator _elevator;
    private bool _fromAtoB;

    // Use this for initialization
    void Start()
    {
    }

    public void Init(LevelElevator elevator)
    {
        _mpMotor = GetComponent<MovingPlatformMotor2D>();
        _mpMotor.velocity = Vector2.zero;
        _elevator = elevator;
        _mpMotor.position = elevator.Points.pointA;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var platformPosition = _mpMotor.position;

        if (platformPosition == _elevator.Points.pointA)
        {
            _fromAtoB = true;
        }

        if (platformPosition == _elevator.Points.pointB)
        {
            _fromAtoB = false;
        }

        var newPosition = Vector2.MoveTowards(_mpMotor.position,
            _fromAtoB ? _elevator.Points.pointB : _elevator.Points.pointA, Speed * Time.deltaTime);
        var newVelocity = newPosition - _mpMotor.position;

        _mpMotor.velocity = newVelocity;
        _mpMotor.transform.position = newPosition;
    }
}
