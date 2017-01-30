using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameStateManager : NetworkBehaviour {

    public static GameStateManager GM;

    [SyncVar]
    [SerializeField]
    int MaxRound = 30;
    [SyncVar]
    [SerializeField]
    int CurrentRound = 1;

    [SyncVar]
    [SerializeField]
    int CTScore;
    [SyncVar]
    [SerializeField]
    int TRScore;

    //Round = 0 BuyTime;
    // ROund = 1 Match Prebomb;
    // Round = 2 Bomb;
    // Round = 3 Pos round
    [SyncVar]
    [SerializeField]
    int RoundState = 0;
    [SyncVar]
    [SerializeField]
    float BuyTime = 15;
    [SyncVar]
    [SerializeField]
    float RoundTime = 120;
    [SyncVar]
    [SerializeField]
    float BombTime = 40;
    [SyncVar]
    [SerializeField]
    float PostRoundTime = 10;
    [SyncVar]
    [SerializeField]
    bool BombPlanted;
    [SyncVar]
    [SerializeField]
    bool BombDefused;

    [SyncVar][SerializeField]
    public bool Respawn = false;

    [SyncVar]
    public bool Warmup;

    [SyncVar]
    public float WarmupTime;

    [SyncVar]
    [SerializeField]
    int NumberOfPlayers;
    [SyncVar]
    [SerializeField]
    public int TRPlayers;
    [SyncVar]
    [SerializeField]
    public int CTPlayers;
    [SyncVar]
    [SerializeField]
    int DeadTRPlayers;
    [SyncVar]
    [SerializeField]
    int DeadCTPlayers;


    GameObject[] Player;

    GameObject[] playerList = new GameObject[20];

    int ConnectedPlayers;

    void Awake()
    {
        if (GM == null)
            GM = this;
        else if (GM != this)
            Destroy(gameObject);
    }

    // Use this for initialization
    void Start() {

    }
    [Server]
    void AddPlayer(int team)
    {


    }
    [Server]
    void ChangePlayerTeam()
    {

    }

    [ClientRpc]
    void RpcUpdateScore()
    {
        if (Warmup)
        {
            PlayerCanvas.canvas.SetScoreandTime(WarmupTime, CTScore, TRScore, "Warmpup");
        }
        if (!Warmup)
        {
            if (RoundState == 0) { PlayerCanvas.canvas.SetScoreandTime(BuyTime, CTScore, TRScore, "Buy"); }
            if (RoundState == 1) { PlayerCanvas.canvas.SetScoreandTime(RoundTime, CTScore, TRScore, "Round"); }
            if (RoundState == 2) { PlayerCanvas.canvas.SetScoreandTime(BombTime, CTScore, TRScore, "bomb has been planted"); }
            if (RoundState == 3) { PlayerCanvas.canvas.SetScoreandTime(PostRoundTime, CTScore, TRScore, "Post round"); }
        }
    }

	// Update is called once per frame
    [Server]
        void OnPlayerConnected()
    {
        CTPlayers = 0;
        TRPlayers = 0;
        NumberOfPlayers = 0;
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerObjects.Length; i++)
        {
            if (playerObjects[i].GetComponent<Player>() != null)
            {
                NumberOfPlayers++;
                if (playerObjects[i].GetComponent<Player>().Team == 1)
                {
                    CTPlayers++;

                }
                if (playerObjects[i].GetComponent<Player>().Team == 2)
                {
                    TRPlayers++;
                }

            }
        }

    }
    [Command]
    public void CmdDied(int Team)
    {
        if(Team == 1)
        {
            DeadCTPlayers++;

        }
        if(Team == 2)
        {
            DeadTRPlayers++;
        }

    }

    


    [Server]
    void OnPlayerDisconnected()
    {
        CTPlayers = 0;
        TRPlayers = 0;
        NumberOfPlayers = 0;
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerObjects.Length; i++)
        {
            if (playerObjects[i].GetComponent<Player>() != null)
            {
                NumberOfPlayers++;
                if (playerObjects[i].GetComponent<Player>().Team == 1)
                {
                    CTPlayers++;

                }
                if (playerObjects[i].GetComponent<Player>().Team == 2)
                {
                    TRPlayers++;
                }

            }
        }

    }

    [Server]
    public void OnPlayerTeamChange()
    {
        CTPlayers = 0;
        TRPlayers = 0;
        NumberOfPlayers = 0;
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerObjects.Length; i++)
        {
            if (playerObjects[i].GetComponent<Player>() != null)
            {
                NumberOfPlayers++;
                if (playerObjects[i].GetComponent<Player>().Team == 1)
                {
                    CTPlayers++;

                }
                if (playerObjects[i].GetComponent<Player>().Team == 2)
                {
                    TRPlayers++;
                }
            }
        }
    }
    [ClientRpc]
    public void RpcRespawnPlayers()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerObjects.Length; i++)
        {
            if (playerObjects[i].GetComponent<Player>() != null)
            {
                playerObjects[i].GetComponent<Player>().Respawn();
            }
        }
   }
    [ClientRpc]
    public void RpcDisableAllAlivePlayers()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerObjects.Length; i++)
        {
            if (playerObjects[i].GetComponent<Player>() != null)
            {
                if(!playerObjects[i].GetComponent<Player>().Dead)
                {
                    playerObjects[i].GetComponent<Player>().RoundoverAlive();
                }
            }
        }
    }

    [Server]
	void Update () {
        //Debug.Log("Network Manager number" + NetworkManager.singleton.numPlayers);
        //Debug.Log("Game State Manager Number " + ConnectedPlayers);
        //Debug.Log("The tr team has :" + TRPlayers + " Members");
        //Debug.Log("The CT team has :" + CTPlayers + " Members");



        RpcUpdateScore();

        if (ConnectedPlayers != NetworkManager.singleton.numPlayers)
        {
            if (ConnectedPlayers > NetworkManager.singleton.numPlayers)
            {
                OnPlayerDisconnected();
                ConnectedPlayers = NetworkManager.singleton.numPlayers;
            }
            if (ConnectedPlayers < NetworkManager.singleton.numPlayers)
            {
                OnPlayerConnected();
                ConnectedPlayers = NetworkManager.singleton.numPlayers;

            }
        }

        if (NumberOfPlayers == (TRPlayers + CTPlayers))
        {
            WarmupTime -= Time.deltaTime;
            if (WarmupTime < 0)
            {
                Warmup = false;
            }
        }

        if (!Warmup)
        {

            if (RoundState == 0)
            {
                BuyTime -= Time.deltaTime;
                if (BuyTime >= 14.5)
                {
                    RpcDisableAllAlivePlayers();
                    
                }
                if (BuyTime >= 14)
                {
                    RpcRespawnPlayers();
                    
                }
                if(BuyTime <= 0)
                {
                    RoundState = 1;
                }
            }
            if (RoundState == 1)
            {
                Respawn = false;
                RoundTime -= Time.deltaTime;
                if (DeadCTPlayers >= CTPlayers)
                {
                    TRVictory();
                }
                else
                if (DeadTRPlayers >= TRPlayers)
                {
                    CTVictory();
                }
                else
                if (RoundTime <= 0)
                {
                    CTVictory();
                }
                else
                if (BombPlanted)
                {
                    RoundState = 2;
                }
            }
            if (RoundState == 2)
            {
                BombTime -= Time.deltaTime;
                if (BombTime <= 0)
                {
                    TRVictory();
                }
                else
                if (BombDefused)
                {
                    CTVictory();
                }
                else
                if (DeadCTPlayers >= CTPlayers)
                {
                    TRVictory();
                }

            }

            if (RoundState == 3)
            {
                PostRoundTime -= Time.deltaTime;
                if (PostRoundTime < 0)
                { RestartRound(); }

            }
        }

	}
    [Server]
    void CTVictory ()
    {
        CTScore++;
        RoundState = 3;

    }

    [Server]
    void RestartRound()
    {
    CurrentRound++;
    RoundState = 0;
    BuyTime = 15;
    RoundTime = 120;
    BombTime = 40;
    PostRoundTime = 10;
    BombPlanted = false;
    BombDefused = false;
    DeadCTPlayers = 0;
    DeadTRPlayers = 0;
    Respawn = true;
    
}

    [Server] 
    void TRVictory ()
    {
        TRScore++;
        RoundState = 3;

    }
}

