using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class EnemyBehaviourVisualiser : Editor
{
    EnemyController manager;
    IdleState idleState;
    ChasingState chaseState;
    bool showAggroRadius = true;
    bool showChaseRadius = true;
    private void OnSceneGUI()
    {
        if (chaseState == null)
            return;
        if ((idleState == null))
            return;

        Draw();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUIStyle headStyle = new GUIStyle(GUI.skin.label);
        headStyle.fontSize = 15;
        GUILayout.Label("Idle Behaviour Options", headStyle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Show Aggro Radius", new GUILayoutOption[] { GUILayout.Width(135f) }))
        {
            showAggroRadius = !showAggroRadius;
            SceneView.RepaintAll();
        }
        GUILayout.Space(26f);
        if (GUILayout.Button("Show Chase Radius", new GUILayoutOption[] { GUILayout.Width(135f) }))
        {
            showChaseRadius = !showChaseRadius;
            SceneView.RepaintAll();
        }
        GUILayout.EndHorizontal();

    }
    private void OnEnable()
    {
        manager = (EnemyController)target;
        showAggroRadius = true;
        showChaseRadius = true;
        if (manager.idleState != null)
        {
            idleState = manager.idleState;
        }
        if (manager.chaseState != null)
        {
            chaseState = manager.chaseState;
        }
    }

    void Draw()
    {

        DrawAggro();
        DrawChaseRange();

    }

    void DrawAggro()
    {
        if (showAggroRadius)
            return;
        Handles.color = new Color(0.8f, 0.2f, 0.1f);
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        Handles.DrawWireDisc(manager.transform.position, Vector3.up, idleState.detectionRadius, 2f);
        Handles.DrawWireDisc(manager.transform.position, Vector3.left, idleState.detectionRadius, 2f);
        Handles.DrawWireDisc(manager.transform.position, (Vector3.left + Vector3.up).normalized, idleState.detectionRadius, 2f);
        Handles.DrawWireDisc(manager.transform.position, (Vector3.right + Vector3.up).normalized, idleState.detectionRadius, 2f);

        Handles.color = Color.red;
        idleState.detectionRadius = Handles.ScaleValueHandle(idleState.detectionRadius, manager.transform.position + Vector3.right * idleState.detectionRadius, Quaternion.LookRotation(Vector3.right), 5.0f, Handles.ConeHandleCap, 1);
        if(idleState.detectionRadius < 0)
        {
            idleState.detectionRadius = 0;
        }
    }

    void DrawChaseRange()
    {
        if (showChaseRadius)
            return;
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(manager.transform.position, Vector3.up, chaseState.minDist, 2f);
        Handles.color = Color.blue;
        Handles.DrawWireDisc(manager.transform.position, Vector3.up, chaseState.maxDist, 2f);
        Handles.color = Color.red;
        chaseState.minDist = Handles.ScaleValueHandle(chaseState.minDist, manager.transform.position + Vector3.right * chaseState.minDist, Quaternion.LookRotation(Vector3.right), 5.0f, Handles.ConeHandleCap, 1);
        chaseState.maxDist = Handles.ScaleValueHandle(chaseState.maxDist, manager.transform.position + Vector3.right * chaseState.maxDist, Quaternion.LookRotation(Vector3.right), 5.0f, Handles.ConeHandleCap, 1);
        if(chaseState.maxDist < chaseState.minDist)
        {
            chaseState.maxDist = chaseState.minDist;
        }if(chaseState.minDist < 0)
        {
            chaseState.minDist = 0;
        }
    }


}
