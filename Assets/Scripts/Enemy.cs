using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Animator enemyAnimator;
    private Rigidbody enemyRb;
    private GameObject target;
    private NavMeshAgent agent;
    
    private BoxCollider enemyCollider;
    private GameManager gameManager;
    [SerializeField] private ParticleSystem appearParticle;

    private Vector3 attackPosition;
    private Vector2 attackPosZRange = new Vector2(-15f, 15f);
    
    private float attackPosX = 10f;
    private float damageFromProjectile = 1000f;

    [SerializeField] private float speed = 3.5f;
    
    private float currentSpeed;

    public bool isHit;
    public bool isSpeedChecking;
    private bool isCelebrating = false;
    private bool isFisrtTimeSpawn = true;

    private Vector3 currentPos;
    private Vector3 prevPos;

    // Start is called before the first frame update
    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        enemyRb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Gun");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemyCollider = GetComponentInChildren<BoxCollider>();

        appearParticle.Play();
        gameManager.audioManager.PlayEnemyAppearenceSound();
        agent.speed = speed;
        isHit = false;
        isSpeedChecking = false;

        StartCoroutine(MoveToAttackPosition());

        isFisrtTimeSpawn = false;
        
    }

    private void FixedUpdate()
    {
        enemyCollider.transform.rotation = Quaternion.LookRotation(target.transform.position - enemyCollider.transform.position);
    }

    private void Update()
    {
        if ( gameManager.isGameOver && !isCelebrating )
        {
            agent.speed = 0f;
            enemyAnimator.SetTrigger("EnemyWin");
            isCelebrating = true;
        }

        if ( !gameManager.isGameOver && isHit && !isSpeedChecking)
        {
            isSpeedChecking = true;
            StartCoroutine(CheckSpeedAfterHit());
                        
        }
       
    }

    private void OnEnable()
    {
        if ( !isFisrtTimeSpawn )
        {
            appearParticle.Play();
            gameManager.audioManager.PlayEnemyAppearenceSound();
            enemyRb.velocity = Vector3.zero;
            agent.speed = speed;

            StartCoroutine(MoveToAttackPosition());
        }

    }

    private Vector3 GetRandomAttackPosition()
    {
        float attackPosZ = Random.Range(attackPosZRange.x, attackPosZRange.y);

        return new Vector3(attackPosX, transform.position.y, attackPosZ);
    }

    IEnumerator MoveToAttackPosition()
    {
        attackPosition = GetRandomAttackPosition();
        agent.SetDestination(attackPosition);

        while ( Vector3.Distance(this.transform.position, attackPosition) > 0.5f )
        {
            yield return null;
        }

        agent.SetDestination(target.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ( collision.gameObject.CompareTag("Projectile") )
        {
            Vector3 awayFromTarget = this.transform.position - collision.GetContact(0).point;
           
            transform.rotation = Quaternion.LookRotation(awayFromTarget);

            enemyAnimator.SetBool("EnemyHit", true);

            enemyRb.AddForce(awayFromTarget.normalized * damageFromProjectile, ForceMode.Impulse);
            
            agent.speed = 0f;
            
            isHit = true;


        } else if ( !collision.gameObject.CompareTag("Floor") && !collision.gameObject.CompareTag("Enemy") )
        {
            enemyRb.velocity = Vector3.zero;
            transform.Rotate( target.transform.position - this.transform.position);
            agent.speed = speed;
            enemyAnimator.SetBool("EnemyHit", false);
            isHit = false;
          
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag("Finish") && !gameManager.isGameOver )
        {
            gameManager.GameOver();
        }
    }

    IEnumerator CheckSpeedAfterHit()
    {
        prevPos = this.transform.position;
        yield return new WaitForSeconds(0.25f);
        currentPos = this.transform.position;

        if ( Vector3.Distance(currentPos, prevPos) < 5.0f )
        {
            agent.speed = speed;
            enemyAnimator.SetBool("EnemyHit", false);
            isHit = false;
        }

        isSpeedChecking = false;
        
    }

}
