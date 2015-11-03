using UnityEngine;
using System.Collections;

public class RayCastFoward : MonoBehaviour {
	private GameObject currentObject; //The currently highlighted object
	private Color oldColor; //The old color of the currentObject
	private float highlightFactor = 0.7f; //How much should the object be highlighted

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		// create a ray cast that orignates from the cursor position
		Vector3 forward = transform.TransformDirection (Vector3.forward) * 5;
		RaycastHit hit;
		Debug.DrawRay (transform.position, forward, Color.red);
		if (Physics.Raycast (transform.position, forward, out hit)) {
			print ("Cursor's Direction " + transform.position.x + " " + transform.position.y + " " + transform.position.z + " "
				+ hit.collider.gameObject.name);

			if (currentObject == null) { //IF we haved no current object
				currentObject = hit.transform.gameObject; //save the object
				HighlightCurrentObject (); //and highlight it
			} else if (hit.transform != currentObject.transform) { //ELSE IF we have hit a different object
				RestoreCurrentObject (); //THEN restore the old object
				currentObject = hit.transform.gameObject; //save the new object
				HighlightCurrentObject (); //and highlight it
				
			}  
		} else //ELSE no object was hit
			RestoreCurrentObject (); //THEN restore the old object
	}

	private void HighlightCurrentObject() {
		Renderer r = currentObject.GetComponent(typeof(Renderer)) as Renderer;
		oldColor = r.material.GetColor("_Color");
		Color newColor = new Color(oldColor.r + highlightFactor, oldColor.g +highlightFactor, oldColor.b + highlightFactor, oldColor.a);
		r.material.SetColor("_Color", newColor);
	}
	
	//Restores the current object to it's formaer state.
	private void RestoreCurrentObject() {
		if (currentObject != null) { //IF we actually have an object to restore
			Renderer r = currentObject.GetComponent(typeof(Renderer)) as Renderer;
			r.material.SetColor("_Color", oldColor);
			currentObject = null;
		}
	}  
}
