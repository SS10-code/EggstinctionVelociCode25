using UnityEngine;
using System.Collections;

public class SentryEnemy : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip chargeClip;

    public float speed = 2f;
    public float arenaWidth = 16f;
    public float arenaHeight = 8f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;
    public float minMoveDistance = 1.5f;
    public float collisionCheckRadius = 0.5f;

    public Animator animator;             
    public Transform spriteVisual;        

    private Vector2 target;
    private bool isWaiting = false;
    private Collider2D selfCollider;
    private Vector3 originalSpriteScale;  

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        selfCollider = GetComponent<Collider2D>();

        originalSpriteScale = spriteVisual.localScale;

        StartCoroutine(PickNewAndWait());
    }

    void Update()
    {
        if (isWaiting)
        {
            SetIsMoving(false);
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        SetIsMoving(true);

        // flip logic
        Vector2 dir = target - (Vector2)transform.position;
        if (spriteVisual != null)
        {
            Vector3 scale = originalSpriteScale;
            if (dir.x > 0.05f)
                scale.x = -Mathf.Abs(originalSpriteScale.x); 

            else if (dir.x < -0.05f)
                scale.x = Mathf.Abs(originalSpriteScale.x);  


            spriteVisual.localScale = scale;
        }

        if (Vector2.Distance(transform.position, target) < 0.05f)
            StartCoroutine(PickNewAndWait());

    }

    IEnumerator PickNewAndWait()
    {
        isWaiting = true;
        SetIsMoving(false);

        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        int safety = 0;

        while (safety++ < 100)
        {
            Vector2 candidate = GetPoint();

            if (Vector2.Distance(transform.position, candidate) >= minMoveDistance &&
                PathCheck(transform.position, candidate))
            {
                target = candidate;
                isWaiting = false;

                audioSource.pitch = Random.Range(0f, 1f);
                audioSource.PlayOneShot(chargeClip);

                yield break;
            }
        }

        StartCoroutine(PickNewAndWait());
    }


    Vector2 GetPoint()
    {
        float x = Random.Range(-arenaWidth / 2f, arenaWidth / 2f);
        float y = Random.Range(-arenaHeight / 2f, arenaHeight / 2f);
        return new Vector2(x, y);
    }

    bool PathCheck(Vector2 from, Vector2 to)
    {
        Vector2 dir = (to - from).normalized;
        float dist = Vector2.Distance(from, to);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(from, collisionCheckRadius, dir, dist);
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider != selfCollider)
            {
                return false;
            }
        }

        return true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (animator != null)
                animator.SetTrigger("Attack");

            StartCoroutine(PickNewAndWait());
        }
    }

    void SetIsMoving(bool moving)
    {
        if (animator != null)
            animator.SetBool("isMoving", moving);
    }
}
