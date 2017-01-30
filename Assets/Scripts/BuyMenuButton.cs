using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class BuyMenuButton : NetworkBehaviour
{
    private Button myselfButton;
    private PlayerCurrency PCurrency;
    [SerializeField]
    string WhatToBuy;

    void Start()
    {
        PCurrency = GetComponentInParent<PlayerCurrency>();
        myselfButton = GetComponent<Button>();
        myselfButton.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnClick()
    {
        if (WhatToBuy == "Smoke") { PCurrency.buySmoke(); }
        if (WhatToBuy == "AK47" ) { PCurrency.buyAk47(); }
        if (WhatToBuy == "AWP") { PCurrency.buyAWP(); }
        if (WhatToBuy == "Armor") { PCurrency.buyArmor(); }
        if (WhatToBuy == "Skorpion") { PCurrency.buySkorpion(); }

    }
}