using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private int damageDealt;
    [SerializeField] private int damageTaken;
    [SerializeField] private bool bulletDamage = true;
    [SerializeField] GameObject Player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("DamagesPlayer"))
        {
             collision.gameObject.GetComponent<EnemyHealthManager>().TakeDamage(damageDealt);
            GameState.currentHP -= damageTaken;
        }
        if (collision.gameObject.CompareTag("WeakShell"))
        {
            if (bulletDamage)
                GameState.currentHP -= 3;

            Destroy(collision.gameObject);
        }
    }
}
