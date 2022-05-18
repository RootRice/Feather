using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyState
{
    void Init();
    void MainLoop(float deltaTime);
    void EndState();

}
