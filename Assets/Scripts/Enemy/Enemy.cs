using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class Enemy : MonoBehaviour
{
    [SerializeField] float HP = 100f;
    [SerializeField] float damage = 10f;
    [SerializeField] GameObject deathSmokePrefab;
    [SerializeField] GameObject tapok;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    private NavMeshAgent agent;
    private Transform player;
    private Animator anim;
    [SerializeField] bool boss;
    [SerializeField] Image hpbar;
    [SerializeField] private bool shootingEnemy;


    // Patroling
    [SerializeField] private Vector3 walkPoint;
    [SerializeField] private float walkPointRange;
    private bool walkPointSet;

    // Attacking
    [SerializeField] private float attackCooldown;
    private bool canAttack = true;
    [SerializeField] Transform bossAttackSpawnPoint;


    // States
    [SerializeField] private float sightRange = 20f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private bool playerInSightRange;
    [SerializeField] private bool playerInAttackRange;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (playerInAttackRange && canAttack)
        {
            Attack();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            Chase();
        }
        else
        {
            Patrol();
        }

        if (boss)
        {
            hpbar.fillAmount = HP * 0.00067f;
        }

    }

    void Patrol()
    {
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        else
        {
            SearchWalkPoint();
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    void SearchWalkPoint()
    {
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            walkPointSet = true;
        }
    }

    void Chase()
    {
        agent.SetDestination(player.position);
        anim.SetBool("running", true);
    }

    void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void Attack()
    {
        agent.SetDestination(transform.position);
        FaceTarget();
        anim.SetBool("attacking", true);
        anim.SetBool("running", false);
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    public void ResetAttackAnimation()
    {
        anim.SetBool("attacking", false);
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        anim.SetTrigger("damaged");
        if (HP <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        anim.SetTrigger("death");
        if (boss)
        {
            GameManager.gameManager.EndLevelMethod();
        }
    }

    public void DeathSmoke()
    {
        Instantiate(deathSmokePrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        Instantiate(deathSmokePrefab, transform.position + Vector3.up * 3, Quaternion.identity);
        Instantiate(deathSmokePrefab, transform.position + Vector3.up * 4, Quaternion.identity);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void CheckHit()
    {
        if (playerInAttackRange)
        {
            PlayerHP.player.GetDamage(damage);
        }
    }

    public void BossAttack()
    {
        Instantiate(tapok, bossAttackSpawnPoint.position, Quaternion.identity);
    }

    public void ShootSound()
    {
        if (shootingEnemy)
            SoundManager.soundManager.PlaySound(16);
    }
    public void StepSound()
    {
        SoundManager.soundManager.PlaySound(26);
    }
}
