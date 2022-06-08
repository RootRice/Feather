using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackProfile", menuName = "AttackPatterns/AttackProfile", order = 1)]
public class AttackProfile : ScriptableObject
{

    public AttackType[] attackTypes;
    public int numAttacks;

}
public enum AttackType { Red, Blue, Purple };
