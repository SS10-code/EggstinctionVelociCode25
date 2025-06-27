public enum NodeType { Fight, Heal, Rest, DNA, Boss }

[System.Serializable]
public class MapNode
{
    public NodeType type;
    public bool completed;
}
