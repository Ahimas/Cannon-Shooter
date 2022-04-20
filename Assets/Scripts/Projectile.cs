using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody projectileRb;
    [SerializeField] private float timeTillDestroy = 3f;
    [SerializeField] private float timeTillExplosion = 0.0f;
    [SerializeField] private float timeOfExplosion = 0.35f;
    private ParticleSystem explosionParticle;


    // Start is called before the first frame update
    void Start()
    {
        projectileRb = GetComponent<Rigidbody>();
        this.gameObject.SetActive(false);
        explosionParticle = GetComponentInChildren<ParticleSystem>();

    }

    public void Fire(float power)
    {
        projectileRb.velocity = Vector3.zero;
        projectileRb.AddForce(transform.up * power, ForceMode.Impulse);
        StartCoroutine(CountToDestroy(timeTillDestroy));
    }

    IEnumerator CountToDestroy(float time)
    {
        yield return new WaitForSeconds(time);

        projectileRb.velocity = Vector3.zero;

        explosionParticle.transform.position = transform.position;

        explosionParticle.Play();
        GameObject.Find("AudioManager").GetComponent<AudioManager>().PlayExplosionSound();

        yield return new WaitForSeconds(timeOfExplosion);

        this.gameObject.SetActive(false);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Floor") && !collision.gameObject.CompareTag("Gun"))
        {
            
            StartCoroutine(CountToDestroy(timeTillExplosion));
        }
    }

}
    