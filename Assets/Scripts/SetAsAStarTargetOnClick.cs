using UnityEngine;
using System.Collections;

public class SetAsAStarTargetOnClick : MonoBehaviour {


	Rect srBounds2D;

	// Use this for initialization
	void Start () {
		
		srBounds2D = SpriteRendererBoundingRect (this.gameObject.GetComponent<SpriteRenderer> ());
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

	Rect SpriteRendererBoundingRect(SpriteRenderer sr) {
		Bounds bounds = sr.bounds;
		return new Rect (bounds.min.x, bounds.min.y, 2 * bounds.extents.x, 2 * bounds.extents.y);
	}
}
