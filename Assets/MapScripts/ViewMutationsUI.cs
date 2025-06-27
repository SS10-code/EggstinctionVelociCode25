using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewMutationsUI : MonoBehaviour
{
    public GameObject panel; 
    public Transform contentContainer; 
    public GameObject mutationEntryPrefab;
    public GameObject viewButton;
    public GameObject closeButton;

    void Start()
    {
        panel.SetActive(false);
    }

    public void OnViewMutationsPressed()
    {
        panel.SetActive(true);
        viewButton.SetActive(false);
        PopulatePanel();
    }

    public void OnClosePressed()
    {
        panel.SetActive(false);
        viewButton.SetActive(true);
        foreach (Transform child in contentContainer)
            Destroy(child.gameObject);
    }

    void PopulatePanel()
    {
        foreach (var m in GameState.acquiredMutations)
        {
            GameObject entry = Instantiate(mutationEntryPrefab, contentContainer);

            var nameText = entry.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null) nameText.text = m.name;

            var descText = entry.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();
            if (descText != null) descText.text = m.description;

            var levelText = entry.transform.Find("Level")?.GetComponent<TextMeshProUGUI>();
            if (levelText != null) levelText.text = "Level: " + m.level;
        }
    }
}
