using UnityEngine;
using System.Collections;

public class SetAsAStarTargetOnClick : MonoBehaviour {


	Rect srBounds2D;

	// Use this for initialization
	void Start () {
		
		Bounds srBounds = this.gameObject.GetComponent<SpriteRenderer>().bounds;
		Debug.Log ("srBounds min for icecream: (" + srBounds.min.x + ", " + srBounds.min.y + ", " + srBounds.min.z + ")");
		Debug.Log ("srBounds max for icecream: (" + srBounds.max.x + ", " + srBounds.max.y + ", " + srBounds.max.z + ")");

		srBounds2D = new Rect(srBounds.min.x, srBounds.min.y, 2*srBounds.extents.x, 2*srBounds.extents.y);
		Debug.Log ("srBounds2D Rect left: " + srBounds2D.xMin + " top: " + srBounds2D.yMax + " right: " + srBounds2D.xMax + " bottom: " + srBounds2D.yMin);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")) {

			// Determine if we are the target of the click
			Vector3 clickWorldCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Debug.Log("Click at (" + clickWorldCoords.x + ", " + clickWorldCoords.y + ")");


			if (srBounds2D.Contains(clickWorldCoords)) {
				// The click was on the ice cream cone, so set it as the AStar target
				GameObject flappy = GameObject.Find("Flappy");
				flappy.GetComponent<AStar>().target = this.gameObject;
			}
		}
	}
}
