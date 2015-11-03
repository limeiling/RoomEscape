using UnityEngine;
using System.Collections;

public class CursorRotation : MonoBehaviour {

	public GameObject cursorFaceDirection;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//directionToFace = destination - source;
		Vector3 directionToFace = cursorFaceDirection.transform.position - transform.position;

		transform.rotation = Quaternion.LookRotation (-directionToFace);
	}
}
