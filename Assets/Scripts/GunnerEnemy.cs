using UnityEngine;

public class GunnerEnemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 2f;
    public float bulletSpeed = 5f;
    public GameObject PlayerEgg;
    public float rotationSpeed = 5f;
    public float bulletSpawnDistance = 0.5f;
    public Animator characterAnimator;

    private float fireTimer = 1f;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerEgg = player;

    }

    void Update()
    {

        FacePlayer();


        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            Fire();
            fireTimer = fireRate;
        }
    }

    void FacePlayer()
    {
        Vector2 direction = PlayerEgg.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
    }

    void Fire()
    {
        Vector2 shootDirection = transform.right.normalized;
        Vector2 spawnPosition = (Vector2)transform.position + shootDirection * bulletSpawnDistance;

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.Euler(0, 0, Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg));
        
        bullet.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * bulletSpeed;


        FacePlayer();
        characterAnimator.SetTrigger("Attack");
    }
}