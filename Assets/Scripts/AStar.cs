using UnityEngine;
using System.Collections;

public class AStar : MonoBehaviour {

	ArrayList path;
	Navmesh navmesh;
	public GameObject target;

	// Use this for initialization
	void Start () {
		navmesh = GameObject.Find("NavmeshGenerator").GetComponent<NavmeshSpawner>().GetNavmesh();
		path = new ArrayList();
		target = null;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SetTarget(GameObject tgt) {
		if (!target == tgt) {
			target = tgt;
			CalculatePath();
		}
	}

	private void CalculatePath() {

	}
}
