using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {

        MapGenerator mapGenerator = (MapGenerator)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate World"))
        {
            mapGenerator.DrawMapInEditor();
        }
        if (GUILayout.Button("Save Current Mesh as Asset"))
        {
            SaveTextureFromMaterialAsAsset(mapGenerator.meshMaterial, "TerrainTexture", true);
            Mesh myMesh = mapGenerator.gameObject.GetComponent<MeshFilter>().sharedMesh;
            MeshData.SaveMeshAsAsset(myMesh, true, true);
        }
    }
    private static void SaveTextureFromMaterialAsAsset(Material material, string name, bool makeNewInstance)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Texture Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);
        Texture TextureToSave = (makeNewInstance) ? Object.Instantiate(material.mainTexture) as Texture : material.mainTexture;

        AssetDatabase.CreateAsset(TextureToSave, path);
        AssetDatabase.SaveAssets();
    }
}