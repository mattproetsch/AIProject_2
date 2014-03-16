using UnityEngine;
using System.Collections;

public class ShowNearbyNavpoints : MonoBehaviour {

	ArrayList nearbyNavpoints;

	// Use this for initialization
	void Start () {

		nearbyNavpoints = new ArrayList ();
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "NavmeshObject") {
			other.gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			other.gameObject.GetComponent<SpriteRenderer> ().color = Color.gray;
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
}
