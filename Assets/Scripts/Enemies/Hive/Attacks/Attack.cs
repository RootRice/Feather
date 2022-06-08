using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "AttackPatterns/Attack", order = 1)]
public class Attack : ScriptableObject
{
    public AttackType type;
}
