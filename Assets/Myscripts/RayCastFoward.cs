using UnityEngine;
using System.Collections;

public class RayCastFoward : MonoBehaviour {

	float distance;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		// cursor's position
		GameObject cursorForHere = GameObject.Find ("Cursor");
		Transform cursorPosition = cursorForHere.transform;
		Vector3 cPos = cursorPosition.position;

		// create a ray cast that orignates from the main camera position to cursor position
		Vector3 forward = transform.TransformDirection (Vector3.forward) * 10;
		RaycastHit hit;

		Debug.DrawRay (transform.position,forward, Color.red);

		if (Input.GetMouseButtonDown (0)) {
			if(Physics.Raycast(transform.position,(forward),out hit)){
				distance = hit.distance;
				print("Camera's Direction "+transform.position.x+" "+transform.position.y+" "+transform.position.z+" "
				      + "Object's distance "+distance + " "+ hit.collider.gameObject.name);
			}
		}
	}
}
