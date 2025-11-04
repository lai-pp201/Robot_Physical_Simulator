using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public GameObject goal;
    public GameObject face;
    public float leftWheelSpeed;
    public float rightWheelSpeed;
    bool move = false;
    float Speed = 5f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            move = move ? false : true;
        }
        ToPos(goal.transform.position);
    }
    public void ToPos(Vector3 target)
    {
        if(move)
        {
            Vector3 toOther = target - (this.transform.position - new Vector3(0, this.transform.position.y, 0));
            toOther.Normalize();
            Vector3 faceTo = face.transform.position - this.transform.position;
            faceTo.Normalize();
            if (Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), target) > 0.05f)
            {
                Debug.Log(Vector3.Dot(faceTo, toOther));
                if (Vector3.Dot(faceTo, toOther) < -0.2f)
                {
                    Debug.Log("OnRight");
                    rightWheelSpeed = Speed;
                    leftWheelSpeed = -Speed;
                }
                else if (Vector3.Dot(faceTo, toOther) > 0.2f)
                {
                    Debug.Log("OnLeft");
                    rightWheelSpeed = -Speed;
                    leftWheelSpeed = Speed;
                }
                else
                {
                    Debug.Log("Front");
                    rightWheelSpeed = Speed;
                    leftWheelSpeed = Speed;
                }
            }
            else
            {
                rightWheelSpeed = 0;
                leftWheelSpeed = 0;
            }
        }
    }
}
