using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Move : MonoBehaviour
{
    public GameObject goal;
    public GameObject face;
    public GameObject user;
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
            //goal = GameObject.Find("GoalPos");
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
            Vector2 RU = new Vector2(this.transform.position.x - user.transform.position.x, this.transform.position.z - user.transform.position.z);
            RU = RU.normalized;
            Vector2 goalpos = new Vector2(user.transform.position.x + RU.x * Mathf.Cos(Mathf.PI / 4) - RU.y * Mathf.Sin(Mathf.PI / 4), user.transform.position.z + RU.x * Mathf.Sin(Mathf.PI / 4) + RU.y * Mathf.Cos(Mathf.PI / 4));
            //Mathf.Cos(45);

            Debug.Log(Mathf.Cos(Mathf.PI/4));
            goal.transform.position = new Vector3(goalpos.x, 0f, goalpos.y);
            ToPos(new Vector3(goalpos.x,0f,goalpos.y));
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
            toOther.Normalize();
            Vector2 faceTo = new Vector2(face.transform.position.x - this.transform.position.x, face.transform.position.z - this.transform.position.z);
            faceTo.Normalize();
            float dot = toOther.x * faceTo.x + toOther.y * faceTo.y;
            float cross = toOther.x * faceTo.y - toOther.y * faceTo.x;
            saving.check.Add(Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), target - new Vector3(0, target.y, 0)));
            //Debug.Log(Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), target - new Vector3(0, target.y, 0)));
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
