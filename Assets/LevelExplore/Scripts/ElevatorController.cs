using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public GameObject ElevatorPlatform;
    public float Speed;

    private LevelElevator _elevator;
    private bool _fromAtoB;

    public void Init(LevelElevator elevator)
    {
        _elevator = elevator;
        transform.position = new Vector3(elevator.Points.pointA.x, elevator.Points.pointA.y, 0);
    }
    
    void Update()
    {
        if(ElevatorPlatform == null)
            return;

        var platformPosition = new Vector2(ElevatorPlatform.transform.position.x, ElevatorPlatform.transform.position.y);

        if (platformPosition == _elevator.Points.pointA)
        {
            _fromAtoB = true;
        }

        if (platformPosition == _elevator.Points.pointB)
        {
            _fromAtoB = false;
        }

        var newPosition = Vector2.MoveTowards(platformPosition,
            _fromAtoB ? _elevator.Points.pointB : _elevator.Points.pointA, Speed * Time.deltaTime);

        ElevatorPlatform.transform.position = new Vector3(newPosition.x, newPosition.y, 0);
    }
}
