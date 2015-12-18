using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour {

	Vector3 orbit = Vector3.left;
	void Start () {
		this.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
	}

	void Update () {
		this.transform.RotateAround (Vector3.zero, orbit, -20 * Time.deltaTime);
		this.transform.RotateAround (Vector3.zero, Vector3.down, -15 * Time.deltaTime);
		if (Input.GetKey ("k"))
			Destroy (GameObject.Find("Arwing"));
	}
}
