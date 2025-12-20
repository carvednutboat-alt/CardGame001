using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ConfigureGameManager
{
    [MenuItem("Tools/Setup/Configure GameManager")]
    static void SetupGameManager()
    {
        // 查找 GameManager
        GameManager gm = Object.FindObjectOfType<GameManager>();
        
        if (gm == null)
        {
            Debug.LogError("[ConfigureGameManager] 场景中没有找到 GameManager！");
            return;
        }
        
        Debug.Log("[ConfigureGameManager] 找到 GameManager，正在配置...");
        
        // 配置 MapConfig
        string mapConfigPath = "Assets/scripts/Data/Map/DefaultMapConfig.asset";
        MapConfig mapConfig = AssetDatabase.LoadAssetAtPath<MapConfig>(mapConfigPath);
        if (mapConfig != null)
        {
            gm.MapConfig = mapConfig;
            Debug.Log($"[ConfigureGameManager] MapConfig 已设置: {mapConfigPath}");
        }
        else
        {
            Debug.LogWarning($"[ConfigureGameManager] 未找到 MapConfig: {mapConfigPath}");
        }
        
        // 配置初始卡组
        string[] cardPaths = new string[]
        {
            "Assets/Resources/Cards/勇鸟猛攻.asset",
            "Assets/Resources/Cards/圣剑加拉廷.asset",
            "Assets/Resources/Cards/圣剑卡利班.asset",
            "Assets/Resources/Cards/天命之圣剑.asset",
            "Assets/Resources/Cards/强欲之壶.asset",
            "Assets/Resources/Cards/往日种种.asset",
            "Assets/Resources/Cards/死者苏生.asset",
            "Assets/Resources/Cards/火球.asset",
            "Assets/Resources/Cards/群鸦风暴.asset",
            "Assets/Resources/Cards/千具武.asset",
            "Assets/Resources/Cards/万具武.asset",
            "Assets/Resources/Cards/突袭.asset"
        };
        
        gm.CurrentDeck = new List<CardData>();
        
        foreach (string path in cardPaths)
        {
            CardData card = AssetDatabase.LoadAssetAtPath<CardData>(path);
            if (card != null)
            {
                gm.CurrentDeck.Add(card);
                Debug.Log($"[ConfigureGameManager] 添加卡牌: {card.cardName}");
            }
            else
            {
                Debug.LogWarning($"[ConfigureGameManager] 未找到卡牌: {path}");
            }
        }
        
        // 标记为已修改
        EditorUtility.SetDirty(gm);
        
        Debug.Log($"[ConfigureGameManager] 配置完成！CurrentDeck 包含 {gm.CurrentDeck.Count} 张卡。");
        EditorUtility.DisplayDialog("配置完成", 
            $"GameManager 已配置完成！\n卡组包含 {gm.CurrentDeck.Count} 张卡。", "确定");
    }
}
