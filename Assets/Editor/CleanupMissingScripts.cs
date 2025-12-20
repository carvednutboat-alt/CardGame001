using UnityEngine;
using UnityEditor;

public class CleanupMissingScripts
{
    [MenuItem("Tools/Cleanup/Remove Missing Scripts in Scene")]
    static void RemoveMissingScriptsInScene()
    {
        var objs = Object.FindObjectsOfType<GameObject>();
        int removedCount = 0;
        
        foreach (var obj in objs)
        {
            var components = obj.GetComponents<Component>();
            
            for (int i = components.Length - 1; i >= 0; i--)
            {
                if (components[i] == null)
                {
                    Debug.LogWarning($"Removing missing script from: {obj.name}", obj);
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
                    removedCount++;
                    break;
                }
            }
        }
        
        if (removedCount > 0)
        {
            Debug.Log($"[Cleanup] Removed {removedCount} missing scripts from scene objects.");
            EditorUtility.DisplayDialog("Cleanup Complete", 
                $"Removed {removedCount} missing script references.", "OK");
        }
        else
        {
            Debug.Log("[Cleanup] No missing scripts found.");
            EditorUtility.DisplayDialog("Cleanup Complete", 
                "No missing scripts found in the scene.", "OK");
        }
    }
}
