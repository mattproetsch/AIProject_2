using UnityEngine;
using System.Collections;

public class ShowNearbyNavpoints : MonoBehaviour {

	public ArrayList nearbyNavpoints;

	// Use this for initialization
	void Start () {

		nearbyNavpoints = new ArrayList ();
	
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject navpoint in nearbyNavpoints) {
			if (!Obscuredd (navpoint)) {
				navpoint.GetComponent<SpriteRenderer> ().enabled = true;
				navpoint.GetComponent<SpriteRenderer> ().color = Color.gray;
			} else {
				navpoint.GetComponent<SpriteRenderer> ().color = Color.white;
				navpoint.GetComponent<SpriteRenderer> ().enabled = false;
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
		RaycastHit2D rh2D = Physics2D.Raycast (transform.position, (tgt.transform.position - transform.position), Mathf.Infinity, ((1 << 8) | (1 << 10)));
		tgt.layer = 0;
		if (rh2D && rh2D.collider.tag != "Obstacle") {
			return false;
		} else
			Debug.Log (rh2D.collider.tag);
			return true;
	}
}
