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
    float RTSpeed = 2f;
    float MVSpeed = 5f;
    float acceptError = 0.2f;
    public class Data
    {
        public List<float> check = new List<float>();
    }
    Data saving = new Data();
    private void Start()
    {
        //Physics.IgnoreLayerCollision(20, ~20);
        //Physics.IgnoreLayerCollision(20, 13);
        //Physics.IgnoreLayerCollision(20, 7);
        //Physics.IgnoreLayerCollision(20, 8);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            goal = GameObject.Find("GoalPos");
            move = move ? false : true;
            if (!move)
            {
                SaveData.Save(saving, "12321");
                saving.check.Clear();
                Debug.Log("儲存並清空");
            }
        }
        if (goal != null)
        {
            ToPos(goal.transform.position);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }
    public void ToPos(Vector3 target)
    {
        if (move)
        {
            Vector2 toOther = new Vector2(target.x - this.transform.position.x, target.z - this.transform.position.z);
            //Debug.Log(toOther);
            toOther.Normalize();
            //Debug.Log(toOther);
            Vector2 faceTo = new Vector2(face.transform.position.x - this.transform.position.x, face.transform.position.z - this.transform.position.z);
            faceTo.Normalize();
            float dot = toOther.x * faceTo.x + toOther.y * faceTo.y;
            float cross = toOther.x * faceTo.y - toOther.y * faceTo.x;
            //Debug.Log(faceTo);
            //Debug.Log(Vector2.Dot(faceTo, target));
            Debug.Log(dot);
            Debug.Log(cross);
            //Debug.Log(Vector2.Angle(faceTo, target));
            //Debug.Log(Vector3.Cross(new Vector2(faceTo.x, faceTo.z), new Vector2(target.x, target.z)));
            //Vector2.SignedAngle
            //Debug.Log(Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), target - new Vector3(0,target.y,0)));
            //Debug.Log(Vector3.Dot(faceTo, toOther));
            saving.check.Add(Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), target - new Vector3(0, target.y, 0)));
            if (Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), target - new Vector3(0, target.y, 0)) > 0.1f)
            {
                //Debug.Log(Vector3.Dot(faceTo, toOther));
                if (dot >= 0)
                {
                    if (cross > acceptError)
                    {
                        rightWheelSpeed = -RTSpeed;
                        leftWheelSpeed = RTSpeed;
                    }
                    else if (cross < -acceptError)
                    {
                        rightWheelSpeed = RTSpeed;
                        leftWheelSpeed = -RTSpeed;
                    }
                    else
                    {
                        rightWheelSpeed = MVSpeed;
                        leftWheelSpeed = MVSpeed;
                    }
                }
                else
                {
                    if (cross > acceptError)
                    {
                        rightWheelSpeed = RTSpeed;
                        leftWheelSpeed = -RTSpeed;
                    }
                    else if (cross < -acceptError)
                    {
                        rightWheelSpeed = -RTSpeed;
                        leftWheelSpeed = RTSpeed;
                    }
                    else
                    {
                        rightWheelSpeed = -MVSpeed;
                        leftWheelSpeed = -MVSpeed;
                    }
                }
            }
            else
            {
                rightWheelSpeed = 0;
                leftWheelSpeed = 0;
            }
        }
        else
        {
            rightWheelSpeed = 0;
            leftWheelSpeed = 0;
        }
    }

}
