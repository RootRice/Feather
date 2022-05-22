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

    [SerializeField, HideInInspector] IdleMovementType constructor;
    [SerializeField, HideInInspector] Points points;
    [SerializeField, HideInInspector] IdleMovementType.DrawType drawType;
    bool showPath = false;
    delegate void DrawInstruction(Vector3[] vector3s);
    DrawInstruction drawInstructions;
    private void OnSceneGUI()
    {
        if (chaseState == null)
            return;
        if ((idleState == null))
            return;
        if ((points == null))
            return;
        if (showPath)
            return;
        Draw();
        Input();
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

        headStyle.fontSize = 15;
        GUILayout.Label("Path options", headStyle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Show Path", new GUILayoutOption[] { GUILayout.Width(100f) }))
        {
            showPath = !showPath;
            SceneView.RepaintAll();


        }
        GUILayout.Space(25f);
        if (GUILayout.Button("Create Path", new GUILayoutOption[] { GUILayout.Width(100f) }))
        {
            Create();
            SceneView.RepaintAll();
        }
        GUILayout.EndHorizontal();

    }
    private void OnEnable()
    {
        manager = (EnemyController)target;
        showAggroRadius = true;
        showChaseRadius = true;
        showPath = true;
        if (manager.idleState != null)
        {
            idleState = manager.idleState;
        }
        if (manager.chaseState != null)
        {
            chaseState = manager.chaseState;
        }
        DrawInstruction[] instructions = new DrawInstruction[] { Lines, Bezier, Box };
        constructor = manager.idleState.movementType;
        if (constructor.patrol != null)
        {
            points = constructor.patrol;
            drawInstructions = instructions[(int)constructor.drawType];
        }
        else
        {
            constructor.patrol = new Points(manager.transform);
        }
    }

    void Draw()
    {

        DrawAggro();
        DrawChaseRange();

        Handles.color = Color.red;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        for (int i = 0; i < points.NumPoints(); i++)
        {
            Vector3 newPos = Handles.FreeMoveHandle(points.points[i], Quaternion.identity, 0.5f, Vector3.zero, Handles.CylinderHandleCap);
            newPos = new Vector3(newPos.x, manager.transform.position.y, newPos.z);
            if (points.points[i] != newPos)
            {
                Undo.RecordObject(constructor, "Move Point");
                constructor.patrol.MovePoint(i, newPos);
            }
        }
        drawInstructions(constructor.patrol.points.ToArray());
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
        if(chaseState.maxDist < chaseState.minDist + 1.5f)
        {
            chaseState.maxDist = chaseState.minDist+1.5f;
        }
        if(chaseState.minDist < 0)
        {
            chaseState.minDist = 0;
        }
    }

   




    void Input()
    {
        Event guiEvent = Event.current;
        Ray r = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        Plane plane = new Plane(Vector3.up, manager.transform.position);
        float hitDist;
        plane.Raycast(r, out hitDist);

        Vector3 newPos = r.GetPoint(hitDist);
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            Undo.RecordObject(constructor, "Create Point");
            constructor.patrol.AddPoint(newPos);
        }

    }



    void Create()
    {
        DrawInstruction[] instructions = new DrawInstruction[] { Lines, Bezier, Box };
        constructor.patrol = new Points(manager.transform);
        points = constructor.patrol;
        drawInstructions = instructions[(int)constructor.drawType];
    }



    void Box(Vector3[] vector3s)
    {
        Handles.color = Color.green;
        Vector3[] boxPoints = new Vector3[4];
        boxPoints[0] = vector3s[0];
        boxPoints[1] = new Vector3(vector3s[0].x, vector3s[0].y, vector3s[1].z);
        boxPoints[2] = vector3s[1];
        boxPoints[3] = new Vector3(vector3s[1].x, vector3s[0].y, vector3s[0].z);
        for (int i = 0; i < boxPoints.Length - 1; i++)
        {
            Handles.DrawLine(boxPoints[i], boxPoints[i + 1]);
        }
        Handles.DrawLine(boxPoints[boxPoints.Length - 1], boxPoints[0]);
    }

    void Bezier(Vector3[] vector3s)
    {

    }

    void Lines(Vector3[] vector3s)
    {
        Handles.color = Color.green;
        for (int i = 0; i < vector3s.Length - 1; i++)
        {
            Handles.DrawLine(vector3s[i], vector3s[i + 1]);
        }

        Handles.DrawLine(vector3s[vector3s.Length - 1], vector3s[0]);
    }
}
