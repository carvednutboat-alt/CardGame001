using UnityEngine;
using UnityEditor;

/// <summary>
/// 编辑器工具：创建列车通行证遗物
/// 使用方法: Unity菜单 → Tools → Create Train Pass Relic
/// 或自动在编辑器加载时创建（如果不存在）
/// </summary>
[InitializeOnLoad]
public class CreateTrainPassRelic
{
    static CreateTrainPassRelic()
    {
        // 编辑器加载时自动检查并创建
        EditorApplication.delayCall += () =>
        {
            string path = "Assets/Resources/Relics/TrainPass.asset";
            if (!AssetDatabase.LoadAssetAtPath<RelicData>(path))
            {
                CreateRelic();
            }
        };
    }

    [MenuItem("Tools/Create Train Pass Relic")]
    public static void CreateRelic()
    {
        // 创建 RelicData 实例
        RelicData relic = ScriptableObject.CreateInstance<RelicData>();
        
        // 配置遗物属性
        relic.relicId = "train_pass";
        relic.relicName = "列车通行证";
        relic.description = "一张神秘的列车通行证。持有它的人可以登上传说中的幽灵列车...\n\n（此遗物没有战斗效果，仅用于解锁特殊事件）";
        relic.effectType = RelicEffectType.无;
        relic.effectValue = 0;
        
        // 设置为不可购买（只能通过事件获得）
        relic.minPrice = 999;
        relic.maxPrice = 999;
        
        // 保存为资源文件
        string path = "Assets/Resources/Relics/TrainPass.asset";
        AssetDatabase.CreateAsset(relic, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // 选中新创建的资源
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = relic;
        
        Debug.Log($"[Editor] 成功创建遗物: {relic.relicName} at {path}");
    }
}
