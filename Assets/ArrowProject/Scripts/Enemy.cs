using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public enum EnemyStates { Idle,Attack,Die}
    [Header("Settings")]
    public int level;
    public float health;
    public float maxHealth;
    public float hitDamage;
    public float hitDamageDiff;
    public float xp;
    public float xpDiff;
    public float gold;
    public float goldDiff;
    public EnemyStates states;
    public Animator animator;
    public Player mainPlayer;

    [Header("UI")]
    public GameObject healthBarUI;
    public Slider slider;
    public Text damageTaken;

    [Header("AI")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;


    //private variable
    private Coroutine currentCoroutine;

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
        mainPlayer = FindObjectOfType<Player>();
        maxHealth = maxHealth * level;
        health = maxHealth;
        slider.value = CalculateHealth();
        healthBarUI.SetActive(false);
        damageTaken.gameObject.SetActive(false);
        //hitBox.SetActive(false);
    }

    private void Update()
    {
        CheckHealth();
        if (states == EnemyStates.Die)
        {

            healthBarUI.SetActive(false);
            StartCoroutine(StartDieAnimation());
            agent.isStopped = true;
        }
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        //Debug.Log(playerInAttackRange + " " + playerInSightRange);
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
        else if ((playerInSightRange && !playerInAttackRange) || getAttacked) ChasePlayer();
        else if (!playerInSightRange && !playerInAttackRange) Patroling();

        if (healthBarUI.activeSelf)
        {
            healthBarUI.transform.LookAt(player.position);
        }

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
        mainPlayer.Xperiences += Random.Range((int)(xp - xpDiff) , (int)(xp + xpDiff));
        mainPlayer.Golds += Random.Range((int)(gold - goldDiff), (int)(gold + goldDiff));
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Arrow")
        {
            int damageDeal = mainPlayer.DealDamage();
            TakeDamage(damageDeal);
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                StartCoroutine(DamageText(damageDeal));
            }
            else
            {
                currentCoroutine = StartCoroutine(DamageText(damageDeal));
            }
            getAttacked = true;
        }
    }

    private IEnumerator DamageText(int damage)
    {
        damageTaken.gameObject.SetActive(true);
        damageTaken.text = "- " + damage;
        yield return new WaitForSeconds(2f);
        damageTaken.gameObject.SetActive(false);
        damageTaken.text = "";

    }

    public void ActivateHit()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, 4f, whatIsPlayer);
        if (collider.Length > 0)
        {
            mainPlayer.Health -= Random.Range((int)(hitDamage - hitDamageDiff)*level,(int)(hitDamage + hitDamageDiff)*level);
        }
    }

}
