using UnityEngine;
using System.Collections;

public class JoystickController : MonoBehaviour {

	public int speed = 3;
	public int rotationSpeed = 70;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey("w") || Input.GetKey("up")) {
			Vector3 heading = GetHeading();
			transform.position += heading * Time.deltaTime * speed;
		}
		else if (Input.GetKey("s") || Input.GetKey("down")) {
			Vector3 heading = GetHeading();
			transform.position -= heading * Time.deltaTime * speed;
		}
		if (Input.GetKey("a") || Input.GetKey("left")) {
			transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
		}

		else if (Input.GetKey("d") || Input.GetKey("right")) {
			transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime );
		}
	
	}

	Vector3 GetHeading() {
		float heading = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
		return new Vector3 (Mathf.Cos (heading), Mathf.Sin (heading), 0);
	}
}
