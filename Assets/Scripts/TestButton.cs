using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestButton : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        NetworkIdentity[] NI = GetComponents<NetworkIdentity>();

        for(int i = 0; i < NI.Length; i++)
        {
            Debug.Log("NetworkID" + NI[i].netId);

        }
	}
}
