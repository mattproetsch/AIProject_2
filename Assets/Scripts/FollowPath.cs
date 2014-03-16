using UnityEngine;
using System.Collections;

public class FollowPath : MonoBehaviour {

	ArrayList path;
	SeekToNode seekComponent;

	// Use this for initialization
	void Start () {
		path = null;
		seekComponent = this.gameObject.GetComponent<SeekToNode>();
	}
	
	// Update is called once per frame
	void Update () {

		if (path == null) {
			return;
		}

		if (seekComponent.IsSeeking())
			return;

		if (path.Count > 0) {
			seekComponent.SetNode(path[0] as GameObject);
			path.RemoveAt(0);
		}
	
	}

	public void StartFollowing(ArrayList p) {
		path = p;
	}
}
