using UnityEngine;
using TMPro;

public class PlayerHealthBar : MonoBehaviour
{
    public GameObject Foreground;
    public TMP_Text healthText;
    private float displayedhealthFraction;

    void Update()
    {

        float reachFrac = (float)GameState.currentHP / 100f;
        displayedhealthFraction = Mathf.Lerp(displayedhealthFraction, reachFrac, Time.deltaTime * 10f);

        Vector3 scale = Foreground.transform.localScale;
        scale.x = displayedhealthFraction;
        Foreground.transform.localScale = scale;

        int showHP = Mathf.RoundToInt(displayedhealthFraction * 100f);
        healthText.text = $"{showHP} / 100";
    }
}
