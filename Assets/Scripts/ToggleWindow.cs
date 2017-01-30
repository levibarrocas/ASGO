using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleWindow : MonoBehaviour {
    private Button myselfButton;
    [SerializeField]
    int WindowNumber;

	// Use this for initialization
	void Start () {
        myselfButton = GetComponent<Button>();
        myselfButton.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update () {
		
	}
    void OnClick()
    {

        GetComponentInParent<WindowManager>().WindowSelected = WindowNumber;
    }

}
