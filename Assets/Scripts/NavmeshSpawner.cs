using UnityEngine;
using System.Collections;

public class NavmeshSpawner : MonoBehaviour {
	
	public GameObject point;

	Navmesh points;

	public float step = 15.0f;

	
	// Use this for initialization
	void Start () {
	
		points = new Navmesh();

		float width = Camera.main.pixelWidth;
		float height = Camera.main.pixelHeight;
		
		for (float x = 0; x < width; x += step) {
			for (float y = 0; y < height; y += step) {
				Vector3 pointPos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0));
				if (!Physics.Raycast(pointPos, Vector3.forward))
					points.Add(Instantiate(point, pointPos + (10 * Vector3.forward), Quaternion.identity));
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

}

public class Navmesh : ArrayList {

	public float gameUnitStep;


}
