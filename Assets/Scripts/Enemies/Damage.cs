using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Damage
{
    GameObject s;
    public Damage(GameObject obj)
    {
        s = obj;
    }
    public enum Type { OneBlockable, TwoBlockable, Unblockable};

    public Type myType;


    public void Blocked()
    {
        GameObject.Destroy(s);
    }

    public void Hit()
    {

    }

}
