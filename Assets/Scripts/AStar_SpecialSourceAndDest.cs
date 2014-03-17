using UnityEngine;
using System;
using System.Collections;


public class AStar_SpecialSourceAndDest : MonoBehaviour {

	ArrayList path;
	Navmesh navmesh;
	public GameObject source;
	float pathDist;

	// the NavmeshPoint that pathfinding will end at
	public static GameObject endPoint;

	// fudge must be << than navmesh.span
	private float fudge = 0.001f;

	// used in finding adjacent points
	private static int layerMask = ~(1 << 2 | 1 << 8 | 1 << 9 );

	// used in distance calculations
	private float sqrt_2 = Mathf.Sqrt(2.0f);

	// keep track of source and dest locations
	private Vector3 sourceClick;
	private bool sourceClicked = false;
	private Vector3 destClick;
	private bool destClicked = false;


	// Use this for initialization
	void Start () {

		source = null;
		pathDist = 0.0f;


	}

	public void UpdateNavmesh() {
				navmesh = GameObject.Find ("NavmeshGenerator").GetComponent<NavmeshSpawner> ().GetNavmesh ();
	}
	
	// Update is called once per frame
	void Update () {
		if (pathDist < 0.01 && sourceClicked && destClicked) {
			pathDist = 1.0f;
			CalculatePath();
			sourceClicked = false;
			destClicked = false;
		}

		if (Input.GetMouseButton (0)) {
			sourceClick = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			sourceClicked = true;
			Debug.Log (sourceClick);
		}
		else if (Input.GetMouseButton (1)) {
			destClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			destClicked = true;
			Debug.Log(destClick);
		}
	}


	private void CalculatePath() {

		path = new ArrayList();
		navmesh = GameObject.Find ("NavmeshGenerator").GetComponent<NavmeshSpawner> ().GetNavmesh ();

		// cast rays up, down, left, right and if its longer than it should be, ignore the impact
		float expectedDist = navmesh.gameUnitStep + fudge;

		// Start the calculation by finding the closest node to the player (or GameObject to which we are attached)
		GameObject startPoint = FindClosestNavmeshPointTo(sourceClick);
		startPoint.GetComponent<SpriteRenderer> ().enabled = true;
		startPoint.GetComponent<SpriteRenderer> ().color = Color.red;
		//Debug.Log ("Starting from: (" + startPoint.transform.position.x + ", " + startPoint.transform.position.y + ")");

		_SearchElement startElement = new _SearchElement(startPoint, 0.0f);

		// Then find the closest GameObject to the target
		endPoint = FindClosestNavmeshPointTo(destClick);
		endPoint.GetComponent<SpriteRenderer> ().enabled = true;
		endPoint.GetComponent<SpriteRenderer> ().color = Color.red;
		//Debug.Log ("Ending at: (" + endPoint.transform.position.x + ", " + endPoint.transform.position.y + ")");

		// Keep a priority queue of points on the frontier, sorted in increasing order by F = G + H
		// The CompareTo() function of each _SearchElement takes into account the H value
		SortedList openSet = new SortedList();
		// Keep a list of NavmeshPoints we have already found in the SPT
		ArrayList closedSet = new ArrayList();

		openSet.Add(startElement, null);

		_SearchElement finalSearchElement = null;

		while (openSet.Count > 0) {

			// Dequeue the element in the openSet with the smallest distance
			_SearchElement current = openSet.GetKey(0) as _SearchElement;

			// Is this what we are looking for?
			if (current.point == endPoint) {
				closedSet.Add(current.point);
				finalSearchElement = current;
				break;

			}

			// Remove this NavmeshPoint from the openSet and add it to the closedSet
			openSet.Remove(current);
			closedSet.Add(current.point);
			current.point.layer = 9;

			//Debug.Log ("Processing point at (" + current.point.transform.position.x
			//           + ", " + current.point.transform.position.y + ")");

			// Get all NavmeshPoints adjacent to this point in the form of _SearchElements whose dists are current.dist
			// plus however long the edge from current to the adjacent point is (measured in terms of game space dist)
			ArrayList adj = GetAdjacentPoints(current, expectedDist);


			// Find out if any points adjacent to current are already in the openSet
			// If they are, find out if the distance through the current path is shorter than the distance
			// they are currently at through other paths. If the distance through current is shorter, update the dist
			// to be the dist through current, and update the from field to be current.

			// Note: We do not explicitly handle the heuristic estimate at this time, as it is taken care of for us
			// behind the scenes in the openSet.Add() function by the IComparable interface implemented by _SearchElement
			foreach (_SearchElement newFrontierElement in adj) {

				bool elementInOpenSet = false;
				bool replaceExistingElement = false;
				_SearchElement existingElementIndex = null;

				foreach (_SearchElement establishedFrontierElement in openSet.Keys) {
					if (newFrontierElement.point == establishedFrontierElement.point) {

						// This NavmeshPoint exists in the openSet
						elementInOpenSet = true;

						if (newFrontierElement.dist < establishedFrontierElement.dist) {

							// The new path is a better path than the current path
							replaceExistingElement = true;
							existingElementIndex = establishedFrontierElement;
						}

						// Break out of the openSet for-loop; we are done here since we found a match
						break;
					}
				}

				if (!elementInOpenSet) {
					openSet.Add(newFrontierElement, null);
					
				}
				else if (elementInOpenSet && replaceExistingElement) {
					openSet.Remove(existingElementIndex);
					openSet.Add(newFrontierElement, null);
				}
			}

		}

		// We either ran out of elements in the navmesh and should throw an error, or we arrived at the target
		if (finalSearchElement == null) {
			throw new Exception("Target element not found by A* algorithm");
		}
		else {

			// We shouldn't show the close navpoints any longer
			//this.gameObject.GetComponent<ShowNearbyNavpoints>().SendMessage("Cleanup");

			// Reconstruct the path that won
			path = new ArrayList();
			pathDist = 0.0f;

			_SearchElement pathPoint = finalSearchElement;
			while (pathPoint != null) {
				path.Add(pathPoint.point);
				pathDist += pathPoint.dist;
				pathPoint = (_SearchElement)pathPoint.from;
			}

			// Finally, reverse the path, since we added elements to it in reverse order (i.e. starting from target)
			path.Reverse();
			foreach (GameObject navmeshPoint in path) {
				SpriteRenderer sr = navmeshPoint.GetComponent<SpriteRenderer>();
				sr.enabled = true;
				sr.color = Color.red;
			}



			//this.gameObject.GetComponent<FollowPath>().StartFollowing(path);

			//Debug.Log ("Final path distance: " + pathDist);

		}

	}

	private GameObject FindClosestNavmeshPointTo(Vector3 tgt) {

		float closestDist = 1000;

		GameObject closestObj = null;


		foreach (GameObject cur in navmesh) {
			if (Vector2.Distance(cur.transform.position, tgt) < closestDist) {
				closestDist = Vector2.Distance(cur.transform.position, tgt);
				closestObj = cur;
			}
		}

		return closestObj;
	}

	// Returns an ArrayList of up to four adjacent _SearchElements in the up, down, left, right directions
	// relative to element, with their relevant fields filled out
	private ArrayList GetAdjacentPoints(_SearchElement element, float expectedDist) {

		// Set the NavmeshPoint pointed to by element.point to be invisible to raycasting so that we can raycast
		// from inside it
		int oldLayer = element.point.layer;
		element.point.layer = 8;

		// Cast rays up, down, left, right
		// We don't need to cast diagonally because we will have path smoothing kicking in for that later
		// If they intersect with a GameObject with tag NavmeshObject in <= expectedDist, add that point to AdjPoints
		ArrayList adj = new ArrayList();

		RaycastHit2D raycastHitUp = Physics2D.Raycast(element.point.transform.position, Vector2.up, expectedDist, layerMask);
		RaycastHit2D raycastHitDown = Physics2D.Raycast(element.point.transform.position, -Vector2.up, expectedDist, layerMask);
		RaycastHit2D raycastHitLeft = Physics2D.Raycast(element.point.transform.position, -Vector2.right, expectedDist, layerMask);
		RaycastHit2D raycastHitRight = Physics2D.Raycast(element.point.transform.position, Vector2.right, expectedDist, layerMask);

		RaycastHit2D raycastHitUpLeft = Physics2D.Raycast(element.point.transform.position, (Vector2.up - Vector2.right), expectedDist*sqrt_2, layerMask);
		RaycastHit2D raycastHitUpRight = Physics2D.Raycast(element.point.transform.position, (Vector2.up + Vector2.right), expectedDist*sqrt_2, layerMask);
		RaycastHit2D raycastHitDownLeft = Physics2D.Raycast(element.point.transform.position, (-Vector2.up - Vector2.right), expectedDist*sqrt_2, layerMask);
		RaycastHit2D raycastHitDownRight = Physics2D.Raycast(element.point.transform.position, (-Vector2.up + Vector2.right), expectedDist*sqrt_2, layerMask);

		// If the raycast hits something, and it's a NavmeshPoint, add it to adj with dist = G (we account for H later,
		// when adding to the priority queue) and with its from field pointing to element
		//Debug.Log ("Rays are cast");
		if (raycastHitUp.collider != null) {
			if (raycastHitUp.collider.gameObject.CompareTag("NavmeshObject")) {
				adj.Add(new _SearchElement(raycastHitUp.collider.gameObject,
				                          element.dist
				                          + Vector2.Distance(raycastHitUp.point, element.point.transform.position),
				                          element));
				//Debug.Log ("Adding frontier element at ("
				//           + raycastHitUp.collider.gameObject.transform.position.x
				//           + ", "
				//           + raycastHitUp.collider.gameObject.transform.position.y
				//           + ")");
			}
		}
		if (raycastHitDown.collider != null) {
			if (raycastHitDown.collider.gameObject.CompareTag("NavmeshObject")) {
				adj.Add(new _SearchElement(raycastHitDown.collider.gameObject,
				                          element.dist
				                          + Vector2.Distance(raycastHitDown.point, element.point.transform.position),
				                          element));
				//Debug.Log ("Adding frontier element at ("
				//           + raycastHitDown.collider.gameObject.transform.position.x
				//           + ", "
				//           + raycastHitDown.collider.gameObject.transform.position.y
				//           + ")");

			}
		}
		if (raycastHitLeft.collider != null) {
			if (raycastHitLeft.collider.gameObject.CompareTag("NavmeshObject")) {
				adj.Add(new _SearchElement(raycastHitLeft.collider.gameObject,
				                          element.dist
				                          + Vector2.Distance(raycastHitLeft.point, element.point.transform.position),
				                          element));
				//Debug.Log ("Adding frontier element at ("
				//           + raycastHitLeft.collider.gameObject.transform.position.x
				//           + ", "
				//           + raycastHitLeft.collider.gameObject.transform.position.y
				//           + ")");

			}
		}
		if (raycastHitRight.collider != null) {
			if (raycastHitRight.collider.gameObject.CompareTag("NavmeshObject")) {
				adj.Add(new _SearchElement(raycastHitRight.collider.gameObject,
				                          element.dist
				                          + Vector2.Distance(raycastHitRight.point, element.point.transform.position),
				                          element));
				//Debug.Log ("Adding frontier element at ("
				//           + raycastHitRight.collider.gameObject.transform.position.x
				//           + ", "
				//           + raycastHitRight.collider.gameObject.transform.position.y
				//           + ")");

			}
		}
		if (raycastHitUpLeft.collider != null) {
			if (raycastHitUpLeft.collider.gameObject.CompareTag("NavmeshObject")) {
				adj.Add(new _SearchElement(raycastHitUpLeft.collider.gameObject,
				                          element.dist
				                          + Vector2.Distance(raycastHitUpLeft.point, element.point.transform.position),
				                          element));
				//Debug.Log ("Adding frontier element at ("
				//           + raycastHitUpLeft.collider.gameObject.transform.position.x
				//           + ", "
				//           + raycastHitUpLeft.collider.gameObject.transform.position.y
				//           + ")");

			}
		}
		if (raycastHitUpRight.collider != null) {
			if (raycastHitUpRight.collider.gameObject.CompareTag("NavmeshObject")) {
				adj.Add(new _SearchElement(raycastHitUpRight.collider.gameObject,
				                          element.dist
				                          + Vector2.Distance(raycastHitUpRight.point, element.point.transform.position),
				                          element));
				//Debug.Log ("Adding frontier element at ("
				//           + raycastHitUpRight.collider.gameObject.transform.position.x
				//           + ", "
				//           + raycastHitUpRight.collider.gameObject.transform.position.y
				//           + ")");

			}
		}
		if (raycastHitDownLeft.collider != null) {
			if (raycastHitDownLeft.collider.gameObject.CompareTag("NavmeshObject")) {
				adj.Add(new _SearchElement(raycastHitDownLeft.collider.gameObject,
				                          element.dist
				                          + Vector2.Distance(raycastHitDownLeft.point, element.point.transform.position),
				                          element));
				//Debug.Log ("Adding frontier element at ("
				//           + raycastHitDownLeft.collider.gameObject.transform.position.x
				//           + ", "
				//           + raycastHitDownLeft.collider.gameObject.transform.position.y
				//           + ")");

			}
		}
		if (raycastHitDownRight.collider != null) {
			if (raycastHitDownRight.collider.gameObject.CompareTag("NavmeshObject")) {
				adj.Add(new _SearchElement(raycastHitDownRight.collider.gameObject,
				                          element.dist
				                          + Vector2.Distance(raycastHitDownRight.point, element.point.transform.position),
				                          element));
				//Debug.Log ("Adding frontier element at ("
				//           + raycastHitDownRight.collider.gameObject.transform.position.x
				//           + ", "
				//           + raycastHitDownRight.collider.gameObject.transform.position.y
				//           + ")");

			}
		}

		// Reset old element.point.layer
		element.point.layer = oldLayer;

		return adj;


	}

}

public class _SearchElement : IComparable {

	public GameObject point;
	public float dist;
	public _SearchElement from;

	public _SearchElement(GameObject g, float d) {
		point = g;
		dist = d;
		from = null;
	}

	public _SearchElement(GameObject g, float d, _SearchElement f) {
		point = g;
		dist = d;
		from = f;
	}

	public int CompareTo(object obj) {

		if (obj == null)
			return 1;

		_SearchElement other = obj as _SearchElement;

		// Compare these points based on G + H values
		float thisF = this.dist + Vector2.Distance(point.transform.position, AStar.endPoint.transform.position);
		float otherF = other.dist + Vector2.Distance(other.point.transform.position, AStar.endPoint.transform.position);

		if (thisF == otherF) {
			if (UnityEngine.Random.value < 0.5)
				return 1;
			else
				return -1;
		}
		else
			return thisF.CompareTo(otherF);
	}

}