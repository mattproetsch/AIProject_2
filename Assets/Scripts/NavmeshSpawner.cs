using UnityEngine;
using System.Collections;

public class NavmeshSpawner : MonoBehaviour {
	
	public GameObject point;
	
	float width;
	float height;
	public float step = 20.0f;

	ArrayList points;
	
	// Use this for initialization
	void Start () {

		points = new ArrayList();

		width = Camera.main.pixelWidth;
		height = Camera.main.pixelHeight;
		
		for (float x = 0; x < width; x += step) {
			for (float y = 0; y < height; y += step) {
				Vector3 pointPos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0)) + new Vector3(0, 0, 9);
				if (!Physics.Raycast(pointPos, Vector3.forward))
					points.Add(Instantiate(point, pointPos, Quaternion.identity));
			}
		}

		Debug.Log ("Total points: " + points.Count);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
