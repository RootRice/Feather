using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHive : MonoBehaviour
{
    [SerializeField] List<EnemyController> enemies;
    List<Transform> enemyTransforms;
    [HideInInspector] public List<EnemyController> activeEnemies;


    float timer;
    HiveState startAttacks = new StartAttack();

    [SerializeField] AttackPattern pattern1;

    private void Awake()
    {
        enemyTransforms = new List<Transform>();
        foreach (EnemyController e in enemies)
        {
            enemyTransforms.Add(e.transform);
        }
        activeEnemies = new List<EnemyController>();
        startAttacks.Init(this);
        startAttacks.Start();
        GetEnemyAbilities();
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.0f)
            startAttacks.MainLoop(Time.deltaTime);


    }

    void GetEnemyAbilities()
    {
        int[][] enemyAttacks = new int[enemies.Count][];
        for(int i = 0; i < enemies.Count; i++)
        {
            enemyAttacks[i] = new int[enemies[i].attackProfile.attackTypes.Length];
            for(int ii = 0; ii < enemyAttacks[i].Length; ii++)
            {
                enemyAttacks[i][ii] = (int)enemies[i].attackProfile.attackTypes[ii];
                //Debug.Log("Enemy " + i + "'s " + ii + "th attack is: " + enemyAttacks[i][ii]);
            }
        }
        PatternSolver pSolver = new PatternSolver(enemyAttacks);
        pSolver.FindCombinations();
    }

  
}
