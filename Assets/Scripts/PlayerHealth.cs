using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField]
    int maxHealth = 3;

    [SyncVar(hook = "OnHealthChanged")]
    float health;

    [SyncVar(hook = "OnArmorChanged")]
    float armor = 0;

    Player player;


    void Awake()
    {
        player = GetComponent<Player>();
    }

    [ServerCallback]
    void OnEnable()
    {
        health = maxHealth;
    }

    [Command]
    public void CmdBuyArmor()
    {
        armor = 100;
    }

    [Server]
    public bool TakeDamage(float Damage,float ArmorPenetration)
    {
        bool died = false;

        if (health <= 0)
            return died;
        if (armor > (Damage / ArmorPenetration))
        {
            health -= Damage * ArmorPenetration;
            armor -= Damage / ArmorPenetration;
        }
        else health -= Damage;
        died = health <= 0;

        RpcTakeDamage(died);

        return died;
    }

    [ClientRpc]
    void RpcTakeDamage(bool died)
    {
        if (isLocalPlayer)
            PlayerCanvas.canvas.FlashDamageEffect();

        if (died)
            player.Die();
    }

    void OnHealthChanged(float value)
    {
        health = value;
        if (isLocalPlayer)
            PlayerCanvas.canvas.SetHealth(value);
    }
    void OnArmorChanged(float value)
    {
        armor = value;
        if (isLocalPlayer)
            PlayerCanvas.canvas.SetArmor(value);
    }
}