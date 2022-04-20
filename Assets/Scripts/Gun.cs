using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Gun : MonoBehaviour
{
    private float horiznontalInput;

    private Vector2 gunYLimit = new Vector2(40f, 140f);
    private float powerMod = 25f;
    private float reloadTime = 1f;
    private int ammoQnty = 10;
    private bool isReadyForFire;

    [SerializeField] private float controlSpeed; 
    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform barrel;
    [SerializeField] private Slider power;
    [SerializeField] private ParticleSystem fireParticle;
    [SerializeField] private AudioClip fireAudio;

    private AudioSource audioSource;
    private GameManager gameManager;

    private List<Projectile> projectilePool = new List<Projectile>();

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
        horiznontalInput = 90f;
        PreparePjectiles();
        isReadyForFire = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CannonControl();       

    }
    void Update() 
    {
        FireControl();
    }

    void CannonControl()
    {
        Vector3 rot = transform.localEulerAngles;

        if (gameManager.control == ControlType.Mouse)
        {
            horiznontalInput += Input.GetAxis("Mouse X") * controlSpeed * Time.fixedDeltaTime;
        } else if (gameManager.control == ControlType.Arrows )
        {
            horiznontalInput += Input.GetAxis("Horizontal") * controlSpeed * Time.fixedDeltaTime;
        }

        horiznontalInput = Mathf.Clamp(horiznontalInput, gunYLimit.x, gunYLimit.y);
        
        transform.localEulerAngles = new Vector3(rot.x, horiznontalInput, rot.z);
    }

    private Projectile GetProjectileFromPool()
    {
        for ( int i = 0; i < projectilePool.Count; i++ )
        {
            if ( !projectilePool[i].gameObject.activeSelf )
            {
                projectilePool[i].gameObject.SetActive(true);

                return projectilePool[i];
            }
        }

        return null;
    }

    private void PreparePjectiles()
    {
        for ( int i = 0; i < ammoQnty; i++ )
        {
            projectilePool.Add(Instantiate(projectile));
        }
    }

    private void Fire()
    {
        Projectile proj = GetProjectileFromPool();

        proj.transform.position = barrel.position;
        proj.transform.rotation = barrel.rotation;
        fireParticle.Play();
        audioSource.PlayOneShot(fireAudio);
        proj.Fire(power.value);
        power.value = power.minValue;
        isReadyForFire = false;

    }

    private void Reload()
    {
        isReadyForFire = true;
    }

    void FireControl()
    {
        if (isReadyForFire && !gameManager.isGameOver)
        {
            if (gameManager.control == ControlType.Mouse)
            {
                if (Input.GetMouseButton(0) && power.value < power.maxValue)
                {
                    power.value += powerMod * Time.deltaTime;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    Fire();
                    Invoke("Reload", reloadTime);
                }
            }
            else if (gameManager.control == ControlType.Arrows)
            {
                if (Input.GetKey(KeyCode.Space) && power.value < power.maxValue)
                {
                    power.value += powerMod * Time.deltaTime;
                }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    Fire();
                    Invoke("Reload", reloadTime);
                }
            }

        }

    }

}
