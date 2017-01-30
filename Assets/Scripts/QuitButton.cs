using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour {
    private Button myselfButton;


    // Use this for initialization
    void Start()
    {
        myselfButton = GetComponent<Button>();
        myselfButton.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void OnClick () {
        Application.Quit();
	}
}
