using UnityEditor;
using UnityEngine;

public class TextureImportSettings : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;

        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);

        // Disable sprite cropping - use full rect instead of tight mesh
        settings.spriteMeshType = SpriteMeshType.FullRect;

        importer.SetTextureSettings(settings);
    }
}

public class ReimportAllTextures
{
    [MenuItem("Tools/Reimport All Textures")]
    static void ReimportTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");
        int total = guids.Length;

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            EditorUtility.DisplayProgressBar("Reimporting Textures", path, (float)i / total);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
}