using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    public int maxHealth = 20;
    public int currentHealth;
    public GameObject deathEffectPrefab;

    public GameObject Holder;


    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Instantiate(deathEffectPrefab, gameObject.transform.position, gameObject.transform.rotation);

        if (Holder == null)
            Destroy(gameObject);
        else
            Destroy(Holder);

    }
}
