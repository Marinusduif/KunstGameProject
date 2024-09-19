using UnityEditor;
using UnityEngine;

public static class SaveMesh
{
    public static void Save(Mesh mesh, string path)
    {
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
    }
}
