using UnityEngine;

public class SmokeGrenade : MonoBehaviour
{
    int Impacts;

    float ElapsedTime;
    float Duration = 15;
    [SerializeField]
    ParticleSystem PS;
    Rigidbody RB;
    [SerializeField]
    RigidbodyConstraints Stop;
    private void Start()
    {
        RB = GetComponent<Rigidbody>();
    }


    private void OnCollisionEnter(Collision other)
    {

        Impacts++;
        
    }

    void Update()
    {
        if(Impacts > 1)
        {
            ElapsedTime += Time.deltaTime;
        }

        if(ElapsedTime > 2)
        {
            RB.constraints = Stop;
            PS.Play();
        }
        if(ElapsedTime > Duration)
        {
            Destroy(gameObject);
        }

    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        return 0f;
    }
}