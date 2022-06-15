using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHive : MonoBehaviour
{
    [SerializeField] List<EnemyController> enemies;
    List<Transform> enemyTransforms;
     public List<EnemyController> activeEnemies;


    float timer;
    HiveState startAttacks = new StartAttack();

    [SerializeField] AttackPattern pattern1;

    PatternSolver pSolver;

    private void Awake()
    {
        activeEnemies = new List<EnemyController>();
        enemyTransforms = new List<Transform>();
        foreach (EnemyController e in enemies)
        {
            enemyTransforms.Add(e.transform);
        }

        GetEnemyAbilities();
    }

    private void Start()
    {
        GetAttackers(pattern1);
        startAttacks.Init(this);
        startAttacks.Start();
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
        pSolver = new PatternSolver(enemyAttacks);
        pSolver.FindCombinations();
        
    }

    void GetAttackers(AttackPattern pattern)
    {
        int[] combo = new int[pattern.attacks.Length];
        for (int i = 0; i < combo.Length; i++)
        {
            combo[i] = (int)pattern.attacks[i].type;
        }

        int[][] enemiesToAttack = pSolver.FindCombo(combo);
        int n = enemiesToAttack.GetLength(0);
        for(int i = 0; i < n; i++)
        {
            activeEnemies.Add(enemies[enemiesToAttack[i][0]]);
            enemies[enemiesToAttack[i][0]].NewAttack((AttackType)enemiesToAttack[i][1], pattern.attackTimings[i]);
        }
    }

  
}
