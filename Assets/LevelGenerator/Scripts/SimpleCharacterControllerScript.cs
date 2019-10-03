using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerMotor2D))]
public class SimpleCharacterControllerScript : MonoBehaviour
{
    private PlatformerMotor2D _platformerMotor2D;
    private float _originalGroundSpeed;
    private float _originalAirSpeed;
    private float _originalSlopeAngle;

    public float RunMultiplier = 1.5f;
    public float SlopeAngle = 30f;

    void Start()
    {
        _platformerMotor2D = GetComponent<PlatformerMotor2D>();
        _originalGroundSpeed = _platformerMotor2D.groundSpeed;
        _originalAirSpeed = _platformerMotor2D.airSpeed;
        _originalSlopeAngle = _platformerMotor2D.angleAllowedForMoving;
    }

    void Update()
    {
        if (Input.GetButtonDown("Run"))
        {
            _platformerMotor2D.groundSpeed = _originalGroundSpeed * RunMultiplier;
            _platformerMotor2D.airSpeed = _originalAirSpeed * RunMultiplier;
        }

        if (Input.GetButtonUp("Run"))
        {
            _platformerMotor2D.groundSpeed = _originalGroundSpeed * RunMultiplier;
            _platformerMotor2D.airSpeed = _originalAirSpeed * RunMultiplier;
        }

        if (Input.GetButtonDown("Crawl"))
        {
            _platformerMotor2D.angleAllowedForMoving = SlopeAngle;
        }

        if (Input.GetButtonUp("Crawl"))
        {
            _platformerMotor2D.angleAllowedForMoving = _originalSlopeAngle;
        }
    }
}
