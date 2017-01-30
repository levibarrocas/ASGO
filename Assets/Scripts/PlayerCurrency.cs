using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerCurrency : NetworkBehaviour
{


    [SyncVar(hook = "OnMoneyChanged")]
    public int Money = 1000;
    [SerializeField]
    Text MoneyCounter;

    PlayerShooting PS;
    PlayerHealth PH;
    GameObject BuyMenu;

    // Use this for initialization
    void Start()
    {
        PS = GetComponent<PlayerShooting>();
        PH = GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void buySmoke()
    {
        if (Money > 300)
        {
            PS.CmdSmokeAdd();
            CmdSpend(300);
        }


    }

    void OnMoneyChanged(int value)
    {
        MoneyCounter.text = value.ToString();
    }

    public void buyAk47()
    {
        if (Money > 2700)
        {
            PS.CmdChangeWeapon(1);
            CmdSpend(2700);
        }


    }


    public void buyAWP()
    {
        if (Money > 4750)
        {
            PS.CmdChangeWeapon(2);
            CmdSpend(4750);
        }


    }

    public void buySkorpion()
    {
        if (Money > 3200)
        {
            PS.CmdChangeWeapon(3);
            CmdSpend(3200);
        }


    }

    public void buyArmor()
    {
        if (Money > 650)
        {
            PH.CmdBuyArmor();
            CmdSpend(650);
        }


    }
    [Command]
    public void CmdGainMoney(int value)
    {
        Money += value;
    }

    [Command]
    void CmdSpend(int Value)
    {
        RemoveMoney(Value);

    }

    [Server]
    void RemoveMoney(int Value)
    {
        Money -= Value;

    }
}
