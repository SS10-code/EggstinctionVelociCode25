using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DNASceneManager : MonoBehaviour
{
    public Transform cardContainer;
    public GameObject cardPrefab;

    void Start()
    {
        if (GameState.acquiredMutations == null)
            GameState.acquiredMutations = new List<Mutation>();

        ShowMutationChoices();
    }

    void ShowMutationChoices()
    {
        List<Mutation> pool = new List<Mutation>(MutationData.allMutations);
        List<Mutation> selected = new List<Mutation>();

        for (int i = 0; i < 3 && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            selected.Add(pool[index]);
            pool.RemoveAt(index);
        }

        foreach (Mutation mutation in selected)
        {
            GameObject card = Instantiate(cardPrefab, cardContainer);

            int currentLevel = 0;
            foreach (var m in GameState.acquiredMutations)
            {
                if (m.name == mutation.name)
                {
                    currentLevel = m.level;
                    break;
                }
            }

            int nextLevel = currentLevel + 1;


            var nameText = card.transform.Find("Name")?.GetComponent<Text>();
            if (nameText != null) 
                nameText.text = mutation.name + $" To Lv.{nextLevel}";

            var descText = card.transform.Find("Description")?.GetComponent<Text>();
            if (descText != null) 
                descText.text = mutation.description + $"\n(Will become Lv.{nextLevel})";

            var iconImage = card.transform.Find("Icon")?.GetComponent<Image>();

            if (iconImage != null) 
                iconImage.sprite = mutation.icon;

            Button button = card.GetComponent<Button>();
            if (button == null)
                button = card.GetComponentInChildren<Button>();

            if (button == null)
                continue;

            button.onClick.AddListener(() =>
            {
                AddMutation(mutation);
                CompleteNode();
                SceneManager.LoadScene("MapScene");
            });
        }
    }

    void AddMutation(Mutation newMut)
    {
        foreach (Mutation m in GameState.acquiredMutations)
        {
            if (m.name == newMut.name)
            {
                m.level++;
                return;
            }
        }

        GameState.acquiredMutations.Add(new Mutation{name = newMut.name,description = newMut.description,icon = newMut.icon, type = newMut.type, effectValue = newMut.effectValue,level = 1});
    }

    void CompleteNode()
    {
        if (GameState.stages == null) return;

        int stageIndex = GameState.currentStageIndex;
        int pathIndex = GameState.currentPath;
        int nodeIndex = GameState.currentNodeIndex;

        if (stageIndex < 0 || stageIndex >= GameState.stages.Count) 
            return;
        if (pathIndex < 0 || pathIndex >= GameState.stages[stageIndex].Count) 
            return;
        if (nodeIndex < 0 || nodeIndex >= GameState.stages[stageIndex][pathIndex].Count) 
            return;

        GameState.stages[stageIndex][pathIndex][nodeIndex].completed = true;
        GameState.currentNodeIndex++;
    }
}
