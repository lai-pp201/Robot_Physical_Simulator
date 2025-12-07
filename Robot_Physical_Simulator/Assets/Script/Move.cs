using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Move : MonoBehaviour
{
    public GameObject goal;
    public GameObject face;
    public GameObject user;
    public float leftWheelSpeed;
    public float rightWheelSpeed;
    public float Robotd = 0.5f;
    bool move = false;
    float RTSpeed = 3f;
    float MVSpeed = 7f;
    float MVadd = 0.0f;
    float RTadd = 0.0f;
    float speedPoint1 = 0.3f;
    float speedPoint2 = 1.5f;
    float rtPoint1 = 0.25f;
    float safeDis = 0.6f;
    float acceptError = 0.2f;
    float acceptPosError = 0.1f;
    float STTime = 0f;
    float pointTime = 0;
    float pointStopDuration = 0.04f;
    float pointMoveDuration = 0.3f;
    bool isPointZero = false;
    bool isPointMove = false;
    public class Data
    {
        public List<float> Time = new List<float>();
        public List<float> check = new List<float>();
        public List<Vector2> Position = new List<Vector2>();
        public List<Vector2> FaceTo = new List<Vector2>();
        public List<float> Foward = new List<float>(); //1直走 0停下 -1旋轉
        public List<float> AngleCross = new List<float>();
        public List<float> AngleDot = new List<float>();
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
                SaveData.Save(saving, "monitor_newmove");
                saving.check.Clear();
                saving.AngleCross.Clear();
                saving.Foward.Clear();
                saving.Position.Clear();
                saving.AngleDot.Clear();
                saving.FaceTo.Clear();
                saving.Time.Clear();
                Debug.Log("儲存並清空");
            }
            else
            {
                STTime = Time.time;
                pointTime = Time.time;
            }
        }
        if (goal != null)
        {
            Vector2 RU = new Vector2(this.transform.position.x - user.transform.position.x, this.transform.position.z - user.transform.position.z);
            RU = RU.normalized;
            Vector2 goalpos = new Vector2(user.transform.position.x + RU.x * Mathf.Cos(Mathf.PI / 4) - RU.y * Mathf.Sin(Mathf.PI / 4), user.transform.position.z + RU.x * Mathf.Sin(Mathf.PI / 4) + RU.y * Mathf.Cos(Mathf.PI / 4));
            //Mathf.Cos(45);

            //Debug.Log(Mathf.Cos(Mathf.PI/4));
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
        Vector2 userVec2 = new Vector2(user.transform.position.x, user.transform.position.z);
        float d = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), userVec2);
        float sinTheta = safeDis / d;               // r / d
        float thetaRad = Mathf.Asin(sinTheta);     // 弧度
        float thetaDeg = thetaRad * Mathf.Rad2Deg;
        float ang = Vector2.Angle(new Vector2(user.transform.position.x - this.transform.position.x, user.transform.position.z - this.transform.position.z), new Vector2(goal.transform.position.x - this.transform.position.x, goal.transform.position.z - this.transform.position.z));
        Vector2 ut = new Vector2(this.transform.position.x - user.transform.position.x, this.transform.position.z - user.transform.position.z);
        Vector2 gt = new Vector2(this.transform.position.x - goal.transform.position.x, this.transform.position.z - goal.transform.position.z);
        ut = ut.normalized;
        gt = gt.normalized;
        float angcross = ut.x * gt.y - ut.y * gt.x; //計算在左側或右側
        bool checkarive = false;
        Vector2 n = new Vector2(-ut.y, ut.x);  //向量旋轉
        float a = (safeDis * safeDis) / d; //係數
        float h = Mathf.Sqrt(safeDis * safeDis - a * a); //係數
        Vector2 t1 = userVec2 + ut * a + n * h; //切點一
        Vector2 t2 = userVec2 + ut * a - n * h; //切點二
        t1 = t1 + t1 - userVec2;
        t2 = t2 + t2 - userVec2;
        //Debug.Log(angcross);
        if (d <= safeDis)
        {
            checkarive = true;
        }
        else if (thetaDeg > ang)
        {
            if (angcross <= 0)
            {
                target = new Vector3(t1.x, 0f, t1.y);
            }
            else
            {
                target = new Vector3(t2.x, 0f, t2.y);
            }
        }
        #region move
        if (move)
        {
            if (checkarive)
            {
                if (Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), target - new Vector3(0, target.y, 0)) > acceptPosError)
                {
                    target = new Vector3(2 * this.transform.position.x - user.transform.position.x, 0f, 2 * this.transform.position.z - user.transform.position.z);
                }
            }

            Vector2 toOther = new Vector2(target.x - this.transform.position.x, target.z - this.transform.position.z);
            toOther.Normalize();
            Vector2 faceTo = new Vector2(face.transform.position.x - this.transform.position.x, face.transform.position.z - this.transform.position.z);
            faceTo.Normalize();
            float dot = toOther.x * faceTo.x + toOther.y * faceTo.y;
            float cross = toOther.x * faceTo.y - toOther.y * faceTo.x;
            saving.Time.Add(Time.time - STTime);
            saving.AngleCross.Add(cross);
            saving.AngleDot.Add(dot);
            saving.FaceTo.Add(faceTo);
            saving.Position.Add(new Vector2(this.transform.position.x, this.transform.position.z));
            saving.check.Add(Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), user.transform.position - new Vector3(0, user.transform.position.y, 0)));
            if (Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), target - new Vector3(0, target.y, 0)) > acceptPosError)
            {
                if (dot >= 0)
                {
                    if (cross > acceptError)
                    {
                        saving.Foward.Add(-1);
                        if (cross > rtPoint1)
                        {
                            isPointMove = false;
                            rightWheelSpeed = -(RTSpeed + RTadd);
                            leftWheelSpeed = RTSpeed + RTadd;
                        }
                        else
                        {
                            isPointMove = true;
                            rightWheelSpeed = -RTSpeed;
                            leftWheelSpeed = RTSpeed;
                        }
                    }
                    else if (cross < -acceptError)
                    {
                        saving.Foward.Add(-1);
                        if (cross < -rtPoint1)
                        {
                            isPointMove = false;
                            rightWheelSpeed = (RTSpeed + RTadd);
                            leftWheelSpeed = -(RTSpeed + RTadd);
                        }
                        else
                        {
                            isPointMove = true;
                            rightWheelSpeed = RTSpeed;
                            leftWheelSpeed = -RTSpeed;
                        }
                    }
                    else
                    {
                        saving.Foward.Add(1);
                        if (Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), target - new Vector3(0, target.y, 0)) > speedPoint2)
                        {
                            isPointMove = false;
                            rightWheelSpeed = (MVSpeed + MVadd);
                            leftWheelSpeed = MVSpeed + MVadd;
                        }
                        else if (Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), target - new Vector3(0, target.y, 0)) < speedPoint1)
                        {
                            isPointMove = true;
                            rightWheelSpeed = (MVSpeed - MVadd);
                            leftWheelSpeed = MVSpeed - MVadd;
                        }
                        else
                        {
                            isPointMove = false;
                            rightWheelSpeed = MVSpeed;
                            leftWheelSpeed = MVSpeed;
                        }

                    }
                }
                else
                {
                    if (cross > acceptError)
                    {
                        saving.Foward.Add(-1);
                        if (cross > rtPoint1)
                        {
                            isPointMove = false;
                            rightWheelSpeed = (RTSpeed + RTadd);
                            leftWheelSpeed = -(RTSpeed + RTadd);
                        }
                        else
                        {
                            isPointMove = true;
                            rightWheelSpeed = RTSpeed;
                            leftWheelSpeed = -RTSpeed;
                        }

                    }
                    else if (cross < -acceptError)
                    {
                        saving.Foward.Add(-1);
                        if (cross < -rtPoint1)
                        {
                            isPointMove = false;
                            rightWheelSpeed = -(RTSpeed + RTadd);
                            leftWheelSpeed = RTSpeed + RTadd;
                        }
                        else
                        {
                            isPointMove = true;
                            rightWheelSpeed = -RTSpeed;
                            leftWheelSpeed = RTSpeed;
                        }
                    }
                    else
                    {
                        saving.Foward.Add(1);
                        if (Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), target - new Vector3(0, target.y, 0)) > speedPoint2)
                        {
                            isPointMove = false;
                            rightWheelSpeed = -(MVSpeed + MVadd);
                            leftWheelSpeed = -(MVSpeed + MVadd);
                        }
                        else if (Vector3.Distance(this.transform.position - new Vector3(0, this.transform.position.y, 0), target - new Vector3(0, target.y, 0)) < speedPoint1)
                        {
                            isPointMove = true;
                            rightWheelSpeed = -(MVSpeed - MVadd);
                            leftWheelSpeed = -(MVSpeed - MVadd);
                        }
                        else
                        {
                            isPointMove = false;
                            rightWheelSpeed = -MVSpeed;
                            leftWheelSpeed = -MVSpeed;
                        }
                    }
                }
            }
            else
            {
                saving.Foward.Add(0);
                rightWheelSpeed = 0;
                leftWheelSpeed = 0;
            }
             CaculateBestWheelSpeed(target);
        }
        else
        {
            saving.Foward.Add(0);
            rightWheelSpeed = 0;
            leftWheelSpeed = 0;
        }
        #endregion move
        
        PointMove(isPointMove);
    }
    void PointMove(bool isPoint)
    {
        if (isPointZero)
        {
            if (Time.time - pointTime > pointStopDuration)
            {
                isPointZero = false;
                pointTime = Time.time;
            }
        }
        else
        {
            if (Time.time - pointTime > pointMoveDuration)
            {
                isPointZero = true;
                pointTime = Time.time;
            }
        }
        if (isPoint)
        {
            if (isPointZero)
            {
                rightWheelSpeed = 0;
                leftWheelSpeed = 0;
            }
        }
    }

    void CaculateBestWheelSpeed(Vector3 target)
    {
        Vector2 faceTo = new Vector2(face.transform.position.x - this.transform.position.x, face.transform.position.z - this.transform.position.z);
        faceTo.Normalize();
        Vector2 target2D = new Vector2(target.x, target.z);
        Vector2 tracker2D = new Vector2(this.transform.position.x, this.transform.position.z);
        Vector2 toOther = target2D - tracker2D;
        Vector2 toOther_Raw = toOther;
        toOther.Normalize();
        float dot = toOther.x * faceTo.x + toOther.y * faceTo.y;
        if (dot > 0)
        {
            Vector2 n = new Vector2(-faceTo.y, faceTo.x); //faceTo的垂直向量
            float R = ((target2D.x - tracker2D.x) * (target2D.x - tracker2D.x) + (target2D.y - tracker2D.y) * (target2D.y - tracker2D.y)) / (2 * (toOther_Raw.x * n.x + toOther_Raw.y * n.y));
            Vector2 PointO = new Vector2(tracker2D.x + R * n.x, tracker2D.y + R * n.y);
            float thettaa = 2 * Mathf.Asin(Vector2.Distance(target2D, tracker2D) / (2 * Mathf.Abs(R)));
            Debug.Log(thettaa * (Mathf.Abs(R) - Robotd) / thettaa * (Mathf.Abs(R) + Robotd));
            rightWheelSpeed = MVSpeed;
            leftWheelSpeed = MVSpeed * ((thettaa * (Mathf.Abs(R) - Robotd)) / (thettaa * (Mathf.Abs(R) + Robotd)));
        }
        else if(dot < 0)
        {
            Vector2 n = new Vector2(-faceTo.y, faceTo.x); //faceTo的垂直向量
            n = -n;
            float R = ((target2D.x - tracker2D.x) * (target2D.x - tracker2D.x) + (target2D.y - tracker2D.y) * (target2D.y - tracker2D.y)) / (2 * (toOther_Raw.x * n.x + toOther_Raw.y * n.y));
            Vector2 PointO = new Vector2(tracker2D.x + R * n.x, tracker2D.y + R * n.y);
            float thettaa = 2 * Mathf.Asin(Vector2.Distance(target2D, tracker2D) / (2 * Mathf.Abs(R)));
            rightWheelSpeed = -(MVSpeed * ((thettaa * (Mathf.Abs(R) - Robotd)) / (thettaa * (Mathf.Abs(R) + Robotd))));
            leftWheelSpeed = -MVSpeed;
        }
        else
        {
            rightWheelSpeed = 0;
            leftWheelSpeed = 0;
        }
    }

}
