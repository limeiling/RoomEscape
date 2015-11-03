using UnityEngine;
using System.Collections;

public class RayCastFoward : MonoBehaviour {

	

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		// create a ray cast that orignates from the cursor position
		Vector3 forward = transform.TransformDirection(Vector3.forward) * 5;
		RaycastHit hit;
		Debug.DrawRay (transform.position,forward, Color.red);
			if(Physics.Raycast(transform.position,forward, out hit)){
				print("Cursor's Direction "+transform.position.x+" "+transform.position.y+" "+transform.position.z+" "
				     + hit.collider.gameObject.name);
			}
	}
}
