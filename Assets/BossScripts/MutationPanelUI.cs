using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MutationPanelUI : MonoBehaviour
{
    public GameObject mutationButtonPrefab;
    public Transform contentParent;           
    public GameObject panelRoot;           

    public Button closeButton;  

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
    }

    public void ShowMutations(MutationType type)
    {
        ClearExisting();

        List<Mutation> mutlist = GameState.acquiredMutations.FindAll(m => m.type == type);

        foreach (var mutation in mutlist)
        {
            GameObject btn = Instantiate(mutationButtonPrefab, contentParent);

            var nameText = btn.transform.Find("Name")?.GetComponent<Text>();
            if (nameText != null) nameText.text = mutation.name;

            var descText = btn.transform.Find("Description")?.GetComponent<Text>();
            if (descText != null) descText.text = mutation.description;

        }

        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }

    private void ClearExisting()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
}
