using UnityEngine;

public enum MutationType { Attack, Defend, Heal }

[System.Serializable]
public class Mutation
{
    public string name;
    public MutationType type;
    public string description;
    public float effectValue;
    public int level = 1;
    public Sprite icon;

}
