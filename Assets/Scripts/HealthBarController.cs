using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    private int maxHealth;
    private int currentHealth;
    private float showFrac;

    public Image healthBar;
    public float lerpSpeed = 10f;

    void Start()
    {
        maxHealth = GetComponent<EnemyHealthManager>().maxHealth;
        currentHealth = GetComponent<EnemyHealthManager>().currentHealth;
        showFrac = (float)currentHealth / maxHealth;
    }

    void Update()
    {
        currentHealth = GetComponent<EnemyHealthManager>().currentHealth;

        float reachFrac = (float)currentHealth / maxHealth;
        showFrac = Mathf.Lerp(showFrac, reachFrac, Time.deltaTime * lerpSpeed);

        Vector3 scale = healthBar.transform.localScale;
        scale.x = showFrac;
        healthBar.transform.localScale = scale;
    }
}
