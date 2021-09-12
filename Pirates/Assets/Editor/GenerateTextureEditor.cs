using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(GenerateTexture))]
public class GenerateTextureEditor : Editor
{

    private GenerateTexture gT;
    void OnEnable()
    {
        gT = (GenerateTexture)target;
        gT.LoadTerrains();
    }
    public override void OnInspectorGUI()
    {

        EditorGUILayout.GradientField("TerrainGradient", gT.terrainGradient);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New Terrain Type", GUILayout.MaxWidth(150f)))
        {
            TerrainTypeEditorPopUp.ShowWindow();
        };
        if (GUILayout.Button("Refresh Terrains", GUILayout.MaxWidth(150f)))
        {
            gT.LoadTerrains();
        };
        GUILayout.EndHorizontal();

        GUILayout.Space(5f);
        GUILayout.Label("Current Terrains", EditorStyles.boldLabel);
        GUILayout.Space(5f);
        foreach (TerrainTypes tT in gT.terrainTypes)
        {
            GUILayout.BeginVertical();
            EditorGUILayout.ObjectField(tT.terrainName, tT, typeof(TerrainTypes), true);
            tT.height = EditorGUILayout.FloatField("Height Range", tT.height);
            GUILayout.EndVertical();
            GUILayout.Space(10f);
        }

    }
}