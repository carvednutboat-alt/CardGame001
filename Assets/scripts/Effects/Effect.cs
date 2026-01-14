using System;
using System.Collections.Generic;
using XLua;
using UnityEngine;

/// <summary>
/// 卡牌效果类 - 参考YGOPro-core结构
/// 完整的C-C-T-O模式（Condition, Cost, Target, Operation）
/// </summary>
[LuaCallCSharp]
public class Effect
{
    // ============================================
    // 核心标识
    // ============================================
    public int EffectCode;        // 效果代码 (EFFECT_UPDATE_ATTACK等)
    public int EffectType;        // 效果类型 (IGNITION, TRIGGER等)
    public int EffectFlag;        // 效果标志 (SINGLE_RANGE, BOTH_SIDE等)
    public int Range;             // 生效范围 (LOCATION_MZONE等)
    public int TargetRange;       // 目标范围 (我方场/对方场)
    public int TargetRangePlayer;  // 目标范围玩家 (0=自己, 1=对手)
    
    // ============================================
    // 持有者和关联
    // ============================================
    public RuntimeCard OwnerCard;
    public RuntimeUnit OwnerUnit; // 如果效果来自场上单位
    
    // ============================================
    // 效果值和标签
    // ============================================
    public int Value;             // 数值效果（如ATK+500）
    public string Label;          // 效果标签
    public string Description;    // 效果描述
    
    // ============================================
    // C-C-T-O 委托
    // ============================================
    [CSharpCallLua]
    public delegate bool ConditionDelegate(Effect e, int tp, object eg, int ep, int ev, Effect re, int r, int rp, int chk);
    
    [CSharpCallLua]
    public delegate bool CostDelegate(Effect e, int tp, object eg, int ep, int ev, Effect re, int r, int rp, int chk);
    
    [CSharpCallLua]
    public delegate bool TargetDelegate(Effect e, int tp, object eg, int ep, int ev, Effect re, int r, int rp, int chk);
    
    [CSharpCallLua]
    public delegate void OperationDelegate(Effect e, int tp, object eg, int ep, int ev, Effect re, int r, int rp);
    
    public ConditionDelegate Condition;
    public CostDelegate Cost;
    public TargetDelegate Target;
    public OperationDelegate Operation;
    
    // ============================================
    // 效果状态
    // ============================================
    public bool IsActivated;      // 是否已激活
    public bool IsDisabled;       // 是否被无效化
    public int ResetCount;        // 重置计数
    public int ResetFlag;         // 重置标志
    
    // ============================================
    // 目标追踪
    // ============================================
    private List<RuntimeCard> _targets = new List<RuntimeCard>();
    private List<RuntimeUnit> _unitTargets = new List<RuntimeUnit>();
    
    // ============================================
    // Setter方法（供Lua调用）
    // ============================================
    public void SetCode(int code)
    {
        EffectCode = code;
    }
    
    public void SetType(int type)
    {
        EffectType = type;
    }
    
    public void SetRange(int range)
    {
        Range = range;
    }
    
    public void SetTargetRange(int s, int o)
    {
        TargetRange = s;
        TargetRangePlayer = o;
    }
    
    public void SetValue(int value)
    {
        Value = value;
    }
    
    public void SetLabel(string label)
    {
        Label = label;
    }
    
    public void SetDescription(string desc)
    {
        Description = desc;
    }
    
    public void SetCondition(ConditionDelegate func)
    {
        Condition = func;
    }
    
    public void SetCost(CostDelegate func)
    {
        Cost = func;
    }
    
    public void SetTarget(TargetDelegate func)
    {
        Target = func;
    }
    
    public void SetOperation(OperationDelegate func)
    {
        Operation = func;
    }
    
    // ============================================
    // 效果可用性检查
    // ============================================
    public bool IsAvailable()
    {
        if (IsDisabled) return false;
        if (OwnerCard == null) return false;
        
        // 检查位置范围
        if (!CheckRange())
        {
            return false;
        }
        
        return true;
    }
    
    private bool CheckRange()
    {
        if (OwnerCard == null) return false;
        
        // 检查卡牌当前位置是否在效果生效范围内
        int currentLoc = OwnerCard.CurrentLocation;
        return (currentLoc & Range) != 0;
    }
    
    // ============================================
    // 效果执行
    // ============================================
    public bool CheckCondition(int tp, object eg, int ep, int ev, Effect re, int r, int rp, int chk = 0)
    {
        if (Condition == null) return true;
        
        try
        {
            return Condition(this, tp, eg, ep, ev, re, r, rp, chk);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Effect] Condition Error: {ex.Message}");
            return false;
        }
    }
    
    public bool CheckCost(int tp, object eg, int ep, int ev, Effect re, int r, int rp, int chk = 0)
    {
        if (Cost == null) return true;
        
        try
        {
            return Cost(this, tp, eg, ep, ev, re, r, rp, chk);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Effect] Cost Error: {ex.Message}");
            return false;
        }
    }
    
    public bool CheckTarget(int tp, object eg, int ep, int ev, Effect re, int r, int rp, int chk = 0)
    {
        if (Target == null) return true;
        
        try
        {
            return Target(this, tp, eg, ep, ev, re, r, rp, chk);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Effect] Target Error: {ex.Message}");
            return false;
        }
    }
    
    public void ExecuteOperation(int tp, object eg, int ep, int ev, Effect re, int r, int rp)
    {
        if (Operation == null) return;
        
        try
        {
            Operation(this, tp, eg, ep, ev, re, r, rp);
            IsActivated = true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Effect] Operation Error: {ex.Message}");
        }
    }
    
    // ============================================
    // 目标管理
    // ============================================
    public void AddTarget(RuntimeCard card)
    {
        if (!_targets.Contains(card))
        {
            _targets.Add(card);
        }
    }
    
    public void AddTarget(RuntimeUnit unit)
    {
        if (!_unitTargets.Contains(unit))
        {
            _unitTargets.Add(unit);
        }
    }
    
    public List<RuntimeCard> GetTargets()
    {
        return new List<RuntimeCard>(_targets);
    }
    
    public List<RuntimeUnit> GetUnitTargets()
    {
        return new List<RuntimeUnit>(_unitTargets);
    }
    
    public void ClearTargets()
    {
        _targets.Clear();
        _unitTargets.Clear();
    }
    
    // ============================================
    // Getter方法（供Lua调用）
    // ============================================
    public RuntimeCard GetHandler()
    {
        return OwnerCard;
    }
    
    public RuntimeUnit GetHandlerUnit()
    {
        return OwnerUnit;
    }
    
    public int GetCode()
    {
        return EffectCode;
    }
    
    public int GetType()
    {
        return EffectType;
    }
    
    public int GetValue()
    {
        return Value;
    }
    
    public bool IsHasType(int type)
    {
        return (EffectType & type) != 0;
    }
    
    // ============================================
    // 效果复制
    // ============================================
    public Effect Clone()
    {
        var copy = new Effect
        {
            EffectCode = this.EffectCode,
            EffectType = this.EffectType,
            EffectFlag = this.EffectFlag,
            Range = this.Range,
            TargetRange = this.TargetRange,
            TargetRangePlayer = this.TargetRangePlayer,
            Value = this.Value,
            Label = this.Label,
            Description = this.Description,
            Condition = this.Condition,
            Cost = this.Cost,
            Target = this.Target,
            Operation = this.Operation
        };
        return copy;
    }
    
    // ============================================
    // 静态工厂方法（供Lua调用）
    // ============================================
    public static Effect CreateEffect(RuntimeCard c)
    {
        var effect = new Effect
        {
            OwnerCard = c
        };
        return effect;
    }
}