using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HiveState
{
    public void Init(EnemyHive hive);
    public void Start();
    public void MainLoop(float deltaTime);
    public void Exit();
}
