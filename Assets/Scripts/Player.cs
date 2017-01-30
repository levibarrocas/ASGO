using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }

public class Player : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    public int Team;


    [SyncVar (hook = "OnStatusChange")]
    [SerializeField]
    public bool Dead = false;

    [SyncVar]
    [SerializeField]
    bool Respawned = false;

    [SyncVar(hook = "OnNameChanged")]
    [SerializeField]
    public string playerName;
    [SyncVar(hook = "OnColorChanged")]
    [SerializeField]
    public Color playerColor;


    [SerializeField]
    ToggleEvent onToggleShared;
    [SerializeField]
    ToggleEvent onToggleLocal;
    [SerializeField]
    ToggleEvent onToggleRemote;
    [SerializeField]
    float respawnTime = 5f;
    [SerializeField]
    GameObject BuyMenu;
    [SerializeField]
    GameObject TeamMenu;
    [SerializeField]
    RendererToggler RT;
    [SerializeField]
    bool MenuOpen;

    GameStateManager GM;

    static List<Player> players = new List<Player>();

    GameObject mainCamera;
    NetworkAnimator anim;

    bool ActivateRespawn = false;

    void Start()
    {
       
        anim = GetComponent<NetworkAnimator>();
        mainCamera = Camera.main.gameObject;
        
        EnablePlayer();
    }

    [ServerCallback]
    void OnEnable()
    {
        
        if (!players.Contains(this))
            players.Add(this);
    }

    [ServerCallback]
    void OnDisable()
    {
        if (players.Contains(this))
            players.Remove(this);
    }

    void Update()
    {
        if (!isLocalPlayer) {
            return;
        }

        //CmdRespawnProcess();
        //LocalRespawnProcess();
        
        if(ActivateRespawn)
        {
            Respawn();
            ActivateRespawn = false;
            
        }

        if (MenuOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        anim.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
        anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));

        if(Team == 0)
        {
            OpenMenu(TeamMenu);
        }

        if (Input.GetButtonDown("Select Team"))
        {
            if (MenuOpen == false) { OpenMenu(TeamMenu); } else
            if (MenuOpen)  { if (TeamMenu.activeInHierarchy) { CloseOpenMenus(); } else { CloseOpenMenus(); OpenMenu(TeamMenu); } }
          
        }

        if (Input.GetButtonDown("Buy"))
        {
            if (MenuOpen == false) { OpenMenu(BuyMenu); } else
            if (MenuOpen) { if (BuyMenu.activeInHierarchy) { CloseOpenMenus(); } else { CloseOpenMenus(); OpenMenu(BuyMenu); } }

        }



    }

    //[Command]
    //void CmdRespawnProcess()
    //{

    //    if (GameStateManager.GM.Respawn)
    //    {
    //        if (Dead)
    //        {
    //            if (!Respawned)
    //            {
    //                Respawn();
    //                ActivateRespawn = true;
    //                Respawned = true;
    //                CmdStatusChange(false);
    //            }
    //        }
    //        if (!Dead)
    //        {

    //            if (!Respawned)
    //            {
    //                RoundoverAlive();
    //                ActivateRespawn = true;
    //                Respawned = true;

    //                Respawn();

    //            }
    //        }
    //    }

    //}


    
    //void LocalRespawnProcess()
    //{

    //    if (GameStateManager.GM.Respawn)
    //    {
    //        if (Dead)
    //        {
    //            if (!Respawned)
    //            {
    //                Respawn();
                    
    //            }
    //        }
    //        if (!Dead)
    //        {

    //            if (!Respawned)
    //            {
    //                RoundoverAlive();
    //                Respawn();
    //            }
    //        }
    //    }

    //}
    [Command]
    void CmdJoinCT()
    {
        if (Team == 0)
        {
            Die();
            Team = 1;
            GameStateManager.GM.OnPlayerTeamChange();
            playerColor = Color.cyan;
        }
        if(Team == 2)
        {
            Die();
            Team = 1;
            GameStateManager.GM.OnPlayerTeamChange();
            playerColor = Color.cyan;
        }

    }
    [Command]
    void CmdJoinTR()
    {
        if (Team == 0)
        {
            Die();
            Team = 2;
            GameStateManager.GM.OnPlayerTeamChange();
            playerColor = Color.yellow;
        }
        if (Team == 1)
        {
            Die();
            Team = 2;
            GameStateManager.GM.OnPlayerTeamChange();
            playerColor = Color.yellow;
        }

    }

    void DisablePlayer()
    {
        
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.HideReticule();
            mainCamera.SetActive(true);
        }
        Debug.Log("onToggleShared(false) activated on" + playerName);
        onToggleShared.Invoke(false);

        if (isLocalPlayer)

        { onToggleLocal.Invoke(false);
            Debug.Log("onToggleLocal(false) activated on" + playerName);
        }
        else
        {
            onToggleRemote.Invoke(false);
            Debug.Log("onToggleRemote(false) activated on" + playerName);
        }
            
    }

    void EnablePlayer()
    {
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.Initialize();
            mainCamera.SetActive(false);
        }

        onToggleShared.Invoke(true);

        if (isLocalPlayer)
            onToggleLocal.Invoke(true);
        else
            onToggleRemote.Invoke(true);
    }

    [Command]
    public void CmdStatusChange(bool Status)
    {

        Dead = Status;

    }

    public void Die()
    {
        if (isLocalPlayer || playerControllerId == -1)
            anim.SetTrigger("Died");

        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.WriteGameStatusText("You Died!");
            PlayerCanvas.canvas.PlayDeathAudio();
        }
        Debug.Log(playerName + " died.Trying to disable it now.");
        DisablePlayer();
        GetComponent<PlayerShooting>().RestartLoadout();
        CmdStatusChange(true);
        Respawned = false;
        GameStateManager.GM.CmdDied(Team);
    }

    public void RoundoverAlive()
    {

        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.WriteGameStatusText("You Died!");
            PlayerCanvas.canvas.PlayDeathAudio();
        }
        
        Debug.Log(playerName + " didnt die at end of round.Trying to disable it now.");
        DisablePlayer();
    }

    void OnStatusChange(bool value)
    {

        //if (!value)
        //{
        //    Respawn();
        //}
    }

    public Transform GetStartPositionTeams(int Team)
    {
        if (Team == 1)
        {
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("CT Spawn");
            return playerObjects[(int)Random.Range(0,(playerObjects.Length)-1)].transform;
        } else
        if (Team == 2)
        {
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("TR Spawn");
            return playerObjects[(int)Random.Range(0, (playerObjects.Length) - 1)].transform;
        } else
        {
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("TR Spawn");
            return playerObjects[(int)Random.Range(0, (playerObjects.Length) - 1)].transform;
        }


    }

    public void Respawn()
    {
        Debug.Log("Trying to respawn :" + playerName);
        if (isLocalPlayer || playerControllerId == -1) { anim.SetTrigger("Restart"); }
            

        if (isLocalPlayer)
        {
            Transform spawn = GetStartPositionTeams(Team);
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }
        Debug.Log("Trying to enable:" + playerName);
        EnablePlayer();
    }

    void OnNameChanged(string value)
    {
        playerName = value;
        gameObject.name = playerName;
        GetComponentInChildren<Text>(true).text = playerName;
    }

    void OnColorChanged(Color value)
    {
        playerColor = value;
        GetComponentInChildren<RendererToggler>().ChangeColor(playerColor);
    }

    [Server]
    public void Won()
    {
        for (int i = 0; i < players.Count; i++)
            players[i].RpcGameOver(netId, name);

        Invoke("BackToLobby", 5f);
    }

    [ClientRpc]
    void RpcGameOver(NetworkInstanceId networkID, string name)
    {
        DisablePlayer();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (isLocalPlayer)
        {
            if (netId == networkID)
                PlayerCanvas.canvas.WriteGameStatusText("You Won!");
            else
                PlayerCanvas.canvas.WriteGameStatusText("Game Over!\n" + name + " Won");
        }
    }

    void BackToLobby()
    {
        FindObjectOfType<NetworkLobbyManager>().SendReturnToLobby();
    }

    void OpenMenu(GameObject Menu)
    {

        MenuOpen = true;

        Menu.SetActive(true);
    }



    void CloseOpenMenus()
    {

        MenuOpen = false;

        TeamMenu.SetActive(false);
        BuyMenu.SetActive(false);
    }

}