using UnityEngine;
using System.Collections;

public class ShowNearbyNavpoints : MonoBehaviour {

	public ArrayList nearbyNavpoints;
	private static int mask = (1 << 8) | (1 << 10);

	// Use this for initialization
	void Start () {

		nearbyNavpoints = new ArrayList ();
	
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject navpoint in nearbyNavpoints) {
			if (!Obscuredd (navpoint)) {
				navpoint.GetComponent<SpriteRenderer> ().enabled = true;
				navpoint.GetComponent<SpriteRenderer> ().color = Color.white;
			} else {
				navpoint.GetComponent<SpriteRenderer> ().enabled = true;
				navpoint.GetComponent<SpriteRenderer> ().color = Color.gray;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "NavmeshObject") {

			nearbyNavpoints.Add (other.gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "NavmeshObject") {
			nearbyNavpoints.Remove (other.gameObject);
			other.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;
			other.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		}

	}
		
	void Cleanup() {
		foreach (GameObject navpoint in nearbyNavpoints) {
			navpoint.GetComponent<SpriteRenderer> ().enabled = false;

		}

		nearbyNavpoints.Clear ();
		this.gameObject.GetComponent<CircleCollider2D> ().enabled = false;
	}

	bool Obscuredd(GameObject tgt) {

		tgt.layer = 8;
		Vector3 toTarget = tgt.transform.position - transform.position;

		RaycastHit2D rh2D = Physics2D.Raycast (transform.position, toTarget.normalized, toTarget.magnitude, mask);
		tgt.layer = 0;
		if (rh2D.collider != null && rh2D.collider.tag != "Obstacle") {
			return false;
		} else
			//Debug.Log (rh2D.collider.tag);
			return true;
	}
}
