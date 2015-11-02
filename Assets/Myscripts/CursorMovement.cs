using UnityEngine;
using System.Collections;

public class CursorMovement : MonoBehaviour {

	public float moveSpeed;

	//This is for the input source that can be changed to the glove.
	public void moveTo (float x, float y){
		transform.Translate (moveSpeed * x * Time.deltaTime, 
		                     moveSpeed * y * Time.deltaTime,
		                     0f);
	}

	// Use this for initialization
	void Start () {
		moveSpeed = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		moveTo (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
	}
}
