using UnityEngine;
using System.Collections;

public class SeekToNode : MonoBehaviour {

	public float seekSpeed = 3.0f;

	bool isSeeking;
	Vector3 target, forward;
	GameObject seekNode;
	GameObject prevNode;
	
	
	// Use this for initialization
	void Start () {
		isSeeking = false;
		seekNode = null;
		prevNode = null;
	}
	
	// Update is called once per frame
	void Update () {

		// See if the seekNode has been updated in the preceding tick
		// If it has, we need to update where we are looking
		// as well as where we are seeking to
		if (seekNode != prevNode && seekNode != null) {
			
			// Set new target position
			target = seekNode.transform.position;
			target.z = 0;
			Vector3 toTarget = target - transform.position;

			// Calculate where we need to be headed
			forward = toTarget.normalized;
			
			// Calculate where we are headed
			Vector3 pointAhead = new Vector3(Mathf.Cos (transform.eulerAngles.z * Mathf.Deg2Rad),
			                                 Mathf.Sin (transform.eulerAngles.z * Mathf.Deg2Rad),
			                                 0);

			// Finally calculate needed degrees of rotation
			float rotDegrees = Mathf.Acos(Mathf.Clamp01(Vector3.Dot(pointAhead.normalized, forward))) * Mathf.Rad2Deg;
			
			
			// Orient player toward target
			if ((transform.worldToLocalMatrix.MultiplyPoint3x4(target)).y > 0) {
				transform.Rotate(Vector3.forward, rotDegrees);

			}
			else {
				transform.Rotate(Vector3.forward, -rotDegrees);

			}

			
		}

		// Seek to the target node
		if (seekNode != null) {
			SeekToTarget();
			prevNode = seekNode;
		}

	}

	public void SetNode(GameObject newSeekNode) {
		seekNode = newSeekNode;
	}

	public void SeekToTarget() {
		if (Vector2.Distance(target, transform.position) > 0.13) {
			isSeeking = true;
			transform.position += forward * Time.deltaTime * seekSpeed;
		} else {
			seekNode = null;
			isSeeking = false;
		}
	}

	public bool IsSeeking() {
		return isSeeking;
	}
	
}

