using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterControllerScript : MonoBehaviour
{
    Rigidbody2D rigidbody2;

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;

    public float maxVelocity = 10.0f;

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        moveDirection *= speed;

        if (Input.GetButton("Jump"))
        {
            moveDirection.y = jumpSpeed;
        }

        if (Mathf.Abs(rigidbody2.velocity.x) < maxVelocity && rigidbody2.velocity.y < maxVelocity)
        {
            rigidbody2.AddForce(moveDirection * Time.deltaTime);
        }
    }
}
