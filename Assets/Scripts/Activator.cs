using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour {

    [SerializeField]
    GameObject Panel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Panel.activeSelf)
        {
            gameObject.SetActive(true);
        } else
        {
            gameObject.SetActive(false);
        }
	}
}
