using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DropPodAI : MonoBehaviour
{
    public Transform player;
    public float detectionDistance = 15f;
    public float approachSpeed = 5f;
    public float chargeSpeed = 20f;
    public float flashDuration = 1f;
    public SpriteRenderer spriteRenderer;
    public float spawnInvulnerabilityDuration = 2.0f; // Time after spawn during which the pod is invulnerable

    private bool isCharging = false;
    private bool isFlashing = false;
    private EnemyHealth enemyHealth;
    private float invulnerabilityTimer;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
        invulnerabilityTimer = spawnInvulnerabilityDuration;
    }

    void Update()
    {
        if (player == null || spriteRenderer == null) return;

        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isCharging && !isFlashing)
        {
            if (distanceToPlayer > detectionDistance)
            {
                LookAtPlayer();
                rb.velocity = transform.up * approachSpeed; // Move towards the player using Rigidbody
            }
            else if (!isFlashing)
            {
                StartCoroutine(InitiateCharge());
            }
        }
    }

    IEnumerator InitiateCharge()
{
    isFlashing = true;
    rb.velocity = Vector2.zero; // Stop movement before blinking and charging
    for (float timer = flashDuration; timer > 0; timer -= 0.1f)
    {
        spriteRenderer.enabled = !spriteRenderer.enabled;
        yield return new WaitForSeconds(0.1f);
    }
    spriteRenderer.enabled = true;
    isFlashing = false;
    isCharging = true;
    rb.velocity = transform.up * chargeSpeed; // Start moving in the captured direction
}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (invulnerabilityTimer > 0 && other.CompareTag("Boundary")) return; // Ignore boundaries during invulnerability

        if (other.CompareTag("Portal"))
        {
            return; // Handle portal logic separately
        }

        if (other.CompareTag("Enemy") || other.CompareTag("Player") || other.CompareTag("Boundary"))
        {
            EnemyHealth otherEnemyHealth = other.GetComponent<EnemyHealth>();
            if (otherEnemyHealth != null)
            {
                otherEnemyHealth.Die(); // Kill other enemy
            }
            enemyHealth.Die(); // Kill self
        }
    }

    private void LookAtPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}