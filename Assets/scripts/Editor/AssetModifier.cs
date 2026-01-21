using UnityEngine;
using UnityEditor;

public class AssetModifier
{
    [MenuItem("Tools/ModifyNewCard")]
    public static void Modify()
    {
        string oldPath = "Assets/Resources/Cards/过载模式 1.asset";
        string newPath = "Assets/Resources/Cards/过载释放.asset";
        
        string error = AssetDatabase.MoveAsset(oldPath, newPath);
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError("Move failed: " + error);
        }
        else
        {
            Debug.Log("Renamed to 过载释放.asset");
        }

        CardData card = AssetDatabase.LoadAssetAtPath<CardData>(newPath);
        if (card != null)
        {
            card.cardName = "过载释放";
            card.cost = 1;
            card.value = 5;
            card.effectType = CardEffectType.ReduceOverloadAndAOE;
            card.targetType = CardTargetType.Ally; // Target Ally to remove Overload
            // Ensure irrelevant fields are cleared if necessary, but duplicating OverloadMode is mostly fine.
            
            EditorUtility.SetDirty(card);
            AssetDatabase.SaveAssets();
            Debug.Log("Card properties updated.");
        }
        else
        {
            Debug.LogError("Could not load new card asset.");
        }
    }
}