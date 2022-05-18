using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(IdlePathManager))]
public class PointEditor : Editor
{
    [SerializeField, HideInInspector] IdlePathManager constructor;
    [SerializeField,HideInInspector] Points points;
    [SerializeField,HideInInspector] IdlePathManager.DrawType drawType;
    bool showPath = false;
    delegate void DrawInstruction(Vector3[] vector3s);
    DrawInstruction drawInstructions;
    private void OnSceneGUI()
    {
        if ((points == null) )
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
        GUILayout.Label("Path options", headStyle);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Show Path", new GUILayoutOption[] { GUILayout.Width(100f) }))
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

    void Input()
    {
        Event guiEvent = Event.current;
        Ray r = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        Plane plane = new Plane(Vector3.up, constructor.transform.position);
        float hitDist;
        plane.Raycast(r, out hitDist);

        Vector3 newPos = r.GetPoint(hitDist);
        if(guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            Undo.RecordObject(constructor, "Create Point");
            constructor.AddPoint(newPos);
        }
        
    }

    void Draw()
    {
        Handles.color = Color.red;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        for (int i = 0; i < points.NumPoints(); i++)
        {
            Vector3 newPos = Handles.FreeMoveHandle(points.points[i], Quaternion.identity, 0.5f, Vector3.zero, Handles.CylinderHandleCap);
            if(points.points[i] != newPos)
            {
                Undo.RecordObject(constructor, "Move Point");
                constructor.MovePoint(i, newPos);
            }
        }
        drawInstructions(points.points.ToArray());
    }

    void Create()
    {
        DrawInstruction[] instructions = new DrawInstruction[] { Lines, Bezier, Box };
        constructor.CreatePoints();
        points = constructor.GetPoints();
        drawInstructions = instructions[(int)constructor.drawType];
    }

    private void OnEnable()
    {
        DrawInstruction[] instructions = new DrawInstruction[] { Lines, Bezier, Box };
        constructor = (IdlePathManager)target;
        if (constructor.points != null)
        {
            points = constructor.points;
            drawInstructions = instructions[(int)constructor.drawType];
        }
    }

    void Box(Vector3[] vector3s)
    {
        Handles.color = Color.green;
        Vector3[] boxPoints = new Vector3[4];
        boxPoints[0] = vector3s[0];
        boxPoints[1] = new Vector3(vector3s[0].x, vector3s[0].y, vector3s[1].z);
        boxPoints[2] = vector3s[1];
        boxPoints[3] = new Vector3(vector3s[1].x, vector3s[0].y, vector3s[0].z);
        for(int i = 0; i < boxPoints.Length-1;i++)
        {
            Handles.DrawLine(boxPoints[i], boxPoints[i+1]);
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
