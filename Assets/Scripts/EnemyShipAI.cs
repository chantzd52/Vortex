using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipAI : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab;
    public float approachDistance = 10f;
    public float retreatDistance = 15f;
    public float shootingDistance = 8f;
    public int shotsPerBurst = 3;
    public int numberOfBursts = 2;
    public float shootingRate = 1f;
    public float speed = 5f;

    private float lastShotTime;
    private int currentShotCount;
    private int currentBurstCount;
    

    private enum State
    {
        Approaching,
        Shooting,
        Retreating
    }

    private State currentState = State.Approaching;

    void Start()
    {
        // Find the player by tag and set it as the target
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (player == null)
        {
            return; // If player is not found, do nothing
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Always look at the player
        LookAtPlayer();

        // Handle movement and shooting based on distance to the player
        if (distanceToPlayer > approachDistance)
        {
            MoveTowards(player.position);
        }
        else if (distanceToPlayer > retreatDistance)
        {
            MoveTowards(player.position);
        }
        else
        {
            MoveAwayFrom(player.position);
        }

        if (distanceToPlayer <= shootingDistance)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (Time.time > lastShotTime + 1f / shootingRate && currentShotCount < shotsPerBurst)
        {
            AimAndShoot();
            lastShotTime = Time.time;
            currentShotCount++;
        }

        if (currentShotCount >= shotsPerBurst)
        {
            currentShotCount = 0;
            currentBurstCount++;
            if (currentBurstCount >= numberOfBursts)
            {
                currentState = State.Retreating;
                currentBurstCount = 0;
            }
        }
    }

    private void AimAndShoot()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90);

        GameObject bullet = Instantiate(bulletPrefab, transform.position + directionToPlayer, rotation);
        bullet.GetComponent<Bullet>().SetShooter(gameObject);
    }

    private void MoveTowards(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        LookAtPlayer();
    }

    private void MoveAwayFrom(Vector3 target)
    {
        Vector3 directionAwayFromPlayer = transform.position - target;
        transform.position += directionAwayFromPlayer.normalized * speed * Time.deltaTime;
        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        if (player == null) return; // Ensure player is valid before calculating the angle

        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}