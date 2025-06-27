using UnityEngine;

public class ChargerEnemy : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip chargeClip;

    public Animator characterAnimator;
    public float chargeSpeed = 12f;
    public float normalSpeed = 4f;
    public float chargeDuration = 1.0f;
    public float chargeCooldown = 3.0f;
    public float avoidDistance = 1f;
    public float repulsionRadius = 2f;
    public float repulsionStrength = 5f;
    public float rotationSpeed = 720f;
    public float pushForce = 10f;


    public LayerMask obstacleMask;
    public LayerMask repulsionMask;

    private Rigidbody2D rb;
    private Transform player;
    private Rigidbody2D playerRb;

    private enum State { CoolingDown, Charging, MovingNormal }
    private State currentState = State.CoolingDown;

    private float stateTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerRb = player.GetComponent<Rigidbody2D>();

        stateTimer = chargeCooldown;
        rb.freezeRotation = false;
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

    }

    void FixedUpdate()
    {
        stateTimer -= Time.fixedDeltaTime;

        switch (currentState)
        {
            case State.CoolingDown:
                rb.linearVelocity = Vector2.zero;
                rb.freezeRotation = true;
                if (stateTimer <= 0f)
                {
                    currentState = State.Charging;
                    stateTimer = chargeDuration;
                    if (audioSource != null && chargeClip != null)
                        audioSource.PlayOneShot(chargeClip);
                }
                break;

            case State.Charging:
                ChargeTowardsPlayer();
                rb.freezeRotation = false;
                if (stateTimer <= 0f)
                {
                    currentState = State.CoolingDown;
                    stateTimer = chargeCooldown;
                    rb.linearVelocity = Vector2.zero;
                }
                break;

            case State.MovingNormal:
                rb.freezeRotation = false;
                NormalMovement();
                break;
        }
        if (characterAnimator != null)
        {
            bool isMoving = rb.linearVelocity.magnitude > 0.1f;
            characterAnimator.SetBool("isMoving", isMoving);
        }
    }

    void ChargeTowardsPlayer()
    {
        Vector2 toPlayer = ((Vector2)player.position - rb.position).normalized;
        Vector2 finalDir;

        if (CanSeePlayer())
        {
            finalDir = toPlayer;
        }
        else
        {
            Vector2 repulsion = Repulsion(toPlayer);
            finalDir = (toPlayer + repulsion).normalized;
        }

        MoveAndRotate(finalDir, chargeSpeed);
    }

    void NormalMovement()
    {
        Vector2 toPlayer = ((Vector2)player.position - rb.position).normalized;
        Vector2 finalDir;

        if (CanSeePlayer())
        {
            finalDir = Steer(toPlayer);
        }
        else
        {
            Vector2 repulsion = Repulsion(toPlayer);
            Vector2 avoid = Steer(toPlayer);
            finalDir = (avoid + repulsion).normalized;
        }

        MoveAndRotate(finalDir, normalSpeed);
    }

    Vector2 Steer(Vector2 directionToPlayer)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, avoidDistance, obstacleMask);
        Vector2 moveDir = directionToPlayer;

        if (hit.collider != null && hit.collider.gameObject != player.gameObject)
        {
            Vector2 right = Vector2.Perpendicular(directionToPlayer);
            Vector2 left = -right;

            bool rightClear = !Physics2D.Raycast(transform.position, right, avoidDistance, obstacleMask);
            bool leftClear = !Physics2D.Raycast(transform.position, left, avoidDistance, obstacleMask);

            if (rightClear && !leftClear)
                moveDir = right;
            else if (leftClear && !rightClear)
                moveDir = left;
            else if (rightClear && leftClear)
                moveDir = right;
            else
                moveDir = -directionToPlayer;
        }

        return moveDir.normalized;
    }

    Vector2 Repulsion(Vector2 targetDir)
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, repulsionRadius, repulsionMask);
        Vector2 repulseTotal = Vector2.zero;
        int repulseCount = 0;

        foreach (var col in nearby)
        {
            if (col.gameObject == gameObject) continue;

            Vector2 away = (Vector2)(transform.position - col.transform.position);
            float dist = away.magnitude;
            if (dist > 0.01f)
            {
                repulseTotal += away.normalized / dist;
                repulseCount++;
            }
        }

        if (repulseCount == 0) return Vector2.zero;

        Vector2 averageRepulsion = repulseTotal.normalized;

        float alignment = Vector2.Dot(averageRepulsion, targetDir);
        if (alignment < -0.5f)
        {
            Vector2 sidestep = Vector2.Perpendicular(targetDir);
            return sidestep * repulsionStrength;
        }

        return averageRepulsion * repulsionStrength;
    }

    void MoveAndRotate(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction * speed;

        if (direction.sqrMagnitude > 0.001f)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);
        }
    }

    bool CanSeePlayer()
    {
        Vector2 toPlayer = player.position;
        float radius = 0.4f;

        Vector2[] offsets = new Vector2[]
        {
            Vector2.zero,
            Vector2.up * radius,
            Vector2.down * radius,
            Vector2.left * radius,
            Vector2.right * radius
        };

        foreach (Vector2 offset in offsets)
        {
            Vector2 origin = (Vector2)transform.position + offset;
            Vector2 dir = (Vector2)player.position - origin;
            RaycastHit2D hit = Physics2D.Raycast(origin, dir.normalized, dir.magnitude, obstacleMask);
            if (hit.collider != null)
                return false;
        }

        return true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && currentState == State.Charging)
        {
            Vector2 pushDir = (player.position - transform.position).normalized;

            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
            }

            if (characterAnimator != null)
                characterAnimator.SetTrigger("Attack");

            currentState = State.CoolingDown;
            stateTimer = chargeCooldown;
            rb.linearVelocity = Vector2.zero;
        }
    }

}
