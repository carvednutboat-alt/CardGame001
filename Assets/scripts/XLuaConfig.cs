using System;
using System.Collections.Generic;
using XLua;

/// <summary>
/// xLua 配置类 - 指定需要生成绑定代码的类型
/// </summary>
public static class XLuaConfig
{
    // 需要在 Lua 中调用 C# 的委托类型
    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>()
    {
        typeof(Effect.ConditionDelegate),
        typeof(Effect.CostDelegate),
        typeof(Effect.TargetDelegate),
        typeof(Effect.OperationDelegate),
        typeof(RuntimeCardFilter),
        typeof(Action<Effect>),
        typeof(Action<RuntimeCard>),
    };

    // 需要在 C# 中调用 Lua 的类型
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>()
    {
        typeof(Effect),
        typeof(RuntimeCard),
        typeof(CardData),
        typeof(Duel),
        typeof(EffectType),
        typeof(Location),
        typeof(EventCode),
        typeof(EffectCode),
        typeof(EffectFlag),
        typeof(Reason),
        typeof(Phase),
        typeof(Position),
        typeof(Player),
        typeof(CardTargetType),
    };
    
    // 使用反射模式的类型（当没有生成代码时）
    [ReflectionUse]
    public static List<Type> ReflectionUse = new List<Type>()
    {
        typeof(Effect.ConditionDelegate),
        typeof(Effect.CostDelegate),
        typeof(Effect.TargetDelegate),
        typeof(Effect.OperationDelegate),
    };
}
