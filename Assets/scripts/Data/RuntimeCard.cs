using System;
using System.Collections.Generic;
using XLua;

// 代表“手中的一张牌”，它是 CardData 的运行时包装
// 代表“手中的一张牌”，它是 CardData 的运行时包装
[Serializable]
[LuaCallCSharp]
public class RuntimeCard
{
    public string UniqueId { get; private set; } // 唯一ID
    public CardData Data { get; private set; }   // 原始配置

    public RuntimeCard(CardData data)
    {
        Data = data;
        UniqueId = Guid.NewGuid().ToString(); // 赋予唯一身份
        Effects = new List<Effect>();
        
        LoadScript();
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
}