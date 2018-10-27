using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

public class PuppetManager : MonoBehaviour {

    public List<GameObject> Characters;
    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;
    private Dictionary<ulong, GameObject> _Puppets = new Dictionary<ulong, GameObject>();
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                Destroy(_Puppets[trackingId]);
                _Bodies.Remove(trackingId);
                _Puppets.Remove(trackingId);
            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    GameObject PuppetClone = Instantiate<GameObject>(Characters[UnityEngine.Random.Range(0, Characters.Count)]);
                    GameObject UnityBodyObject = PuppetClone.GetComponentInChildren<IKControl>().CreateBodyObject(body);
                    PuppetClone.GetComponentInChildren<IKControl>().KinectBodyObject = body;
                    PuppetClone.GetComponentInChildren<IKControl>().UnityBodyObject = UnityBodyObject;
                    _Puppets[body.TrackingId] = PuppetClone;
                    _Bodies[body.TrackingId] = UnityBodyObject;
                }
            }
        }
    }
}
