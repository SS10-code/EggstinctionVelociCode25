using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MutationData
{
    static Sprite GetSprite(string name) =>
        Resources.LoadAll<Sprite>("Icons/StatusIcons").FirstOrDefault(s => s.name == name);

    public static List<Mutation> allMutations = new List<Mutation>
    {
        new Mutation {name = "CrackShot",type = MutationType.Attack, description = "Deal 25 damage but take 5 damage. +5 damage per level.",effectValue = 25f,icon = GetSprite("Bleeding")},
        
        new Mutation {name = "ShellRend",type = MutationType.Attack,description = "Deal 15 damage. +3 damage per level.", effectValue = 15f, icon = GetSprite("Bleeding")},
        
        new Mutation {name = "Eggsecution",type = MutationType.Attack,description = "Deal 50 damage but take 15 damage. +10 damage per level.", effectValue = 50f,icon = GetSprite("Bleeding")},



        new Mutation {name = "HardBoil",type = MutationType.Defend,description = "Take 50% less damage for 2 turns. +1 turn per level.",effectValue = 50f,icon = GetSprite("Stun")},
        
        new Mutation {name = "ShellShield",type = MutationType.Defend,description = "Take 10% less damage permanently. +2% per level.",effectValue = 10f,icon = GetSprite("Stun")},
        
        new Mutation {name = "Eggshell Guard",type = MutationType.Defend,description = "Block the next attack completely. Does not level.",effectValue = 1f,icon = GetSprite("Stun")},


        new Mutation {name = "SunnySide Surge",type = MutationType.Heal, description = "Restore 40 health. +5 health per level.",effectValue = 40f, icon = GetSprite("Healing")},
        
        new Mutation {name = "Egg Drop Soup",type = MutationType.Heal,description = "Restore 60 health, but skip next turn. +5 health per level.",effectValue = 60f,icon = GetSprite("Healing") },
        
        new Mutation {name = "Yolk Burst",type = MutationType.Heal,description = "Restore 35 health (or 55 if below 50% HP). +3/+5 health per level.",effectValue = 35f,icon = GetSprite("Healing")}
    };
}
