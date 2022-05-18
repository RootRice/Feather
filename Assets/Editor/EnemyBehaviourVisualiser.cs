using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class EnemyBehaviourVisualiser : Editor
{
    EnemyController manager;
    IdleState idleState;
    bool showAggroRadius;
    private void OnSceneGUI()
    {
        if ((idleState == null))
            return;
        if (showAggroRadius)
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
        GUILayout.EndHorizontal();

    }
    private void OnEnable()
    {
        manager = (EnemyController)target;
        if (manager.idleState != null)
        {
            idleState = manager.idleState;
        }
    }

    void Draw()
    {
        Handles.color = Color.green;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        Handles.DrawWireDisc(manager.transform.position, Vector3.up, idleState.detectionRadius, 2f);
        Handles.DrawWireDisc(manager.transform.position, Vector3.left, idleState.detectionRadius, 2f);
        Handles.DrawWireDisc(manager.transform.position, (Vector3.left+Vector3.up).normalized, idleState.detectionRadius, 2f);
        Handles.DrawWireDisc(manager.transform.position, (Vector3.right+Vector3.up).normalized, idleState.detectionRadius, 2f);

        Handles.color = Color.red;
        idleState.detectionRadius = Handles.ScaleValueHandle(idleState.detectionRadius, manager.transform.position + Vector3.right* idleState.detectionRadius, Quaternion.LookRotation(Vector3.right), 3.0f, Handles.ConeHandleCap, 1);
    }

 
}
