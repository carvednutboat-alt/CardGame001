using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Lua脚本分析工具
/// 用于分析和迁移现有的Lua脚本到新系统
/// </summary>
public class LuaScriptAnalyzer : MonoBehaviour
{
    [Header("调试")]
    public bool AnalyzeOnStart = false;
    
    private void Start()
    {
        if (AnalyzeOnStart)
        {
            AnalyzeAllScripts();
        }
    }
    
    /// <summary>
    /// 分析所有Lua脚本
    /// </summary>
    public void AnalyzeAllScripts()
    {
        Debug.Log("=== 开始分析Lua脚本 ===");
        
        // 列出所有已知的脚本
        string[] scriptNames = new string[]
        {
            "c_base",
            "c0",
            "c1001",
            "c1002",
            "c1003"
        };
        
        foreach (var name in scriptNames)
        {
            AnalyzeScript(name);
        }
        
        Debug.Log("=== Lua脚本分析完成 ===");
    }
    
    /// <summary>
    /// 分析单个脚本
    /// </summary>
    private void AnalyzeScript(string scriptName)
    {
        TextAsset script = Resources.Load<TextAsset>($"Lua/{scriptName}");
        if (script == null)
        {
            Debug.LogWarning($"[LuaAnalyzer] 无法加载脚本: {scriptName}");
            return;
        }
        
        Debug.Log($"\n--- 分析脚本: {scriptName} ---");
        Debug.Log($"脚本内容:\n{script.text}");
        
        // 分析脚本结构
        AnalyzeScriptStructure(script.text, scriptName);
    }
    
    /// <summary>
    /// 分析脚本结构
    /// </summary>
    private void AnalyzeScriptStructure(string content, string scriptName)
    {
        var findings = new List<string>();
        
        // 检查是否有initial_effect函数
        if (content.Contains("initial_effect"))
        {
            findings.Add("✓ 包含 initial_effect 函数");
        }
        
        // 检查效果类型
        if (content.Contains("Effect.CreateEffect") || content.Contains("CreateEffect"))
        {
            findings.Add("✓ 创建了效果对象");
        }
        
        // 检查SetType
        if (content.Contains("SetType"))
        {
            findings.Add("✓ 设置了效果类型");
            
            if (content.Contains("EFFECT_TYPE_IGNITION"))
                findings.Add("  - 起动效果 (IGNITION)");
            if (content.Contains("EFFECT_TYPE_TRIGGER"))
                findings.Add("  - 触发效果 (TRIGGER)");
            if (content.Contains("EFFECT_TYPE_FIELD"))
                findings.Add("  - 场地效果 (FIELD)");
            if (content.Contains("EFFECT_TYPE_CONTINUOUS"))
                findings.Add("  - 持续效果 (CONTINUOUS)");
        }
        
        // 检查SetCode
        if (content.Contains("SetCode"))
        {
            findings.Add("✓ 设置了效果代码");
        }
        
        // 检查SetRange
        if (content.Contains("SetRange"))
        {
            findings.Add("✓ 设置了生效范围");
        }
        
        // 检查C-C-T-O
        if (content.Contains("SetCondition"))
            findings.Add("✓ 设置了Condition");
        if (content.Contains("SetCost"))
            findings.Add("✓ 设置了Cost");
        if (content.Contains("SetTarget"))
            findings.Add("✓ 设置了Target");
        if (content.Contains("SetOperation"))
            findings.Add("✓ 设置了Operation");
        
        // 检查是否使用Duel API
        if (content.Contains("Duel."))
        {
            findings.Add("✓ 使用了Duel API");
            
            if (content.Contains("Duel.Damage"))
                findings.Add("  - 造成伤害");
            if (content.Contains("Duel.Draw"))
                findings.Add("  - 抽牌");
            if (content.Contains("Duel.SendtoGrave"))
                findings.Add("  - 送入墓地");
        }
        
        // 输出分析结果
        Debug.Log($"分析结果:");
        foreach (var finding in findings)
        {
            Debug.Log($"  {finding}");
        }
        
        if (findings.Count == 0)
        {
            Debug.Log("  未检测到标准效果结构");
        }
    }
    
    /// <summary>
    /// 测试加载单个卡牌脚本
    /// </summary>
    public void TestLoadCard(int cardId)
    {
        Debug.Log($"\n=== 测试加载卡牌: {cardId} ===");
        
        CardData data = Resources.Load<CardData>($"Data/Card_{cardId}");
        if (data == null)
        {
            Debug.LogWarning($"无法加载CardData: {cardId}");
            return;
        }
        
        RuntimeCard card = new RuntimeCard(data);
        
        Debug.Log($"卡牌名称: {data.cardName}");
        Debug.Log($"卡牌ID: {data.id}");
        Debug.Log($"效果数量: {card.Effects.Count}");
        
        foreach (var effect in card.Effects)
        {
            Debug.Log($"  - 效果类型: {effect.EffectType}");
            Debug.Log($"  - 效果代码: {effect.EffectCode}");
            Debug.Log($"  - 描述: {effect.Description}");
        }
    }
}