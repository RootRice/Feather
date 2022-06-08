using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTest : MonoBehaviour
{
    //testing
    Color[] myColours = new Color[3] { Color.red, Color.blue, Color.black };
    public Damage myDamage;

    private void Start()
    {
        myDamage = new Damage(gameObject);
        Destroy(gameObject, 5f);
        int i = Random.Range(0, 4);
        if (i == 1)
        {
            myDamage.myType = Damage.Type.OneBlockable;
        }
        else if (i == 2)
        {
            myDamage.myType = Damage.Type.TwoBlockable;
        }
        else if (i == 3)
        {
            myDamage.myType = Damage.Type.Unblockable;
        }

        gameObject.GetComponent<MeshRenderer>().material.color = myColours[(int)myDamage.myType];

    }

    private void Update()
    {
        transform.position += Vector3.left * 5 * Time.deltaTime;
    }

}
