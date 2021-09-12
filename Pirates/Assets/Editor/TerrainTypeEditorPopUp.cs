using UnityEngine;
using UnityEditor;

public class TerrainTypeEditorPopUp : EditorWindow
{
    string TerrainName;
    float height;
    bool walkable = true;
    bool sailable = true;
    Color color;


    [MenuItem("Window/TerrainType Editor")]
    public static void ShowWindow()
    {
        TerrainTypeEditorPopUp window = GetWindow<TerrainTypeEditorPopUp>("New TerrainType");
        window.maxSize = new Vector2(400, 180);
        window.minSize = window.maxSize;
        window.Show();
    }

    public void OnGUI()
    {
        GUILayout.Label("Add a new Terrail", EditorStyles.boldLabel);
        name = EditorGUILayout.TextField("Name", name);
        height = EditorGUILayout.FloatField("Height", height);
        color = EditorGUILayout.ColorField("Color", color);
        walkable = EditorGUILayout.Toggle("Walkable", walkable);
        sailable = EditorGUILayout.Toggle("Sailable", sailable);
        if (GUILayout.Button("Add Terrain"))
        {
            CreateNewTerrain(name, height, walkable, sailable, color);
            this.Close();
        }
    }
    public void CreateNewTerrain(string Name, float height, bool walkable, bool sailable, Color color)
    {
        TerrainTypes asset = ScriptableObject.CreateInstance<TerrainTypes>();
        asset.terrainName = Name;
        asset.height = height;
        asset.sailable = sailable;
        asset.walkable = walkable;
        asset.color = color;

        UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/Resources/TerrainTypes/" + Name + ".asset");
        UnityEditor.AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    void OnDestroy()
    {
        FindObjectOfType<GenerateTexture>().LoadTerrains();
    }
}