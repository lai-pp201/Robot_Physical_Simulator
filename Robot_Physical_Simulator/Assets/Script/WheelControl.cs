using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    [SerializeField] private float _motorSpeed = 5;

    [SerializeField] private WheelCollider _wheelLeft;
    [SerializeField] private WheelCollider _wheelRight;
    public Transform leftWheelMesh;
    public Transform rightWheelMesh;
    float leftMotor;
    float rightMotor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            leftMotor = _motorSpeed;
            rightMotor = _motorSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            leftMotor = -_motorSpeed;
            rightMotor = -_motorSpeed;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            leftMotor = -_motorSpeed;
            rightMotor = _motorSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            leftMotor = _motorSpeed;
            rightMotor = -_motorSpeed;
        }
        else
        {
            leftMotor = 0;
            rightMotor = 0;
        }
            //float leftMotor = (Input.GetKey(KeyCode.A)) ? _motorSpeed : 0;
            //float rightMotor = (Input.GetKey(KeyCode.D)) ? _motorSpeed : 0;

            _wheelLeft.motorTorque = leftMotor;
        _wheelRight.motorTorque = rightMotor;

        //_wheelLeft.GetWorldPose(out Vector3 posL, out Quaternion rotL);
        //leftWheelMesh.position = posL;
        //leftWheelMesh.rotation = rotL;

        //_wheelRight.GetWorldPose(out Vector3 posR, out Quaternion rotR);
        //rightWheelMesh.position = posL;
        //rightWheelMesh.rotation = rotL;

    }
}
