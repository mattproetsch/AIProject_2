using UnityEngine;
using System.Collections;

public class NavmeshSpawner : MonoBehaviour {
	
	public GameObject point;
	
	public float width;
	public float height;
	public float step;
	
	// Use this for initialization
	void Start () {
		
		for (float x = -width/2f; x < width/2f; x += step) {
			for (float y = -height/2f; y < height/2f; y += step) {
				Instantiate(point, new Vector3(x, y, 1), Quaternion.identity);
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
