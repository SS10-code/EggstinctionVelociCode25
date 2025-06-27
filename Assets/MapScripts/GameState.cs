using System.Collections.Generic;

public static class GameState
{
    public static HashSet<int> defeatedBossStages = new HashSet<int>();
    public static float bossMaxHP = 100f;
    public static int currentHP = 100;
    public static int currentStageIndex = 0; 
    public static int currentPath = -1; 
    public static int currentNodeIndex = 0;
    public static List<List<List<MapNode>>> stages;
    public static List<Mutation> acquiredMutations = new List<Mutation>();
}
