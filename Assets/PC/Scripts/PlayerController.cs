using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float JumpForce = 700f;
    public float MoveForce = 10f;
    
    [Range(0, .3f)]
    [SerializeField]
    private float m_MovementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;

    private Rigidbody2D _rigidbody2D;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        Move();
    }

    void Jump()
    {
        var vector = Vector2.up * JumpForce;
        _rigidbody2D.AddForce(vector);
    }

    void Move()
    {
        var move = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            move = -MoveForce;
        }

        if (Input.GetKey(KeyCode.D))
        {
            move = MoveForce;
        }

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, _rigidbody2D.velocity.y);
        // And then smoothing it out and applying it to the character
        _rigidbody2D.velocity = Vector3.SmoothDamp(_rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

    }
}
