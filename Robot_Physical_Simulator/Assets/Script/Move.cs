using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
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
    //public float Robotd = 0.5f;
    bool move = false;
    float RTSpeed = 3f;
    float MVSpeed = 7f;
    float safeDis = 0.6f;
    private RobotMoveType RunType;
    public float maxAngleCrossError = 0f;
    public float minAngleCrossError = -0.2f;
    public float notAcceptCrossError = -0.5f;
    bool isSafeTarget = false;
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
            //goal = GameObject.Find("0");
            move = move ? false : true;
            //if (!move)
            //{
            //    //SaveData.Save(saving, "20251201_pointTest");
            //    //saving.check.Clear();
            //    //saving.AngleCross.Clear();
            //    //saving.Foward.Clear();
            //    //saving.Position.Clear();
            //    //saving.AngleDot.Clear();
            //    //saving.FaceTo.Clear();
            //    //saving.Time.Clear();
            //    //saving.CirclePoint.Clear();
            //    //Debug.Log("儲存並清空");
            //}
            //else
            //{
            //    //STTime = Time.time;
            //    //pointTime = Time.time;
            //}
        }
        if (goal != null)
        {
            CheckType(CheckSafety(CaculateCircleGoal()));
        }
        Debug.Log(RunType.ToString());
    }
    Vector2 CaculateCircleGoal()
    {
        Vector2 RU = new Vector2(this.transform.position.x - user.transform.position.x, this.transform.position.z - user.transform.position.z);
        RU = RU.normalized;
        Vector2 goalpos = new Vector2(user.transform.position.x + RU.x * Mathf.Cos(Mathf.PI / 4) - RU.y * Mathf.Sin(Mathf.PI / 4), user.transform.position.z + RU.x * Mathf.Sin(Mathf.PI / 4) + RU.y * Mathf.Cos(Mathf.PI / 4));
        return goalpos;
    }
    Vector2 CheckSafety(Vector2 nowGoal)
    {
        Vector2 trackerPos2D = new Vector2(this.transform.position.x, this.transform.position.z);
        Vector2 userPos2D = new Vector2(user.transform.position.x, user.transform.position.z);
        float dis = Vector2.Distance(trackerPos2D, userPos2D);
        if (dis <= safeDis)
        {
            Vector2 target = new Vector2(2 * trackerPos2D.x - userPos2D.x, 2 * trackerPos2D.y - userPos2D.y);
            isSafeTarget = true;
            return target;
        }
        else
        {
            isSafeTarget = false;
            return nowGoal;
        }
    }
    enum RobotMoveType
    {
        Foward,
        Rotate,
        Both,
        Break,
        Stop
    }
    void SetRobotSpeed(RobotMoveType Type, bool isFlip, bool isNeg)
    {
        Vector2 speedboth = new Vector2(0f, 0f);
        switch (Type)
        {
            case RobotMoveType.Foward:
                speedboth = Foward();
                break;
            case RobotMoveType.Rotate:
                speedboth = Rotate();
                break;
            case RobotMoveType.Both:
                speedboth = Both();
                break;
            case RobotMoveType.Break:
                speedboth = Break();
                break;
            case RobotMoveType.Stop:
                speedboth = new Vector2(0f, 0f);
                break;
        }
        if (!isFlip)
        {
            rightWheelSpeed = speedboth.x;
            leftWheelSpeed = speedboth.y;
        }
        else
        {
            rightWheelSpeed = speedboth.y;
            leftWheelSpeed = speedboth.x;
        }
        if (isNeg)
        {
            rightWheelSpeed = -rightWheelSpeed;
            leftWheelSpeed = -leftWheelSpeed;
        }
    }
    Vector2 Rotate()
    {
        Vector2 rot = new Vector2(-RTSpeed, RTSpeed);
        return rot;
    }
    Vector2 Foward()
    {
        Vector2 move = new Vector2(MVSpeed, MVSpeed);
        return move;
    }
    Vector2 Both()
    {
        Vector2 both = new Vector2(1.5f * MVSpeed, MVSpeed);
        return both;
    }
    Vector2 Break()
    {
        Vector2 bre = new Vector2(0f, 0f);
        return bre;
    }
    void CheckType(Vector2 target)
    {
        bool neg = false;
        bool reverse = false;
        goal.transform.position = new Vector3(target.x, 0.5f, target.y);
        Vector2 faceTo = new Vector2(face.transform.position.x - this.transform.position.x, face.transform.position.z - this.transform.position.z);
        faceTo.Normalize();
        Vector2 trackerPos2D = new Vector2(this.transform.position.x, this.transform.position.z);
        Vector2 userPos2D = new Vector2(user.transform.position.x, user.transform.position.z);
        Vector2 toOther = target - trackerPos2D;
        float dot = toOther.x * faceTo.x + toOther.y * faceTo.y;
        float cross = toOther.x * faceTo.y - toOther.y * faceTo.x;
        //Debug.Log(cross);
        if (isSafeTarget)
        {
            if (dot >= 0)
            {
                if (cross > 0.2f)
                {
                    RunType = RobotMoveType.Rotate;
                }
                else if (cross < -0.2f)
                {
                    RunType = RobotMoveType.Rotate;
                    neg = true;
                }
                else
                {
                    RunType = RobotMoveType.Foward;
                }
            }
            else
            {
                reverse = true;
                if (cross > 0.2f)
                {
                    RunType = RobotMoveType.Rotate;
                    neg = true;
                }
                else if (cross < -0.2f)
                {
                    RunType = RobotMoveType.Rotate;
                }
                else
                {
                    RunType = RobotMoveType.Foward;
                    neg = true;
                }
            }
        }
        else if (dot >= 0)
        {
            if (cross < notAcceptCrossError)
            {
                RunType = RobotMoveType.Rotate;
                neg = true;
            }
            else if (cross < minAngleCrossError)
            {
                RunType = RobotMoveType.Both;
            }
            else if (cross > maxAngleCrossError)
            {
                RunType = RobotMoveType.Rotate;
            }
            else
            {
                RunType = RobotMoveType.Foward;
            }
        }
        else
        {
            reverse = true;
            neg = true;
            if (cross < -maxAngleCrossError)
            {
                RunType = RobotMoveType.Rotate;
            }
            else if (cross > -notAcceptCrossError)
            {
                RunType = RobotMoveType.Rotate;
                neg = false;
            }
            else if (cross > -minAngleCrossError)
            {
                RunType = RobotMoveType.Both;
            }
            else
            {
                RunType = RobotMoveType.Foward;
            }
        }
        if (!move)
        {
            RunType = RobotMoveType.Stop;
        }
        Debug.Log(reverse);
        SetRobotSpeed(RunType, reverse, neg);
    }
}
