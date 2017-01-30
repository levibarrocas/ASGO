using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugFov : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Camera>().fieldOfView = 4;

    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(GetComponent<Camera>().fieldOfView);
	}
}
