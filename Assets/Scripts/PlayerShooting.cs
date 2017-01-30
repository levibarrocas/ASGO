using UnityEngine;
using UnityEngine.Networking;

public class PlayerShooting : NetworkBehaviour
{
    [SyncVar]
    public int MaxAmmo = 30;

    [SyncVar(hook = "OnAmmoChanged")]
    public int Municao = 30;

    [SyncVar(hook = "OnWeaponChanged")]
    public int WeaponID = 0;

    [SyncVar(hook = "OnScoreChanged")]
    int score;

    [SyncVar]
    public int Smokes = 0;

    [SyncVar]
    float Dano = 15;
    [SerializeField]
    Gun[] GunArray;

    [SyncVar]
    int ShootingMode;

    [SerializeField]
    LayerMask ShootHitbox;

    [SerializeField]
    float shotCooldown = .2f;
    [SerializeField]
    int killsToWin = 5;
    [SerializeField]
    Transform firePosition;
    [SerializeField]
    int StarterWeapon = 0;

    [SerializeField]
    GameObject grenadePrefab;

    [SyncVar]
    bool SniperAble;
    [SerializeField]
    GameObject Scope;

    ShotEffectsManager shotEffects;
    AudioSource ReloadStartSound;
    AudioSource ReloadEndSound;
    [SerializeField]
    float shotSpread = 0;

    UnityStandardAssets.Characters.FirstPerson.FirstPersonController FPSControl;
    [SerializeField]
    float SpreadDecreaseMultiplier = 0.00001f;
    [SerializeField]
    float SpreadBaseMultiplier = 0.0001f;
    [SerializeField]
    float SpreadCurrentMultiplier = 0.0001f;
    [SerializeField]
    float Spread = 0;

    PlayerCurrency PC;

    Player player;
    float ellapsedTime;
    bool canShoot;
    [SerializeField]
    bool Reloading;
    [SerializeField]
    float ReloadTime = 3;
    [SerializeField]
    float elapsedReload;

    float randomOffset_x;
    float randomOffset_y;


    void Start()
    {
        FPSControl = GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        player = GetComponent<Player>();
        GunArray[StarterWeapon].GunObject.SetActive(true);
        shotEffects = GunArray[StarterWeapon].shotEffects;
        ReloadStartSound = GunArray[StarterWeapon].ReloadStartSound;
        ReloadEndSound = GunArray[StarterWeapon].ReloadEndSound;
        ReloadTime = GunArray[StarterWeapon].ReloadTime;
        shotCooldown = GunArray[StarterWeapon].ShotCooldown;
        PC = GetComponent<PlayerCurrency>();
        shotEffects.Initialize();



        if (isLocalPlayer)
            canShoot = true;
    }

    [ServerCallback]
    void OnEnable()
    {
        score = 0;
    }
    [Server]
   Vector3 SprayDirection(Vector3 Original)
    {
        float vx = Original.x + ((1 - 2 * Random.value) * shotSpread );
        float vy = Original.x + ((1 - 2 * Random.value) * shotSpread );
        float vz = Original.z;
        return transform.TransformDirection(vx, vy, vz);
    }

    

    void Update()
    {
        
        if (!canShoot)
            return;
        if (shotSpread > 0) { shotSpread -= 0.005f; }

        ellapsedTime += Time.deltaTime;
        if (Reloading) { elapsedReload += Time.deltaTime; }

        if (Spread > 0) {Spread -= SpreadDecreaseMultiplier; }
        if (elapsedReload > ReloadTime)
        {
            ReloadEndSound.Play();
            CmdReload(Municao);
            elapsedReload = 0;
            Reloading = false;
        }

        float accuracy = Spread;

        if (Spread > 0)
        {
            randomOffset_x = UnityEngine.Random.Range(-Spread, Spread);
            randomOffset_y = UnityEngine.Random.Range(-Spread, Spread);
        }
        else
        {
            randomOffset_x = 0;
            randomOffset_y = 0;
        }


            //Debug.Log(randomOffset_x + "," + randomOffset_y + "," + randomOffset_z);

            Vector3 direction = firePosition.forward;

        direction.x += randomOffset_x;
        direction.y += randomOffset_y;


        if (ShootingMode == 0)
        {
            if (Input.GetButton("Fire1") && ellapsedTime > shotCooldown && Municao > 0)
            {
                if (!Reloading)
                {
                    ellapsedTime = 0f;

                    CmdFireShot(firePosition.position, direction);
                    Spread += SpreadCurrentMultiplier;
                    SpreadCurrentMultiplier = SpreadCurrentMultiplier * SpreadBaseMultiplier;
                }

            }
        } else if (ShootingMode == 1)
        {
            if (Input.GetButtonDown("Fire1") && ellapsedTime > shotCooldown && Municao > 0)
            {
                if (!Reloading)
                {
                    ellapsedTime = 0f;

                    CmdFireShot(firePosition.position, direction);
                    Spread += SpreadCurrentMultiplier;
                    SpreadCurrentMultiplier = SpreadCurrentMultiplier * SpreadBaseMultiplier;
                }

            }
        }


        if (Input.GetKeyDown("i"))
        {
            CmdChangeWeapon(1);
        }

        if (SniperAble)
        {
            if (Input.GetButtonDown("Fire2")) { if (GetComponentInChildren<Camera>().fieldOfView != 10) {
                    GetComponentInChildren<Camera>().fieldOfView = 10;
                    Scope.SetActive(true);
                    FPSControl.m_MouseLook.XSensitivity = 0.5f;
                    FPSControl.m_MouseLook.YSensitivity = 0.5f;
                }
                else
                { GetComponentInChildren<Camera>().fieldOfView = 60;
                    Scope.SetActive(false);
                    FPSControl.m_MouseLook.XSensitivity = 2;
                    FPSControl.m_MouseLook.YSensitivity = 2;
                } }
            
        }

        if (Input.GetButton("Interact") && ellapsedTime > shotCooldown && Smokes > 0)
        {
            if (!Reloading)
            {
                ellapsedTime = 0f;
                CmdFireSmokeGrenade(firePosition.position, firePosition.rotation);
            }

        }
        if (Input.GetButtonDown("Reload"))
        {
            if(!Reloading)
            {
                ReloadStartSound.Play();
                Reloading = true;
            }
  
        }
    }

    public void BuySmoke()

    {
       
        CmdSmokeAdd();
    }

    public void RestartLoadout()
    {
        CmdChangeWeapon(0);
    }

    [Command]
    void CmdReload(int municao)
    {
        ReloadAmmo();
    }

    [Command]
    public void CmdSmokeAdd()
    {
        AddSmoke();
    }

    [Command]
    public void CmdChangeWeapon(int value)
    {
        ServerChangeWeapon(value);
    }

    [Command]
    void CmdFireShot(Vector3 origin,Vector3 direction)
    {
        
        RaycastHit hit;
        RemoveAmmo();
        Ray ray = new Ray(origin,direction);
        Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 1f);

        bool result = Physics.Raycast(ray, out hit, 50f,ShootHitbox);

        if (result)
        {
            //PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();
            PlayerHealth enemyBody = hit.transform.GetComponentInParent<PlayerHealth>();
            BodyTag tag = hit.transform.GetComponent<BodyTag>();
            

            if(tag != null)
            {
                Debug.Log("The shot hit the " + tag.BodyPart);
                if (tag.BodyPart == "Head")
                {
                    bool wasKillShot = enemyBody.TakeDamage(Dano * 2,0.75f);
                    Debug.Log("The shot activated the " + tag.BodyPart);
                    if (wasKillShot) { PC.CmdGainMoney(300); }

                }
                if(tag.BodyPart == "Torso")
                {
                    bool wasKillShot = enemyBody.TakeDamage(Dano, 0.75f);
                    Debug.Log("The shot hit the " + tag.BodyPart);
                    if (wasKillShot) { PC.CmdGainMoney(300); }

                }
                if (tag.BodyPart == "Legs")
                {
                    bool wasKillShot = enemyBody.TakeDamage(Dano / 2, 0.75f);
                    Debug.Log("The shot hit the " + tag.BodyPart);

                    if (wasKillShot) { PC.CmdGainMoney(300); }
                    
                }
            } else { Debug.Log("The shot didnt hit a body part,it hit " + hit.transform.name); }

            //if (enemy != null)
            //{
            //    bool wasKillShot = enemy.TakeDamage(Dano);
            //    Debug.Log("BODYSHOT");
            //    if (wasKillShot && ++score >= killsToWin)
            //        player.Won();
            //}
        }

        RpcProcessShotEffects(result, hit.point);
    }

    [Command]
    void CmdFireSmokeGrenade(Vector3 origin,Quaternion direction)
    {
    RemoveSmoke();
    var grenade = (GameObject)Instantiate(
    grenadePrefab,
    origin,
    direction
    
    );

        grenade.GetComponent<Rigidbody>().velocity = grenade.transform.forward * 15;
        NetworkServer.Spawn(grenade);

    }


    [ClientRpc]
    void RpcProcessShotEffects(bool playImpact, Vector3 point)
    {
        shotEffects.PlayShotEffects();

        if (playImpact)
            shotEffects.PlayImpactEffect(point);
    }

    //[ClientRpc]
    //void RpcProcessReloadSoundEffect(AudioSource AS)
    //{
    //    AS.Play();
    //}

    [Server]
    void RemoveAmmo()
    {
     
        Municao--;

    }

    [Server]
    void RemoveSmoke()
    {

        Smokes--;

    }

    [Server]
    void ServerChangeWeapon(int value)
    {

        WeaponID = value;
        MaxAmmo = GunArray[value].ClipSize;
        Municao = GunArray[value].ClipSize;
        ShootingMode = GunArray[value].ShootingMode;
        Dano = GunArray[value].Damage;
        SniperAble = GunArray[value].SniperScope;
        SpreadDecreaseMultiplier = GunArray[value].SpreadDecreaseMultiplier;
        SpreadBaseMultiplier = GunArray[value].SpreadBaseMultiplier;

    }

    [Server]
    void AddSmoke()
    {

       Smokes++;

    }

    [Server]
    void ChangeMaxAmmo(int value)
    {

        MaxAmmo = value;
        Municao = value;

    }

    [Server]
    void ReloadAmmo()
    {
       
        Municao = MaxAmmo;

    }


    void OnScoreChanged(int value)
    {
        score = value;
        if (isLocalPlayer)
            PlayerCanvas.canvas.SetKills(value);
    }

    void OnWeaponChanged(int value)
    {
        GunArray[value].GunObject.SetActive(true);
        shotEffects = GunArray[value].shotEffects;
        ReloadStartSound = GunArray[value].ReloadStartSound;
        ReloadEndSound = GunArray[value].ReloadEndSound;
        ReloadTime = GunArray[value].ReloadTime;
        shotCooldown = GunArray[value].ShotCooldown;
        shotEffects.Initialize();

        for (int i = 0;i < GunArray.Length; i++)
        {
            if(i != value)
            {
                GunArray[i].GunObject.SetActive(false);
            }
        }

    }

    void OnAmmoChanged(int value)
    {
        Municao = value;
        if (isLocalPlayer)
            PlayerCanvas.canvas.SetAmmo(value);
    }

    public void FireAsBot()
    {
        CmdFireShot(firePosition.position,firePosition.forward);
    }
}

[System.Serializable]
class Gun : System.Object
{

    [SerializeField]
    public string Name;
    [SerializeField]
    public bool SniperScope;
    [SerializeField]
    public int ShootingMode;
    [SerializeField]
    public float Damage;
    [SerializeField]
    public float ShotCooldown;
    [SerializeField]
    public float ReloadTime;
    [SerializeField]
    public int ClipSize;
    [SerializeField]
    public GameObject GunObject;
    [SerializeField]
    public ShotEffectsManager shotEffects;
    [SerializeField]
    public AudioSource ReloadStartSound;
    [SerializeField]
    public AudioSource ReloadEndSound;
    [SerializeField]
    public float SpreadBaseMultiplier;
    [SerializeField]
    public float SpreadDecreaseMultiplier;


}