using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomInitialForce : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Rigidbody r = GetComponent<Rigidbody>();
        r.AddForce(GetRandomVector3(-5f, 5f), ForceMode.Impulse);
	}

    private Vector3 GetRandomVector3(float min, float max)
    {
        float xRand = Random.Range(min, max);
        float yRand = Random.Range(min, max);
        float zRand = Random.Range(min, max);
        return new Vector3(xRand, yRand, zRand);
    }
}
