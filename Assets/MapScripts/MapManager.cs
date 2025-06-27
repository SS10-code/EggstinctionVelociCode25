using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject nodeButtonPrefab;
    public GameObject bossButtonPrefab;
    public Transform contentParent;
    public GameObject stageContainerPrefab;
    public ScrollRect mapScrollRect;

    [Header("Node Sprites")]
    [SerializeField] private Sprite fightSprite;
    [SerializeField] private Sprite healSprite;
    [SerializeField] private Sprite restSprite;
    [SerializeField] private Sprite dnaSprite;
    [SerializeField] private Sprite bossSprite;
    [SerializeField] private Sprite startSprite;

    private List<GameObject> stageInstances = new List<GameObject>();

    void Start()
    {
        Random.InitState(System.Environment.TickCount);

        if (GameState.stages == null || GameState.stages.Count == 0)
            GenerateNewStage();

        CreateUI();
    }

    void GenerateNewStage()
    {
        var newStage = new List<List<MapNode>>();

        for (int path = 0; path < 5; path++)
        {
            var pathNodes = new List<MapNode>();
            for (int node = 0; node < 5; node++)
            {
                NodeType[] pool = (GameState.currentHP > 90) ?
                    new NodeType[] {
                        NodeType.Fight, NodeType.Fight, NodeType.Fight,
                        NodeType.DNA, NodeType.DNA, NodeType.DNA,
                        NodeType.Rest, NodeType.Rest
                    } :
                    new NodeType[] {
                        NodeType.Fight, NodeType.Fight, NodeType.Fight,
                        NodeType.DNA, NodeType.DNA, NodeType.DNA,
                        NodeType.Heal, NodeType.Heal, NodeType.Heal,
                        NodeType.Rest
                    };

                NodeType type = pool[Random.Range(0, pool.Length)];
                pathNodes.Add(new MapNode { type = type, completed = false });
            }

            newStage.Add(pathNodes);
        }

        if (GameState.stages == null)
            GameState.stages = new List<List<List<MapNode>>>();

        GameState.stages.Add(newStage);
        GameState.currentStageIndex = GameState.stages.Count - 1;
        GameState.currentPath = -1;
        GameState.currentNodeIndex = 0;
    }

    public void CreateUI()
    {
        foreach (var go in stageInstances)
            Destroy(go);
        stageInstances.Clear();

        for (int stageIndex = 0; stageIndex < GameState.stages.Count; stageIndex++)
        {
            var stage = GameState.stages[stageIndex];
            GameObject stageGO = Instantiate(stageContainerPrefab, contentParent);
            stageInstances.Add(stageGO);

            List<Transform> tierRows = new List<Transform>();
            for (int i = 0; i < 5; i++)
                tierRows.Add(stageGO.transform.Find($"TierRow{i}"));
            Transform bossRow = stageGO.transform.Find("BossRow");
            Transform startRow = stageGO.transform.Find("StartRow");


            GameObject startBtn = Instantiate(nodeButtonPrefab, startRow);
            startBtn.GetComponentInChildren<Text>().text = "Egg";
            startBtn.GetComponent<Button>().interactable = false;
            SetIconSprite(startBtn, startSprite);

            // Paths
            for (int tier = 0; tier < 5; tier++)
            {
                foreach (Transform t in tierRows[tier])
                    Destroy(t.gameObject);

                for (int path = 0; path < 5; path++)
                {
                    if (GameState.currentPath != -1 && path != GameState.currentPath)
                        continue;

                    MapNode node = stage[path][tier];
                    GameObject btn = Instantiate(nodeButtonPrefab, tierRows[tier]);
                    btn.GetComponentInChildren<Text>().text = node.type.ToString();
                    SetIconSprite(btn, GetSpriteForNode(node.type));

                    Button buttonComp = btn.GetComponent<Button>();
                    buttonComp.interactable = (stageIndex == GameState.currentStageIndex &&
                                               tier == GameState.currentNodeIndex);

                    int capturedStage = stageIndex;
                    int capturedPath = path;
                    int capturedTier = tier;

                    buttonComp.onClick.AddListener(() =>
                    {
                        if (GameState.currentPath == -1)
                            GameState.currentPath = capturedPath;
                        else if (GameState.currentPath != capturedPath)
                            return;

                        GameState.currentStageIndex = capturedStage;
                        GameState.currentNodeIndex = capturedTier;
                        EnterNode(node);
                    });
                }
            }

            foreach (Transform t in bossRow)
                Destroy(t.gameObject);

            GameObject bossBtn = Instantiate(bossButtonPrefab, bossRow);
            bossBtn.GetComponentInChildren<Text>().text = $"BOSS (Gen {stageIndex + 1})";
            SetIconSprite(bossBtn, bossSprite);

            Button bossButtonComp = bossBtn.GetComponent<Button>();

            bool bossCompleted = GameState.defeatedBossStages.Contains(stageIndex);
            bool bossInteractable = (stageIndex == GameState.currentStageIndex) &&
                                    GameState.currentPath != -1 &&
                                    stage[GameState.currentPath][4].completed &&
                                    !bossCompleted;

            bossButtonComp.interactable = bossInteractable;

            if (bossCompleted)
            {
                bossBtn.GetComponentInChildren<Text>().text += " (Complete)";
                ColorBlock colors = bossButtonComp.colors;
                colors.normalColor = Color.gray;
                colors.disabledColor = Color.gray;
                bossButtonComp.colors = colors;
            }

            bossButtonComp.onClick.AddListener(() =>
            {
                if (bossInteractable)
                    SceneManager.LoadScene("BossScene");
            });
        }
        //Scroll Poss
        if (mapScrollRect != null && PlayerPrefs.HasKey("ScrollPosition"))
        {
            float pos = PlayerPrefs.GetFloat("ScrollPosition");
            Canvas.ForceUpdateCanvases();
            mapScrollRect.verticalNormalizedPosition = pos;
        }
    }

    void EnterNode(MapNode node)
    {
        switch (node.type)
        {
            case NodeType.Fight:
                SceneManager.LoadScene("FightScene");
                break;
            case NodeType.Heal:
                GameState.currentHP += 20;
                GameState.currentHP = Mathf.Min(GameState.currentHP, 100);
                CompleteNode();
                break;
            case NodeType.Rest:
                CompleteNode();
                break;
            case NodeType.DNA:
                SceneManager.LoadScene("DNAScene");
                break;
        }
    }

    void CompleteNode()
    {
        if (mapScrollRect != null)
            PlayerPrefs.SetFloat("ScrollPosition", mapScrollRect.verticalNormalizedPosition);

        var stage = GameState.stages[GameState.currentStageIndex];
        stage[GameState.currentPath][GameState.currentNodeIndex].completed = true;
        GameState.currentNodeIndex++;

        if (GameState.currentNodeIndex >= 5)
            GameState.currentNodeIndex = 5;

        SceneManager.LoadScene("MapScene");
    }

    void SetIconSprite(GameObject button, Sprite sprite)
    {
        Transform icon = button.transform.Find("Icon");
        if (icon != null)
        {
            Image image = icon.GetComponent<Image>();
            if (image != null)
                image.sprite = sprite;
        }
    }

    Sprite GetSpriteForNode(NodeType type)
    {
        return type switch
        {
            NodeType.Fight => fightSprite,
            NodeType.Heal => healSprite,
            NodeType.Rest => restSprite,
            NodeType.DNA => dnaSprite,
            _ => null,
        };
    }
}
