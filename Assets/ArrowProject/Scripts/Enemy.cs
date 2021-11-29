using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public enum EnemyStates { Idle,Attack,Die}
    [Header("Settings")]
    public float health;
    public float maxHealth;
    public float hitDamage;
    public EnemyStates states;
    public Animator animator;
    public GameObject healthBarUI;
    public Slider slider;

    [Header("AI")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    bool getAttacked,alreadyAttack;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;


    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        health = maxHealth;
        slider.value = CalculateHealth();
        healthBarUI.SetActive(false);
    }

    private void FixedUpdate()
    {
        CheckHealth();
        if(states == EnemyStates.Die)
        {

            healthBarUI.SetActive(false);
            StartCoroutine(StartDieAnimation());
            agent.isStopped = true;
        }  
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInAttackRange && playerInSightRange) AttackPlayer();
        else if ((playerInSightRange && !playerInAttackRange) || getAttacked) ChasePlayer();
        else if (!playerInSightRange && !playerInAttackRange) Patroling();

        slider.value = CalculateHealth();

    }


    public IEnumerator Transition()
    {
        yield return new WaitForSeconds(4f);
        walkPointSet = true;
    }

    public IEnumerator StartDieAnimation()
    {
        PlayAnim("Die");
        yield return new WaitForSeconds(2f);
        DestroyEnemy();
    }

    //Health Functions

    public void CheckHealth()
    {
        states = (health <= 0) ? EnemyStates.Die : EnemyStates.Idle;
    }

    float CalculateHealth()
    {
        return health / maxHealth;
    }

    public void PlayAnim(string anim)
    {
        animator.SetTrigger(anim);
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            PlayAnim("Walking");
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            StartCoroutine(Transition());
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        healthBarUI.SetActive(true);

        PlayAnim("Chase");
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);
        if (!alreadyAttack)
        {
            PlayAnim("Attack");
            alreadyAttack = true;
            Invoke(nameof(ResetAttack), 1f);

        }
        else
        {
            PlayAnim("Idle");
        }
    }

    private void ResetAttack()
    {
        alreadyAttack = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Arrow")
        {
            TakeDamage(50);
            getAttacked = true;
        }
    }

}
