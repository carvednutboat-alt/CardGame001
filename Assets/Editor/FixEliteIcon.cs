using UnityEngine;
using UnityEditor;

public class FixEliteIcon : AssetPostprocessor
{
    // Run on load
    [InitializeOnLoadMethod]
    static void ForceFixEliteIcon()
    {
        string path = "Assets/Resources/elite_battle_icon.png";
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            if (importer.textureType != TextureImporterType.Sprite)
            {
                Debug.Log("Fixing Elite Icon Import Settings to Sprite...");
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.SaveAndReimport();
                Debug.Log("Elite Icon fixed!");
            }
        }
    }
}
