using System;
using UnityEngine;

public class BuletController : MonoBehaviour
{
    public float Speed;
    private float _angle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var vector = new Vector3(Speed * (float) Math.Cos(_angle * 3.14f / 180), Speed * (float) Math.Sin(_angle * 3.14f / 180));
        transform.Translate(vector);
    }

    public void InitAngle(float angle)
    {
        _angle = angle;
    }
}
