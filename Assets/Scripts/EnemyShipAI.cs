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
    public int spreadCount = 5; // Number of bullets in a shotgun spread
    public float spreadAngle = 45f; // Angle of the spread
    public bool useSpreadShot = false;

    private float lastShotTime;
    private int currentShotCount;
    private int currentBurstCount;
    public bool enableLooping = true;  // Enable or disable looping from the editor
    public float loopInterval = 5f;   // Time between loops
    public float loopDuration = 5f;   // Duration of each loop

    private float loopTimer = 0f;
    private bool isLooping = false;
    private float currentLoopTime = 0f;

    private enum State
    {
        Approaching,
        Shooting,
        Retreating
    }

    private State currentState = State.Approaching;

        void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (player == null) return; // Skip update if no player

        HandleLooping();
        HandleMovementAndShooting();
    }

    void HandleLooping()
    {
        if (!enableLooping) return;

        if (isLooping)
        {
            if (currentLoopTime < loopDuration)
            {
                // Continue looping
                PerformLoop();
                currentLoopTime += Time.deltaTime;
            }
            else
            {
                // End loop
                isLooping = false;
                currentLoopTime = 0;
            }
        }
        else
        {
            loopTimer += Time.deltaTime;
            if (loopTimer >= loopInterval)
            {
                // Start looping
                isLooping = true;
                loopTimer = 0;
            }
        }
    }

    void PerformLoop()
    {
        // Simulate a looping maneuver, such as a circular motion
        transform.RotateAround(player.position, Vector3.forward, 360 * Time.deltaTime / loopDuration);
    }

    void HandleMovementAndShooting()
    {
        if (isLooping) return; // Do not handle other actions when looping

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        LookAtPlayer();

        if (distanceToPlayer > approachDistance)
        {
            MoveTowards(player.position);
        }
        else if (distanceToPlayer <= retreatDistance)
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
        // Check for the type of shooting based on an enemy property or directly using a condition
        if (useSpreadShot) {
            AimAndShootSpread();  // Call a method similar to the previous example for spread shot
        } else {
            AimAndShoot();  // Straight shot
        }

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

    private void AimAndShootSpread()
{
    float angleStep = spreadAngle / (spreadCount - 1);  // Calculate the angle between each bullet in the spread
    float startingAngle = -spreadAngle / 2;            // Start from the leftmost bullet

    for (int i = 0; i < spreadCount; i++)
    {
        float angle = startingAngle + angleStep * i;    // Calculate the angle for each bullet
        Quaternion rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + angle);
        Vector3 spawnPosition = transform.position + transform.up * 0.5f; // Position a little ahead of the enemy to avoid self-collision

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, rotation);
        bullet.GetComponent<Bullet>().SetShooter(gameObject);
    }
}

    void AimAndShoot()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z);
        GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.up * 0.5f, rotation);
        bullet.GetComponent<Bullet>().SetShooter(gameObject);
    }

    void LookAtPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void MoveTowards(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    void MoveAwayFrom(Vector3 target)
    {
        Vector3 directionAwayFromPlayer = transform.position - target;
        transform.position += directionAwayFromPlayer.normalized * speed * Time.deltaTime;
    }
}