using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    [SerializeField] private float _motorSpeed = 5;

    [SerializeField] private WheelCollider _wheelLeft;
    [SerializeField] private WheelCollider _wheelRight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float leftMotor = (Input.GetKey(KeyCode.A)) ? _motorSpeed : 0;
        float rightMotor = (Input.GetKey(KeyCode.D)) ? _motorSpeed : 0;
        _wheelLeft.motorTorque = leftMotor;
        _wheelRight.motorTorque = rightMotor;
    }
}
