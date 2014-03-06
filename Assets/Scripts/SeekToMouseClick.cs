using UnityEngine;
using System.Collections;

public class SeekToMouseClick : MonoBehaviour {

	bool isSeeking;
	Vector3 forward, target;
	public float seekSpeed = 3.0f;

	

	// Use this for initialization
	void Start () {
		isSeeking = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown("Fire1")) {
			isSeeking = true;

			// Set new target position
			target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			target.z = 0;
			Vector3 toTarget = target - transform.position;

			// Calculate forward vector
			forward = toTarget.normalized;

			// Calculate needed degrees of rotation
			Vector3 pointAhead = new Vector3(Mathf.Cos (transform.eulerAngles.z * Mathf.Deg2Rad),
			                                 Mathf.Sin (transform.eulerAngles.z * Mathf.Deg2Rad),
			                                 0);
			//Debug.Log ("pointAhead: " + pointAhead.x + ", " + pointAhead.y + ", " + pointAhead.z);

			float rotDegrees = Mathf.Acos(Vector3.Dot (pointAhead.normalized, toTarget.normalized)) * Mathf.Rad2Deg;


			// Orient player toward target
			if ((transform.worldToLocalMatrix.MultiplyPoint3x4(target)).y > 0) {
				transform.Rotate(Vector3.forward, rotDegrees);
			}
			else {
				transform.Rotate(Vector3.forward, -rotDegrees);
			}

		}

		if (isSeeking) {
			transform.position += forward * Time.deltaTime * seekSpeed;
			if (Vector3.Distance(transform.position, target) < 0.1) {
				isSeeking = false;
			}
		}
	}
}
