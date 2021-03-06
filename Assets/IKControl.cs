﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{

    protected Animator animator;
    private bool ShowSkeleton = false;

    public bool IkActive = false;
    public Transform HandRightObj = null;
    public Transform HandLeftObj = null;
    public Transform FootRightObj = null;
    public Transform FootLeftObj = null;
    public Transform LookObj = null;
    private const float BODY_SCALE = 1f; //6f
    private const float JOINT_SCALE = 1f; //10f

    public Kinect.Body KinectBodyObject;
    public GameObject UnityBodyObject;

    public Material BoneMaterial;
    //public GameObject BodySourceManager;
    //private BodySourceManager _BodyManager;

    private GameObject _HandRightTarget;
    private GameObject _HandLeftTarget;
    private GameObject _FootRightTarget;
    private GameObject _FootLeftTarget;

    public Vector3 HandRightRotationAdjustment = new Vector3(-90f, -90f, -90f);
    public Vector3 HandLeftRotationAdjustment = new Vector3(-90f, 0f, -90f);
    private Vector3 FootRightRotationAdjustment = new Vector3(0f, -90f, 180f);
    private Vector3 FootLeftRotationAdjustment = new Vector3(0f, 90f, 180f);

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();

    void Start()
    {
        animator = GetComponent<Animator>();

        _HandRightTarget = HandRightObj.gameObject;
        _HandLeftTarget = HandLeftObj.gameObject;
        _FootRightTarget = FootRightObj.gameObject;
        _FootLeftTarget = FootLeftObj.gameObject;
    }

    private void Update()
    {
        RefreshBodyObject(KinectBodyObject, UnityBodyObject);
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (IkActive)
            {

                // Set the look target position, if one has been assigned
                if (LookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(LookObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (HandRightObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, HandRightObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, HandRightObj.rotation);
                }

                // Set the left hand target position and rotation, if one has been assigned
                if (HandLeftObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, HandLeftObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, HandLeftObj.rotation);
                }

                // Set the right foot target position and rotation, if one has been assigned
                if (FootRightObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, FootRightObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, FootRightObj.rotation);
                }

                // Set the left foot target position and rotation, if one has been assigned
                if (FootLeftObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, FootLeftObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, FootLeftObj.rotation);
                }

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }

    public GameObject CreateBodyObject(Kinect.Body kinectBody)
    {
        GameObject body = new GameObject("Body:" + kinectBody.TrackingId);

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {

            GameObject jointObj = new GameObject();
            if (ShowSkeleton)
            {
                jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                LineRenderer lr = jointObj.AddComponent<LineRenderer>();
                lr.SetVertexCount(2);
                lr.material = BoneMaterial;
                lr.SetWidth(0.05f, 0.05f);
            }

            jointObj.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }

        Kinect.Joint Neck = kinectBody.Joints[Kinect.JointType.Neck];
        Kinect.Joint SpineMid = kinectBody.Joints[Kinect.JointType.SpineMid];
        float TorsoLength = GetBoneSize(Neck, SpineMid);
        // TorsoLength is .30 (about a foot) for me (6'1").
        transform.parent.localScale = new Vector3(TorsoLength * BODY_SCALE, TorsoLength * BODY_SCALE, TorsoLength * BODY_SCALE);

        return body;
    }

    private float GetBoneSize(Kinect.Joint j1, Kinect.Joint j2)
    {
        Vector3 v1 = new Vector3(j1.Position.X, j1.Position.Y, j1.Position.Z);
        Vector3 v2 = new Vector3(j2.Position.X, j2.Position.Y, j2.Position.Z);
        return Vector3.Magnitude(v1 - v2);
    }

    private Quaternion GetQuaternionFromJointOrientation(Kinect.JointOrientation jo)
    {
        Quaternion joQuaternion = new Quaternion(jo.Orientation.X, jo.Orientation.Y, jo.Orientation.Z, jo.Orientation.W);
        return joQuaternion;
    }

    private Vector3 GetEulerAnglesFromJointOrientation(Kinect.JointOrientation jo)
    {
        Quaternion joQuaternion = new Quaternion(jo.Orientation.X, jo.Orientation.Y, jo.Orientation.Z, jo.Orientation.W);
        return joQuaternion.eulerAngles;
    }

    private void RotateAvatar(Transform t, Kinect.JointOrientation jo)
    {
        transform.parent.rotation = GetQuaternionFromJointOrientation(jo);
        transform.parent.Rotate(new Vector3(0f, 180f, 0f));
    }

    public void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        // Set the position of the avatar based on the kinect body location
        Kinect.Joint SpineMid = body.Joints[Kinect.JointType.SpineMid];
        transform.parent.position = GetVector3FromJoint(SpineMid);

        // Set the rotation of the avatar based on the kinect body rotation
        Kinect.JointOrientation SpineShoulderOrientation = body.JointOrientations[Kinect.JointType.SpineShoulder];
        RotateAvatar(transform, SpineShoulderOrientation);

        // Get the position and rotation of both hands from Kinect
        Kinect.Joint HandRight = body.Joints[Kinect.JointType.HandRight];
        Kinect.Joint HandLeft = body.Joints[Kinect.JointType.HandLeft];
        Kinect.JointOrientation HandRightOrientation = body.JointOrientations[Kinect.JointType.HandRight];
        Kinect.JointOrientation HandLeftOrientation = body.JointOrientations[Kinect.JointType.HandLeft];

        // Get the position and rotation of both feet from Kinect
        Kinect.Joint FootRight = body.Joints[Kinect.JointType.FootRight];
        Kinect.Joint FootLeft = body.Joints[Kinect.JointType.FootLeft];
        Kinect.JointOrientation FootRightOrientation = body.JointOrientations[Kinect.JointType.AnkleRight];
        Kinect.JointOrientation FootLeftOrientation = body.JointOrientations[Kinect.JointType.AnkleLeft];

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            // Map the left and right IK targets to the left and right hand and foot joints in the Kinect model
            if (jt.ToString().Equals("HandLeft"))
            {
                _HandRightTarget.transform.position = jointObj.localPosition;
                _HandRightTarget.transform.rotation = GetQuaternionFromJointOrientation(HandLeftOrientation);
                _HandRightTarget.transform.Rotate(HandLeftRotationAdjustment);
            }

            if (jt.ToString().Equals("HandRight"))
            {
                _HandLeftTarget.transform.position = jointObj.localPosition;
                _HandLeftTarget.transform.rotation = GetQuaternionFromJointOrientation(HandRightOrientation);
                _HandLeftTarget.transform.Rotate(HandRightRotationAdjustment);
            }

            if (jt.ToString().Equals("FootLeft"))
            {
                _FootRightTarget.transform.position = jointObj.localPosition;
                _FootRightTarget.transform.rotation = GetQuaternionFromJointOrientation(FootLeftOrientation);
                _FootRightTarget.transform.Rotate(FootLeftRotationAdjustment);

            }

            if (jt.ToString().Equals("FootRight"))
            {
                _FootLeftTarget.transform.position = jointObj.localPosition;
                _FootLeftTarget.transform.rotation = GetQuaternionFromJointOrientation(FootRightOrientation);
                _FootLeftTarget.transform.Rotate(FootRightRotationAdjustment);
            }

            if (ShowSkeleton)
            {
                LineRenderer lr = jointObj.GetComponent<LineRenderer>();
                if (targetJoint.HasValue)
                {
                    lr.SetPosition(0, jointObj.localPosition);
                    lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                    lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
                }
                else
                {
                    lr.enabled = false;
                }
            }
        }
    }

    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;

            case Kinect.TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }

    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * JOINT_SCALE, joint.Position.Y * JOINT_SCALE, joint.Position.Z * JOINT_SCALE);
    }
}
