using UnityEngine;

public class ElevatorController : MonoBehaviour
{

    public float Speed;
    public Vector2 PointA;
    public Vector2 PointB;
    public Vector2 CurrentVelocity;

    private bool _fromAtoB;

    // Use this for initialization
    void Start()
    {
    }

    public void Init(LevelElevator elevator)
    {
        PointA = elevator.Points.pointA;
        PointB = elevator.Points.pointB;

        transform.position = PointA;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PointA == Vector2.zero || PointB == Vector2.zero)
            return;

        if (Vector2.Distance(transform.position, PointA) < 0.01f)
        {
            _fromAtoB = true;
        }

        if (Vector2.Distance(transform.position, PointB) < 0.01f)
        {
            _fromAtoB = false;
        }

        var newPosition = Vector2.MoveTowards(transform.position,
            _fromAtoB ? PointB : PointA, Speed * Time.deltaTime);

        CurrentVelocity = newPosition - (Vector2)transform.position;

        //var deltaPosition = newPosition - (Vector2)transform.position;

        //if (!_fromAtoB && Vector2.Distance(newPosition, PointA) < deltaPosition.magnitude + 0.01f)
        //{
        //    CurrentVelocity = Vector2.zero;
        //} 
        //else if (_fromAtoB && Vector2.Distance(newPosition, PointB) < deltaPosition.magnitude + 0.01f)
        //{
        //    CurrentVelocity = Vector2.zero;
        //} 
        //else
        //{
        //    CurrentVelocity = newPosition - (Vector2)transform.position;
        //}

        transform.position = newPosition;
    }
}
