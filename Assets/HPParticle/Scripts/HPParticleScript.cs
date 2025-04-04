// Fades out the Text of the HPParticle and Destroys it 

using UnityEngine;
using System.Collections;

public class HPParticleScript : MonoBehaviour {

	public float Alpha =1f;
	public float FadeSpeed = 4f;

	private GameObject HPLabel;

	// Set a Variable
	void Start () 
	{
		HPLabel = gameObject.transform.Find("HPLabel").gameObject;
	}

	void FixedUpdate () 
	{
		Alpha = Mathf.Lerp(Alpha,0f,FadeSpeed * Time.deltaTime);

		Color CurrentColor = HPLabel.GetComponent<TextMesh>().color;
		HPLabel.GetComponent<TextMesh>().color = new Color(CurrentColor.r,CurrentColor.g,CurrentColor.b,Alpha);

		if (Alpha < 0.005f)
		{
			//Destroy(gameObject);
			EffectPoolManager.Instance.ReturnEffect(this.gameObject, "Damage");
            HPLabel.GetComponent<TextMesh>().color = new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, 1f);

            gameObject.SetActive(false);
		}
	}

	public void Initalize(Vector3 transform, Quaternion rotation)
    {
        HPLabel = gameObject.transform.Find("HPLabel").gameObject;

        Color CurrentColor = HPLabel.GetComponent<TextMesh>().color;

        gameObject.transform.position = transform;
        gameObject.transform.rotation = rotation;
        HPLabel.GetComponent<TextMesh>().color = new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, 1f);

    }
}
