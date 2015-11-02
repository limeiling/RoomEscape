using UnityEngine;
using System.Collections;

public class RayCastFoward : MonoBehaviour {

	

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		// create a ray cast that orignates from the main camera position to cursor position
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		RaycastHit hit;
		Debug.DrawRay (transform.position,forward, Color.red);
			if(Physics.Raycast(transform.position,forward, out hit)){
				print("Camera's Direction "+transform.position.x+" "+transform.position.y+" "+transform.position.z+" "
				      + "Object's distance" + hit.collider.gameObject.name);
			}
	}
}
