using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class GloveWrapper : MonoBehaviour {
	// Use this for initialization
	void Start () {
		AndroidJavaClass cls = new AndroidJavaClass("com.aau.bluetooth.cultar.glove.android.Glove");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
