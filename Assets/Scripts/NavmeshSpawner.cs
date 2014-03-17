using UnityEngine;
using System.Collections;

public class NavmeshSpawner : MonoBehaviour {
	
	public GameObject point;

	Navmesh points;

	public float step = 15.0f;


	
	// Use this for initialization
	void Start () {

		Rect flappyBoundingRect = SpriteRendererBoundingRect (GameObject.Find ("Flappy").GetComponent<SpriteRenderer> ());
		float flappyWidth = 0.5f * flappyBoundingRect.width;
		float flappyHeight = 0.5f * flappyBoundingRect.height;
	
		points = new Navmesh();

		float width = Camera.main.pixelWidth;
		float height = Camera.main.pixelHeight;
		
		for (float x = 0; x < width; x += step) {
			for (float y = 0; y < height; y += step) {
				Vector3 pointPos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0));

				// Check at this point, as well as to the left, right, up, down of it to make sure Flappy can fit
				if (!Physics.Raycast(pointPos, Vector3.forward) &&
				    !Physics.Raycast(pointPos + new Vector3(flappyWidth, 0, 0), Vector3.forward) &&
				    !Physics.Raycast (pointPos + new Vector3(-flappyWidth, 0, 0), Vector3.forward) &&
				    !Physics.Raycast (pointPos + new Vector3(0, flappyHeight, 0), Vector3.forward) &&
				    !Physics.Raycast(pointPos + new Vector3(0, -flappyHeight, 0), Vector3.forward))

					points.Add(Instantiate(point, pointPos + (10 * Vector3.forward), Quaternion.identity) as GameObject);
			}
		}

		Debug.Log ("Total points: " + points.Count);

		float gameUnitInPx = (Camera.main.WorldToScreenPoint(new Vector3(1, 0, 0))
		                       - Camera.main.WorldToScreenPoint(new Vector3(0, 0, 0))).x;

		points.gameUnitStep = (step / gameUnitInPx);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Navmesh GetNavmesh() {
		return points;
	}

	Rect SpriteRendererBoundingRect(SpriteRenderer sr) {
		Bounds bounds = sr.bounds;
		return new Rect (bounds.min.x, bounds.min.y, 2 * bounds.extents.x, 2 * bounds.extents.y);
	}

}

public class Navmesh : ArrayList {

	public float gameUnitStep;


}
