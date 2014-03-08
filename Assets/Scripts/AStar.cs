using UnityEngine;
using System;
using System.Collections;

public class AStar : MonoBehaviour {

	ArrayList path;
	Navmesh navmesh;
	public GameObject target;
	float pathDist;

	// fudge must be << than navmesh.span
	private float fudge = 1.0f;

	// Use this for initialization
	void Start () {

		target = null;
		pathDist = 0.0f;

	}
	
	// Update is called once per frame
	void Update () {
		if (pathDist < 0.01 && target != null) {
			CalculatePath();
			pathDist = 1.0f;
		}
	}


	private void CalculatePath() {

		Debug.Log("Calculating...");

		navmesh = GameObject.Find("NavmeshGenerator").GetComponent<NavmeshSpawner>().GetNavmesh();
		path = new ArrayList();

		// cast rays up, down, left, right and if its longer than width/span + fudge, there's nothing
		float expectedDist = (navmesh.width / navmesh.step) + fudge;

		// Start the calculation by finding the closest node to the player (or GameObject to which we are attached)
		GameObject startPoint = FindClosestNavmeshPointTo(this.gameObject.transform.position);
		Debug.Log ("Starting from: (" + startPoint.transform.position.x + ", " + startPoint.transform.position.y + ")");

		SearchElement start = new SearchElement(startPoint, 0.0f);

		// Then find the closest GameObject to the target
		GameObject endPoint = FindClosestNavmeshPointTo(target.gameObject.transform.position);
		Debug.Log ("Ending at: (" + endPoint.transform.position.x + ", " + endPoint.transform.position.y + ")");

		// Keep a priority queue of points on the frontier, sorted in increasing order by F = G + H
		SortedList openSet = new SortedList();
		// Keep a list of NavmeshPoints we have already found in the SPT
		ArrayList closedSet = new ArrayList();

		openSet.Add(start.dist + Vector2.Distance(startPoint.transform.position, endPoint.transform.position), start); // F = G + H

		SearchElement finalSearchElement = null;

		while (openSet.Count > 0) {

			// Dequeue the element in the openSet with the smallest distance
			SearchElement current = (SearchElement)openSet.GetByIndex(0);

			// Is this what we are looking for?
			if (current.point == endPoint) {
				closedSet.Add(current.point);
				finalSearchElement = current;
				break;

			}

			openSet.RemoveAt(0);
			closedSet.Add(current.point);

			Debug.Log ("Processing point at (" + current.point.transform.position.x
			           + ", " + current.point.transform.position.y + ")");

			// Get all NavmeshPoints adjacent to this point in the form of SearchElements whose dists are current.dist
			// plus however long the edge from current to the adjacent point is (measured in terms of game space dist)
			ArrayList adj = GetAdjacentPoints(current, expectedDist);

			// Remove those points that are already in the closedSet
			ArrayList removeFromAdj = new ArrayList();
			foreach (SearchElement element in adj) {
				if (closedSet.Contains(element.point))
					removeFromAdj.Add(element);
			}
			foreach (SearchElement element in removeFromAdj) {
				adj.Remove(element);
			}

			// Find out if any points adjacent to current are already in the openSet
			// If they are, find out if the distance through the current path is shorter than the distance
			// they are currently at through other paths. If the distance through current is shorter, update the dist
			// to be the dist through current, and update the from field to be current.
			foreach (SearchElement newFrontierElement in adj) {

				bool elementInOpenSet = false;
				bool replaceExistingElement = false;
				float existingElementIndex = -1.0f;
				float heuristicDistance = Vector2.Distance(newFrontierElement.point.transform.position,
				                                           endPoint.transform.position);

				foreach (SearchElement establishedFrontierElement in openSet) {
					if (newFrontierElement.point == establishedFrontierElement.point) {

						// This NavmeshPoint exists in the openSet
						elementInOpenSet = true;

						if (newFrontierElement.dist < establishedFrontierElement.dist) {

							// The new path is a better path than the current path
							// Since we are dealing with the same point, the heuristic distance need not be taken
							// into account at this stage - only when we are dequeueing and enqueueing into the priority
							// queue (we dequeued earlier to get current, and we will enqueue each adjacent point later)
							replaceExistingElement = true;
							existingElementIndex = establishedFrontierElement.dist + heuristicDistance;
						}

						// Break out of the openSet for-loop; we are done here since we found a match
						break;
					}
				}

				if (!elementInOpenSet) {
					openSet.Add(newFrontierElement.dist + heuristicDistance, newFrontierElement);
				}
				else if (elementInOpenSet && replaceExistingElement) {
					openSet.Remove(existingElementIndex);
					openSet.Add(newFrontierElement.dist + heuristicDistance, newFrontierElement);
				}
			}

		}

		// We either ran out of elements in the navmesh and should throw an error, or we arrived at the target
		if (finalSearchElement == null) {
			throw new Exception("Target element not found by A* algorithm");
		}
		else {
			// Reconstruct the path that won
			path = new ArrayList();
			pathDist = 0.0f;

			SearchElement pathPoint = finalSearchElement;
			while (pathPoint != null) {
				path.Add(pathPoint.point);
				pathPoint = (SearchElement)pathPoint.from;
				pathDist += pathPoint.dist;
			}

			// Finally, reverse the path, since we added elements to it in reverse order (i.e. starting from target)
			path.Reverse();

			Debug.Log ("Final path distance: " + pathDist);

		}

	}

	private GameObject FindClosestNavmeshPointTo(Vector3 pos) {
		float closestDist = navmesh.width + 1;
		GameObject closestObj = null;

		foreach (GameObject cur in navmesh) {
			if (Vector3.Distance(cur.transform.position, pos) < closestDist) {
				closestDist = Vector3.Distance(cur.transform.position, pos);
				closestObj = cur;
			}
		}

		return closestObj;
	}

	// Returns an ArrayList of up to four adjacent SearchElements in the up, down, left, right directions
	// relative to element, with their relevant fields filled out
	private ArrayList GetAdjacentPoints(SearchElement element, float expectedDist) {

		// Cast rays up, down, left, right
		// We don't need to cast diagonally because we will have path smoothing kicking in for that later
		// If they intersect with a GameObject with tag NavmeshObject in <= expectedDist, add that point to AdjPoints
		ArrayList adj = new ArrayList();

		RaycastHit2D raycastHitUp = Physics2D.Raycast(element.point.transform.position, Vector2.up);
		RaycastHit2D raycastHitDown = Physics2D.Raycast(element.point.transform.position, -Vector2.up);
		RaycastHit2D raycastHitLeft = Physics2D.Raycast(element.point.transform.position, -Vector2.right);
		RaycastHit2D raycastHitRight = Physics2D.Raycast(element.point.transform.position, Vector2.right);

		// If the raycast hits something, and it's a NavmeshPoint, add it to adj with dist = G (we account for H later,
		// when adding to the priority queue) and with its from field pointing to element
		Debug.Log ("Rays are cast");
		if (raycastHitUp.collider != null) {
			if (raycastHitUp.collider.gameObject.CompareTag("NavmeshObject")) {
				adj.Add(new SearchElement(raycastHitUp.collider.gameObject,
				                          element.dist
				                          + Vector2.Distance(raycastHitUp.point, element.point.transform.position),
				                          element));
				Debug.Log ("Adding frontier element at ("
				           + raycastHitUp.collider.gameObject.transform.position.x
				           + ", "
				           + raycastHitUp.collider.gameObject.transform.position.y
				           + ")");
			}
		}
		if (raycastHitDown.collider != null) {
			if (raycastHitDown.collider.gameObject.CompareTag("NavmeshObject")) {
				adj.Add(new SearchElement(raycastHitDown.collider.gameObject,
				                          element.dist
				                          + Vector2.Distance(raycastHitDown.point, element.point.transform.position),
				                          element));
				Debug.Log ("Adding frontier element at ("
				           + raycastHitDown.collider.gameObject.transform.position.x
				           + ", "
				           + raycastHitDown.collider.gameObject.transform.position.y
				           + ")");
			}
		}
		if (raycastHitLeft.collider != null) {
			if (raycastHitLeft.collider.gameObject.CompareTag("NavmeshObject")) {
				adj.Add(new SearchElement(raycastHitLeft.collider.gameObject,
				                          element.dist
				                          + Vector2.Distance(raycastHitLeft.point, element.point.transform.position),
				                          element));
				Debug.Log ("Adding frontier element at ("
				           + raycastHitLeft.collider.gameObject.transform.position.x
				           + ", "
				           + raycastHitLeft.collider.gameObject.transform.position.y
				           + ")");
			}
		}
		if (raycastHitRight.collider != null) {
			if (raycastHitRight.collider.gameObject.CompareTag("NavmeshObject")) {
				adj.Add(new SearchElement(raycastHitRight.collider.gameObject,
				                          element.dist
				                          + Vector2.Distance(raycastHitRight.point, element.point.transform.position),
				                          element));
				Debug.Log ("Adding frontier element at ("
				           + raycastHitRight.collider.gameObject.transform.position.x
				           + ", "
				           + raycastHitRight.collider.gameObject.transform.position.y
				           + ")");
			}
		}


		return adj;


	}

}

public class SearchElement {

	public GameObject point;
	public float dist;
	public SearchElement from;

	public SearchElement(GameObject g, float d) {
		point = g;
		dist = d;
		from = null;
	}

	public SearchElement(GameObject g, float d, SearchElement f) {
		point = g;
		dist = d;
		from = f;
	}

}