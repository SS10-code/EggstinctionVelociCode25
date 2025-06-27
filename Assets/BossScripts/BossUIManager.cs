using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BossUIManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackClip;
    public AudioClip healClip;

    [Header("UI References")]
    public GameObject mutationPanel;
    public Transform contentContainer;
    public GameObject mutationButtonPrefab;

    [Header("Main Buttons")]
    public Button attackButton;
    public Button defendButton;
    public Button healButton;

    [Header("Boss HP")]
    public Transform bossHPFillTransform;
    public float bossMaxHP = 100f;
    private float targetHP;
    public TMP_Text bossHP;

    [Header("Sprites")]
    public Animator Mage;
    public Animator Guard;
    public Animator Animal;


    [Header("Player Sprites")]
    public Animator Player;
    public Animator Player2;
    public Animator Player3;

    [Header("Turn System")]
    public bool isPlayerTurn = true;
    private int bossTurnCounter = 0;

    private float damageReductionPercent = 0f;
    private int reductionTurns = 0;
    private bool blockNextHit = false;
    private bool skipNextTurn = false;

    void Start()
    {
        bossMaxHP = GameState.bossMaxHP;
        targetHP = bossMaxHP;

        attackButton.onClick.AddListener(() => ShowMutations(MutationType.Attack));
        defendButton.onClick.AddListener(() => ShowMutations(MutationType.Defend));
        healButton.onClick.AddListener(() => ShowMutations(MutationType.Heal));

        mutationPanel.SetActive(false);
        UpdateButtonState();
    }

    void Update()
    {
        float scaleX = Mathf.Lerp(bossHPFillTransform.localScale.x, Mathf.Clamp01(targetHP / bossMaxHP), Time.deltaTime * 5f);
        bossHPFillTransform.localScale = new Vector3(scaleX, 1f, 1f);
        bossHP.text = $"{Mathf.RoundToInt(targetHP)}/{Mathf.RoundToInt(bossMaxHP)}";

        if (GameState.currentHP <= 0)
            SceneManager.LoadScene("DeathScene");
    }

    public void ShowMutations(MutationType type)
    {
        if (!isPlayerTurn || skipNextTurn) return;

        mutationPanel.SetActive(true);
        ClearExistingButtons();

        foreach (var m in GameState.acquiredMutations)
        {
            if (m.type != type) continue;

            GameObject btnObj = Instantiate(mutationButtonPrefab, contentContainer);

            var nameText = btnObj.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null) nameText.text = $"{m.name} Lv.{m.level}";

            var descText = btnObj.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();
            if (descText != null) descText.text = m.description;

            Button btn = btnObj.GetComponent<Button>();
            Mutation captured = m;

            btn.onClick.AddListener(() =>
            {
                UseMutation(captured);
                mutationPanel.SetActive(false);
            });
        }
    }

    void UseMutation(Mutation m)
    {
        int lv = Mathf.Max(1, m.level);
        float prevHP = targetHP;

        if (m.type == MutationType.Attack)
        {
            if(Player.gameObject.activeSelf)
            {
                Player.SetTrigger("Attack");
            }
            else if (Player2.gameObject.activeSelf)
            {
                Player2.SetTrigger("Attack");
            }
            else if (Player3.gameObject.activeSelf)
            {
                Player3.SetTrigger("Attack");
            }
        }


        switch (m.name)
        {
            case "CrackShot":
                if (audioSource != null && attackClip != null)
                    audioSource.PlayOneShot(attackClip);
                CameraShake.Instance.Shake(0.3f, 0.2f);
                targetHP -= 25 + (5 * (lv - 1));
                GameState.currentHP -= 5;
                break;
            case "ShellRend":
                if (audioSource != null && attackClip != null)
                    audioSource.PlayOneShot(attackClip);
                CameraShake.Instance.Shake(0.3f, 0.2f);
                targetHP -= 15 + (3 * (lv - 1));
                break;
            case "Eggsecution":
                if (audioSource != null && attackClip != null)
                    audioSource.PlayOneShot(attackClip);
                CameraShake.Instance.Shake(0.3f, 0.2f);
                targetHP -= 50 + (10 * (lv - 1));
                GameState.currentHP -= 15;
                break;

            case "HardBoil":
                CameraShake.Instance.Shake(0.2f, 0.1f);
                damageReductionPercent = 0.5f;
                reductionTurns = 2 + lv;
                break;
            case "ShellShield":
                CameraShake.Instance.Shake(0.2f, 0.1f);
                damageReductionPercent = 0.10f + 0.02f * (lv - 1);
                reductionTurns = 999;
                break;
            case "Eggshell Guard":
                CameraShake.Instance.Shake(0.2f, 0.1f);
                blockNextHit = true;
                break;

            case "SunnySide Surge":
                GameState.currentHP += 40 + (5 * (lv - 1));
                if (audioSource != null && healClip != null)
                    audioSource.PlayOneShot(healClip);

                GameState.currentHP += 40 + (5 * (lv - 1));
                break;
            case "Egg Drop Soup":
                GameState.currentHP += 40 + (5 * (lv - 1));
                if (audioSource != null && healClip != null)
                    audioSource.PlayOneShot(healClip);

                GameState.currentHP += 60 + (5 * (lv - 1));
                skipNextTurn = true;
                break;
            case "Yolk Burst":
                GameState.currentHP += 40 + (5 * (lv - 1));
                if (audioSource != null && healClip != null)
                    audioSource.PlayOneShot(healClip);

                float hpPercent = (float)GameState.currentHP / 100f;
                GameState.currentHP += (hpPercent < 0.5f) ? 55 + 5 * (lv - 1) : 35 + 3 * (lv - 1);
                break;
        }

        targetHP = Mathf.Max(0, targetHP);
        GameState.currentHP = Mathf.Clamp(GameState.currentHP, 0, 100);

        if (Mathf.Abs(targetHP - prevHP) > 0.01f)
            TriggerBossMoveAnimation();

        if (targetHP <= 0)
        {
            Invoke(nameof(HandleVictory), 1.5f);
            return;
        }

        isPlayerTurn = false;
        UpdateButtonState();
        Invoke(nameof(BossTurn), 1.5f);
    }


    void BossTurn()
    {
        if (bossTurnCounter % 2 == 0)
        {
            TriggerBossAttackAnimation();

            float damage = 5f + (GameState.bossMaxHP / 10f);

            if (blockNextHit)
            {
                blockNextHit = false;
            }
            else
            {
                float finalDamage = damage * (1f - damageReductionPercent);
                if (audioSource != null && attackClip != null)
                    audioSource.PlayOneShot(attackClip);
                GameState.currentHP -= Mathf.RoundToInt(finalDamage);

                if (reductionTurns > 0)
                {
                    reductionTurns--;
                    if (reductionTurns == 0)
                        damageReductionPercent = 0f;
                }
            }

            if (GameState.currentHP <= 0)
                return;
        }
        else
        {
            float heal = 5f + (GameState.bossMaxHP / 10f);
            targetHP += heal;
            targetHP = Mathf.Min(targetHP, bossMaxHP);
            if (audioSource != null && healClip != null)
                audioSource.PlayOneShot(healClip);

            TriggerBossMoveAnimation();
        }

        bossTurnCounter++;
        isPlayerTurn = true;

        if (skipNextTurn)
        {
            skipNextTurn = false;
            isPlayerTurn = false;
            Invoke(nameof(BossTurn), 1.5f);
        }

        UpdateButtonState();
    }

    void TriggerBossAttackAnimation()
    {
        Mage.SetTrigger("Attack");
        Guard.SetTrigger("Attack");
        Animal.SetTrigger("Attack");
        if (audioSource != null && attackClip != null)
            audioSource.PlayOneShot(attackClip);

        CameraShake.Instance.Shake(0.3f, 0.3f);
        StartCoroutine(ResetToIdleAfter(0.5f));
    }

    void TriggerBossMoveAnimation()
    {
        Mage.SetBool("Move", true);
        Guard.SetBool("Move", true);
        Animal.SetBool("Move", true);
        StartCoroutine(ResetMoveBoolAfter(0.5f));
    }

    IEnumerator ResetToIdleAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        Mage.ResetTrigger("Attack");
        Guard.ResetTrigger("Attack");
        Animal.ResetTrigger("Attack");
    }

    IEnumerator ResetMoveBoolAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        Mage.SetBool("Move", false);
        Guard.SetBool("Move", false);
        Animal.SetBool("Move", false);
    }

    void UpdateButtonState()
    {
        bool enable = isPlayerTurn && !skipNextTurn;
        attackButton.interactable = enable;
        defendButton.interactable = enable;
        healButton.interactable = enable;

        if (!enable)
            mutationPanel.SetActive(false);
    }

    void ClearExistingButtons()
    {
        foreach (Transform child in contentContainer)
            Destroy(child.gameObject);
    }

    public void OnBackPressed()
    {
        mutationPanel.SetActive(false);
    }

    void HandleVictory()
    {
        int currentStage = GameState.currentStageIndex;

        if (!GameState.defeatedBossStages.Contains(currentStage))
            GameState.defeatedBossStages.Add(currentStage);

        bossMaxHP *= 1.20f;
        GameState.bossMaxHP = bossMaxHP;

        var newStage = new List<List<MapNode>>();
        for (int path = 0; path < 5; path++)
        {
            var pathNodes = new List<MapNode>();
            for (int i = 0; i < 5; i++)
            {
                NodeType[] pool = {
                    NodeType.Fight, NodeType.Fight, NodeType.Fight,
                    NodeType.DNA, NodeType.DNA, NodeType.DNA,
                    NodeType.Heal, NodeType.Heal, NodeType.Rest
                };
                NodeType type = pool[Random.Range(0, pool.Length)];
                pathNodes.Add(new MapNode { type = type, completed = false });
            }
            newStage.Add(pathNodes);
        }

        GameState.stages.Add(newStage);
        GameState.currentStageIndex = GameState.stages.Count - 1;
        GameState.currentNodeIndex = 0;
        GameState.currentPath = -1;

        SceneManager.LoadScene("MapScene");
    }
}
