using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkyboxColorOnCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Color currentColor = RenderSettings.skybox.GetColor("_SkyTint");
        StartCoroutine(ChangeSkyboxColor(currentColor));
    }

    private IEnumerator ChangeSkyboxColor(Color startColor)
    {
        float a = 0f;
        Color endColor = Random.ColorHSV();
        while (a < 1f)
        {
            RenderSettings.skybox.SetColor("_SkyTint", Color.Lerp(startColor, endColor, a));
            a += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
