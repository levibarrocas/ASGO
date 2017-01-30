using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    public static PlayerCanvas canvas;

    [Header("Component References")]
    [SerializeField]
    Image reticule;
    [SerializeField]
    UIFader damageImage;
    [SerializeField]
    Text gameStatusText;
    [SerializeField]
    Text healthValue;
    [SerializeField]
    Text armorValue;
    [SerializeField]
    Text ammoValue;
    [SerializeField]
    Text clipValue;
    [SerializeField]
    Text killsValue;
    [SerializeField]
    Text logText;
    [SerializeField]
    AudioSource deathAudio;
    [SerializeField]
    AudioSource zawarudoAudio;

    [SerializeField]
    Text timeValue;
    [SerializeField]
    Text Team1ScoreValue;
    [SerializeField]
    Text Team2ScoreValue;
    [SerializeField]
    Text RoundStatusText;

    //Ensure there is only one PlayerCanvas
    void Awake()
    {
        if (canvas == null)
            canvas = this;
        else if (canvas != this)
            Destroy(gameObject);
    }

    //Find all of our resources
    void Reset()
    {
        reticule = GameObject.Find("Reticule").GetComponent<Image>();
        damageImage = GameObject.Find("DamagedFlash").GetComponent<UIFader>();
        gameStatusText = GameObject.Find("GameStatusText").GetComponent<Text>();
        healthValue = GameObject.Find("HealthValue").GetComponent<Text>();
        killsValue = GameObject.Find("KillsValue").GetComponent<Text>();
        logText = GameObject.Find("LogText").GetComponent<Text>();
        deathAudio = GameObject.Find("DeathAudio").GetComponent<AudioSource>();



    }

    public void Initialize()
    {
        reticule.enabled = true;
        gameStatusText.text = "";
    }

    public void HideReticule()
    {
        reticule.enabled = false;
    }

    public void FlashDamageEffect()
    {
        damageImage.Flash();
    }

    public void PlayDeathAudio()
    {
        if (!deathAudio.isPlaying)
            deathAudio.Play();
    }

    public void PlayZaWarudoAudio()
    {
        if (!zawarudoAudio.isPlaying)
            zawarudoAudio.Play();
    }

    public void SetKills(int amount)
    {
        killsValue.text = amount.ToString();
    }

    public void SetHealth(float amount)
    {
        int intamount = (int)amount;
        healthValue.text = intamount.ToString();
    }

    public void SetArmor(float amount)
    {
        int intamount = (int)amount;
        armorValue.text = intamount.ToString();
    }

    public void SetAmmo(int amount)
    {
        ammoValue.text = amount.ToString();
    }

    public void WriteGameStatusText(string text)
    {
        gameStatusText.text = text;
    }

    public void WriteLogText(string text, float duration)
    {
        CancelInvoke();
        logText.text = text;
        Invoke("ClearLogText", duration);
    }

    public void SetScoreandTime(float Time,int Score1,int Score2,string RoundStatus)
    {
        int minutes = Mathf.FloorToInt(Time / 60F);
        int seconds = Mathf.FloorToInt(Time - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        timeValue.text = niceTime.ToString();
        Team1ScoreValue.text = Score1.ToString();
        Team2ScoreValue.text = Score2.ToString();
        RoundStatusText.text = RoundStatus;
    }

    void ClearLogText()
    {
        logText.text = "";
    }
}