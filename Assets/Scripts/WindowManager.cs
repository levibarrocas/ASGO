using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour {
    [SerializeField]
    GameObject[] Windows;
    public int WindowSelected;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        for(int i = 0; i < Windows.Length; i++)
        {
            if (i == WindowSelected)
            {
                Windows[i].SetActive(true);
            }
            if (i != WindowSelected)
            {
                Windows[i].SetActive(false);
            }

        }

	}
}
