using JetBrains.Annotations;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DeveloperTool))]
public class DeveloperEditor : Editor
{
    public override void OnInspectorGUI()
    { 
        DeveloperTool detool=(DeveloperTool)target;
        DrawDefaultInspector();
        GUILayout.Space(10); 
        GUI.backgroundColor = Color.green; 

        if (GUILayout.Button("生成测试", GUILayout.Height(30)))
        {
            detool.Inspecting(detool.id);
        }
        GUI.backgroundColor = Color.white;
    }
}