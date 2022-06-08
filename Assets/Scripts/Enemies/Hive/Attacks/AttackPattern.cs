using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackPattern", menuName = "AttackPatterns/AttackPattern", order = 1)]
public class AttackPattern : ScriptableObject
{
    public Attack[] attacks;

    public Attack GetAttack(int index)
    {
        return attacks[index];
    }

    public bool HasEnded(int index)
    {
        return index < attacks.Length;
    }
}
