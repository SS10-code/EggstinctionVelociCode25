using UnityEngine;
using UnityEngine.SceneManagement;

public class FightNodeEnder : MonoBehaviour
{
    public float startTime = 3f;

    void Update()
    {
        startTime -= Time.deltaTime;
        if (startTime < 0)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject[] gunners = GameObject.FindGameObjectsWithTag("DamagesPlayer");

            if (enemies.Length == 0 && gunners.Length == 0)
            {

                SceneManager.LoadScene("DNAScene");
            }
        }
        if(GameState.currentHP <= 0)
        {
            SceneManager.LoadScene("DeathScene");
        }
    }
}
