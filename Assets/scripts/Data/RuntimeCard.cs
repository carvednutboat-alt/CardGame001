using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

// 代表“手中的一张牌”，它是 CardData 的运行时包装
// 代表“手中的一张牌”，它是 CardData 的运行时包装
[Serializable]
[LuaCallCSharp]
public class RuntimeCard
{
    public string UniqueId { get; private set; }
    public CardData Data { get; private set; }
    
    // Helper property for Lua
    public string Name => Data != null ? Data.cardName : "Unknown Card";
    
    // === 位置追踪（参考YGO） ===
    public int CurrentLocation;  // LOCATION_HAND, MZONE等
    public int CurrentSequence;  // 在该区域的序号（0-4）
    public int Owner;            // 0=玩家, 1=敌人
    public int Controller;       // 当前控制者
    public int PreviousLocation; // 上一个位置（用于效果判定）
    public int PreviousSequence;   // 原始配置
    public bool IsPendingResolved = false;

public RuntimeCard(CardData data)
    {
        Data = data;
        UniqueId = Guid.NewGuid().ToString();
        Effects = new List<Effect>();
        
        // 初始化位置（默认在牌库）
        CurrentLocation = Location.DECK;
        CurrentSequence = 0;
        Owner = 0; // 默认玩家
        Controller = 0;
        PreviousLocation = 0;
        PreviousSequence = 0;

        // === 新增：如果卡片有ID，就加载对应的Lua脚本 ===
        if (Data != null && Data.id > 0)
        {
            LoadScript();
            Debug.Log($"[RuntimeCard] Constructor complete for {Data.cardName}, Effects.Count = {Effects.Count}");
        }
    }

    // === Lua Integration ===
    public XLua.LuaTable Script;
    // 使用新的 Effect 类
    public List<Effect> Effects;

    public void LoadScript()
    {
        if (Data == null) 
        {
            Debug.LogError("[RuntimeCard] Data is null, cannot load script.");
            return;
        }

        int scriptId = Data.id;
        if (scriptId <= 0) 
        {
            return; 
        }

        if (LuaManager.Instance == null) 
        {
            Debug.LogError($"[RuntimeCard] LuaManager.Instance is null! Cannot load script for {Data.cardName} (ID: {scriptId}).");
            return;
        }

        string scriptName = "c" + scriptId;
        Debug.Log($"[RuntimeCard] Attempting to load script: {scriptName} for card {Data.cardName}");

        try
        {
            // 1. 创建该卡实例的 Lua 表
            Script = LuaManager.Instance.NewTable();
            
            // 2. 注入 'c' (self) 和 api
            Script.Set("c", this);
            
            // 3. 注入 RegisterEffect 方法供 Lua 调用
            Script.Set("register_effect", (Action<Effect>)RegisterEffect);

            // 4. 加载并执行脚本
            object[] results = LuaManager.Instance.DoString($"return require '{scriptName}'");
            
            if (results == null || results.Length == 0)
            {
                Debug.LogError($"[RuntimeCard] Lua require returned null/empty for {scriptName}");
                return;
            }
            
            if (!(results[0] is XLua.LuaTable meta))
            {
                Debug.LogError($"[RuntimeCard] Lua require didn't return a table for {scriptName}, got: {results[0]?.GetType()}");
                return;
            }
            
            var init = meta.Get<Action<RuntimeCard>>("initial_effect");
            if (init == null)
            {
                Debug.LogWarning($"[RuntimeCard] No initial_effect function found in {scriptName}");
                return;
            }
            
            Debug.Log($"[RuntimeCard] Invoking initial_effect for {Data.cardName}");
            init(this);
            Debug.Log($"[RuntimeCard] initial_effect completed for {Data.cardName}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[RuntimeCard] Exception loading script {scriptName}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public void RegisterEffect(Effect e)
    {
        e.OwnerCard = this;
        Effects.Add(e);
        UnityEngine.Debug.Log($"[RuntimeCard] Registered Effect: Code={e.EffectCode}, Type={e.EffectType} for {Data?.cardName}");
    }


    /// <summary>
    /// 更新卡牌位置
    /// </summary>
    public void UpdateLocation(int newLocation, int newSequence)
    {
        PreviousLocation = CurrentLocation;
        PreviousSequence = CurrentSequence;
        CurrentLocation = newLocation;
        CurrentSequence = newSequence;
    }
    
    /// <summary>
    /// 检查卡牌是否在指定位置
    /// </summary>
    public bool IsLocation(int location)
    {
        return (CurrentLocation & location) != 0;
    }
    
    /// <summary>
    /// 检查卡牌是否在场上
    /// </summary>
    public bool IsOnField()
    {
        return IsLocation(Location.MZONE) || IsLocation(Location.SZONE);
    }
}