using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

public class GroundManager : MonoBehaviour {

    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;
    public GameObject Ground;
    private float _FloorHeightMultiplier = -4f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // Press the "c" key on the keyboard to calibrate the floor.
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (BodySourceManager == null)
            {
                return;
            }

            _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
            if (_BodyManager == null)
            {
                return;
            }

            Kinect.Vector4 FloorData = _BodyManager.GetFloorData();
            if (FloorData == null)
            {
                return;
            }

            float floorHeight = GetFloorHeight(FloorData);
            if (floorHeight != 0f)
            {
                Debug.Log("Calibrating floor height and tilt.");
                Ground.transform.position = new Vector3(Ground.transform.position.x, GetFloorHeight(FloorData) * _FloorHeightMultiplier, Ground.transform.position.z);
                Ground.transform.eulerAngles = new Vector3((float)GetTilt(FloorData), Ground.transform.eulerAngles.y, Ground.transform.eulerAngles.z);
            } else
            {
                Debug.Log("The Kinect's field of view is limited. Not calibrating floor height and tilt.");
            }
        }
    }

    float GetFloorHeight(Kinect.Vector4 floorData)
    {
        return floorData.W;
    }

    double GetTilt(Kinect.Vector4 floorData)
    {
        return Math.Atan(floorData.Z / floorData.Y) * (180.0 / Math.PI);
    }
}
