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

    void Update()
{
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    // Always look at the player
    LookAtPlayer();

    // Move towards the player regardless of the shooting
    if (distanceToPlayer > approachDistance)
    {
        MoveTowards(player.position);
    }
    else if (distanceToPlayer > retreatDistance) // Within approach distance but outside of retreat distance
    {
        // Enemy can either continue moving towards or stop moving if you want it to hover at this range
        MoveTowards(player.position); // Optional: Comment this out if you want the enemy to stop moving when close enough
    }
    else
    {
        // Retreat if too close
        MoveAwayFrom(player.position);
    }

    // Shoot if within shooting distance and the shooting conditions are met
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
                currentBurstCount = 0; // Reset burst count for next time
            }
        }
    }

private void AimAndShoot()
{
    Vector3 directionToPlayer = (player.position - transform.position).normalized;
    Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90);
    
    // Offset the spawn position forward by a small amount, e.g., 1 unit
    Vector3 spawnPosition = transform.position + directionToPlayer * 1f; // Adjust the multiplier to ensure clear separation

    GameObject bullet = Instantiate(bulletPrefab, spawnPosition, rotation);
    bullet.GetComponent<Bullet>().SetShooter(gameObject);  // Set the shooter to this enemy object
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
    Vector3 directionToPlayer = player.position - transform.position;
    float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f; // Adjusting by 90 degrees if needed
    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
}
}
