using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// 编辑器工具：创建精英蒸汽指挥官遭遇战
/// 使用方法: Unity菜单 → Tools → Create Elite Robot Encounter
/// </summary>
public class CreateEliteRobotEncounter
{
    [MenuItem("Tools/Create Elite Robot Encounter")]
    public static void CreateEncounter()
    {
        // 确保目录存在
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Enemies"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Enemies");
        }

        // 创建 EnemyEncounterProfile 实例
        EnemyEncounterProfile encounter = ScriptableObject.CreateInstance<EnemyEncounterProfile>();
        
        // 配置遭遇战属性
        encounter.EncounterID = "Elite_SteamCommander";
        
        // 加载机器人卡牌 (使用正确的资源名称)
        encounter.Enemies = new List<CardData>();
        
        // 指挥官
        AddCardByName(encounter, "蒸汽机器人·指挥官");
        // 随从
        AddCardByName(encounter, "蒸汽哨兵");
        AddCardByName(encounter, "蒸汽收割者");
        AddCardByName(encounter, "过载增幅器");
        // 技能
        AddCardByName(encounter, "过载模式");
        AddCardByName(encounter, "二重过载");
        AddCardByName(encounter, "极限运转");
        AddCardByName(encounter, "过载释放");
        
        // 保存为资源文件
        string path = "Assets/Resources/Enemies/EliteSteamCommander.asset";
        AssetDatabase.CreateAsset(encounter, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // 选中新创建的资源
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = encounter;
        
        Debug.Log($"[Editor] 成功创建精英遭遇战: {encounter.EncounterID} at {path}");
        Debug.Log($"[Editor] 包含 {encounter.Enemies.Count} 张卡牌");

        // 注册到 EnemyDatabase
        RegisterToDatabase(encounter);
    }

    private static void AddCardByName(EnemyEncounterProfile profile, string cardName)
    {
        CardData card = Resources.Load<CardData>($"Cards/{cardName}");
        if (card != null)
        {
            profile.Enemies.Add(card);
        }
        else
        {
            Debug.LogWarning($"[CreateEncounter] 无法找到卡牌资源: Cards/{cardName}");
        }
    }

    private static void RegisterToDatabase(EnemyEncounterProfile newEncounter)
    {
        string[] guids = AssetDatabase.FindAssets("t:EnemyDatabase");
        if (guids.Length == 0)
        {
            Debug.LogError("[Editor] 找不到 EnemyDatabase.asset!");
            return;
        }

        string dbPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        EnemyDatabase db = AssetDatabase.LoadAssetAtPath<EnemyDatabase>(dbPath);

        if (db != null)
        {
            if (db.EliteEncounters == null) db.EliteEncounters = new List<EnemyEncounterProfile>();

            if (!db.EliteEncounters.Contains(newEncounter))
            {
                db.EliteEncounters.Add(newEncounter);
                EditorUtility.SetDirty(db);
                AssetDatabase.SaveAssets();
                Debug.Log($"[Editor] 已将 {newEncounter.EncounterID} 注册到 {db.name} 的精英战斗池中。");
            }
        }
    }
}
