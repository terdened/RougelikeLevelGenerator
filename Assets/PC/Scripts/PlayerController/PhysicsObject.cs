using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PhysicsObject : MonoBehaviour
{
    public float minGroundNormalY = .65f;
    public float gravityModifier = 1f;

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected bool groundedOneDirected;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected BoxCollider2D collider;


    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {

    }

    void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;
        groundedOneDirected = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;


                var platformEffector = hitBufferList[i].transform.GetComponent<PlatformEffector2D>();

                if (platformEffector != null && platformEffector.useOneWay)
                {
                    Debug.DrawLine(hitBufferList[i].point, hitBufferList[i].point + currentNormal);

                    if (!yMovement)
                    {
                        continue;
                    }
                        
                    var deltaAngle = ((Vector2)platformEffector.transform.position).GetAngle((Vector2)platformEffector.transform.position + move);

                    var playerBottomPosition = (Vector2)transform.position + collider.offset + Vector2.down * (collider.size.y/2);

                    var platformCollider = platformEffector.transform.GetComponent<BoxCollider2D>();

                    var platformTopPosition = (Vector2)platformEffector.transform.position + platformCollider.offset + Vector2.up * (platformEffector.transform.localScale.y * platformCollider.size.y / 2);

                    if (deltaAngle == 90 || playerBottomPosition.y < platformTopPosition.y)
                    {
                        continue;
                    }
                }

                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;

                    if (platformEffector != null)
                    {
                        groundedOneDirected = true;
                    }

                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }


        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }
}
