using System;
using System.Collections.Generic;
using XLua;

// 代表“手中的一张牌”，它是 CardData 的运行时包装
// 代表“手中的一张牌”，它是 CardData 的运行时包装
[Serializable]
[LuaCallCSharp]
public class RuntimeCard
{
    public string UniqueId { get; private set; }
    public CardData Data { get; private set; }
    
    // === 位置追踪（参考YGO） ===
    public int CurrentLocation;  // LOCATION_HAND, MZONE等
    public int CurrentSequence;  // 在该区域的序号（0-4）
    public int Owner;            // 0=玩家, 1=敌人
    public int Controller;       // 当前控制者
    public int PreviousLocation; // 上一个位置（用于效果判定）
    public int PreviousSequence;   // 原始配置

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
        }
    }

    // === Lua Integration ===
    public XLua.LuaTable Script;
    // 使用新的 Effect 类
    public List<Effect> Effects;

    public void LoadScript()
    {
        if (LuaManager.Instance == null) return;
        
        // 假设脚本名为 c{id}，例如 c1001
        // 如果 Data.id 不存在或为 0，可以使用 Data.cardName 的 Hash 或其他方式
        int scriptId = Data.id;
        if (scriptId <= 0) return; 

        string scriptName = "c" + scriptId;

        // 1. 创建该卡实例的 Lua 表
        Script = LuaManager.Instance.NewTable();
        
        // 2. 注入 'c' (self) 和 api
        Script.Set("c", this);
        
        // 3. 注入 RegisterEffect 方法供 Lua 调用
        Script.Set("register_effect", (Action<Effect>)RegisterEffect);

        // 4. 加载并执行脚本
        // 假设 Lua 脚本结构是: 
        // local c1001 = {}
        // function c1001.initial_effect(c) ... end
        // return c1001
        
        object[] results = LuaManager.Instance.DoString($"return require '{scriptName}'");
        
        if (results != null && results.Length > 0 && results[0] is XLua.LuaTable meta)
        {
            var init = meta.Get<Action<XLua.LuaTable>>("initial_effect");
            if (init != null)
            {
                // 调用 initial_effect(c)
                // 这里我们传 Script (即 'c' 表) 进去，虽然 Lua 侧可以用 c:register_effect()
                init(Script); 
            }
        }
    }

    public void RegisterEffect(Effect e)
    {
        e.OwnerCard = this;
        Effects.Add(e);
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