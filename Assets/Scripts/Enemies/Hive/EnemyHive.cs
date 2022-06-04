using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHive : MonoBehaviour
{
    public List<EnemyController> enemies;
    List<Transform> enemyTransforms;

    HiveState startAttacks = new StartAttack();

    private void Awake()
    {
        enemyTransforms = new List<Transform>();
        foreach(EnemyController e in enemies)
        {
            enemyTransforms.Add(e.transform);
        }
        startAttacks.Init(this);
        startAttacks.Start();
    }

    public void Update()
    {
        startAttacks.MainLoop(Time.deltaTime);
    }

}
