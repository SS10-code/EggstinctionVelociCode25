using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void OnPlayAgainPressed()
    {
        GameState.stages = new List<List<List<MapNode>>>();
        GameState.currentStageIndex = 0;
        GameState.currentPath = -1;
        GameState.currentNodeIndex = 0;
        GameState.acquiredMutations = new List<Mutation>();
        GameState.defeatedBossStages = new HashSet<int>();
        GameState.currentHP = 100;
        GameState.bossMaxHP = 100f;
        SceneManager.LoadScene("MapScene");
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }
}
