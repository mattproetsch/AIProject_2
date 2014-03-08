using UnityEngine;
using System.Collections;

public class NavmeshSpawner : MonoBehaviour {
	
	public GameObject point;
	Navmesh points;

	float width, height;
	float step = 15.0f;
	
	// Use this for initialization
	void Start () {

		points = new Navmesh();

		width = Camera.main.pixelWidth;
		height = Camera.main.pixelHeight;
		
		for (float x = 0; x < width; x += step) {
			for (float y = 0; y < height; y += step) {
				Vector3 pointPos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0)) + new Vector3(0, 0, 9);
				if (!Physics.Raycast(pointPos, Vector3.forward))
					points.Add(Instantiate(point, pointPos + new Vector3(0, 0, 1), Quaternion.identity));
			}
		}

		Debug.Log ("Total points: " + points.Count);
		points.width = width;
		points.height = height;
		points.step = step;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Navmesh GetNavmesh() {
		return points;
	}
}

public class Navmesh : ArrayList {

	public float width;
	public float height;
	public float step;

}
