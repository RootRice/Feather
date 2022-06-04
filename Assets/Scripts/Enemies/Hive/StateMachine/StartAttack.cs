using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAttack : HiveState
{
    EnemyHive hive;
    Transform player;
    float timer;
    float[] timers;
    float[] angles;
    bool[] spotTaken;
    bool[] test;
    int[] enemySpots;
    float angleVariation;
    public void Exit()
    {
        
    }

    public void Start()
    {
        SetTimers();
        SetAngles();
        enemySpots = new int[timers.Length];
    }

    void SetTimers()
    {
        timer = 0;
        timers = new float[hive.enemies.Count];
        for (int i = 0; i < timers.Length; i++)
        {
            timers[i] = Random.Range(0.1f, 0.7f);
        }
        timers[0] = 0;
        test = new bool[timers.Length];
    }

    void SetAngles()
    {
        angles = new float[timers.Length*2];
        spotTaken = new bool[angles.Length];
        float step = 360f / (angles.Length);
        angleVariation = step / 2.5f;
        for (int i = 0; i < angles.Length; i++)
        {
            angles[i] = step * i;
        }
    }

    public void Init(EnemyHive _hive)
    {
        hive = _hive;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void MainLoop(float deltaTime)
    {
        timer += deltaTime;
        List<EnemyController> enemies = hive.enemies;
        Vector3 playerPos = player.position;
        for(int i = 0; i < enemies.Count; i++)
        {
            Vector3 pos;
            float curAngle;
            float smallestAngle;
            int j;
            if (test[i])
            {
                pos = enemies[i].transform.position;
                curAngle = Vector3.SignedAngle(pos - playerPos, Vector3.forward, Vector3.up);
                if (Mathf.Abs(Mathf.DeltaAngle(curAngle, angles[enemySpots[i]])) > angleVariation*2)
                {
                    spotTaken[enemySpots[i]] = false;
                    smallestAngle = 10000;
                    j = 0;
                    for (int ii = 0; ii < angles.Length; ii++)
                    {
                        if (Mathf.Abs(Mathf.DeltaAngle(curAngle, angles[ii])) < smallestAngle && !spotTaken[ii])
                        {
                            j = ii;
                            smallestAngle = Mathf.Abs(Mathf.DeltaAngle(curAngle, angles[ii]));
                        }
                    }
                    spotTaken[j] = true;
                    enemies[i].SetState(EnemyController.EnemyStates.ReadyingAttack);
                    enemies[i].SetTargetAngle(angles[j] + Random.Range(-angleVariation, angleVariation));
                    enemySpots[i] = j;
                }
            }

            if (timer < timers[i] || test[i])
                continue;
            smallestAngle = 10000;
            j = 0;
            pos = enemies[i].transform.position;
            curAngle = Vector3.SignedAngle(pos - playerPos, Vector3.forward, Vector3.up);
            for (int ii = 0; ii < angles.Length; ii++)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(curAngle, angles[ii])) < smallestAngle && !spotTaken[ii])
                {
                    j = ii;
                    smallestAngle = Mathf.Abs(Mathf.DeltaAngle(curAngle, angles[ii]));
                }
            }
            spotTaken[j] = true;
            enemies[i].SetState(EnemyController.EnemyStates.ReadyingAttack);
            enemies[i].SetTargetAngle(angles[j] + Random.Range(-angleVariation, angleVariation));
            enemySpots[i] = j;
            test[i] = true;
        }
        

    }
}
