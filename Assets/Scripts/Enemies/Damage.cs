using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Damage : MonoBehaviour
{
    public enum Type { OneBlockable, TwoBlockable, Unblockable};

    public Type myType;

    //testing
    Color[] myColours = new Color[3] { Color.red, Color.blue, Color.black };

    private void Start()
    {
        Destroy(gameObject, 5f);
        int i = Random.Range(0, 4);
        if(i == 1)
        {
            myType = Type.OneBlockable;
        }
        else if (i == 2)
        {
            myType = Type.TwoBlockable;
        }
        else if (i == 3)
        {
            myType = Type.Unblockable;
        }

        gameObject.GetComponent<MeshRenderer>().material.color = myColours[(int)myType];

    }

    private void Update()
    {
        transform.position += Vector3.left * 5 * Time.deltaTime;
    }
    public void Blocked()
    {
        Destroy(gameObject);
    }

    public void Hit()
    {

    }

}
