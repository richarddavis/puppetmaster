using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateObjects : MonoBehaviour {

    public float RegenerateTime = 20f;
    private List<GameObject> _CreatedObjects = new List<GameObject>();

    public List<GameObject> ObjectsToGenerate;
    public List<int> NumberToGenerate;
    public List<float> ScaleOfObject;

    private Vector3 _MinBounds = new Vector3(-20f, 2f, -3f);
    private Vector3 _MaxBounds = new Vector3(20, 10f, 15f);

	// Use this for initialization
	void Start () {
        //StartCoroutine("IRegenerateObjects");
        InvokeRepeating("RegenerateObjects", 0f, RegenerateTime);
        //InvokeRepeating("DestroyObjects", 0f, RegenerateTime);
        //InvokeRepeating("CreateObjects", 0.1f, RegenerateTime);
    }

    private Vector3 GetRandomPosition(Vector3 min, Vector3 max)
    {
        float xRand = Random.Range(min.x, max.x);
        float yRand = Random.Range(min.y, max.y);
        float zRand = Random.Range(min.z, max.z);
        return new Vector3(xRand, yRand, zRand);
    }

    private void RegenerateObjects()
    {
        DestroyObjects();
        CreateObjects();
    }

    private void CreateObjects()
    {
        for (int i = 0; i < ObjectsToGenerate.Count; i++)
        {
            for (int j = 0; j < NumberToGenerate[i]; j++)
            {
                Vector3 randomObjPos = GetRandomPosition(_MinBounds, _MaxBounds);
                GameObject randObj = Instantiate<GameObject>(ObjectsToGenerate[i], randomObjPos, Quaternion.Euler(new Vector3(-70f, 180f, 0f)));
                randObj.transform.localScale = new Vector3(ScaleOfObject[i], ScaleOfObject[i], ScaleOfObject[i]);
                _CreatedObjects.Add(randObj);
            }
        }
    }

    private void DestroyObjects()
    {
        foreach (GameObject g in _CreatedObjects)
        {
            Destroy(g);
        }
    }

}
