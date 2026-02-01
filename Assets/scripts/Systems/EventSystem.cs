using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// 事件系统 - 参考YGOPro的事件驱动机制
/// 负责管理游戏事件的触发和效果的响应
/// </summary>
[LuaCallCSharp]
public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance;
    
    private BattleManager _battleManager;
    
    // 事件队列
    private Queue<GameEvent> _eventQueue = new Queue<GameEvent>();
    
    // 待触发的效果列表
    private List<Effect> _triggerEffects = new List<Effect>();
    
    // 当前正在处理的事件
    private GameEvent _currentEvent;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void Init(BattleManager bm)
    {
        _battleManager = bm;
    }
    
    /// <summary>
    /// 触发事件
    /// </summary>
    public void RaiseEvent(int eventCode, RuntimeCard card = null, int value = 0, int reason = 0)
    {
        var gameEvent = new GameEvent
        {
            Code = eventCode,
            Card = card,
            Value = value,
            Reason = reason,
            Player = card?.Owner ?? 0
        };
        
        _eventQueue.Enqueue(gameEvent);
        
        Debug.Log($"[EventSystem] Event Raised: {eventCode}, Card: {card?.Data?.cardName}");
    }
    
    /// <summary>
    /// 触发单位相关事件
    /// </summary>
    public void RaiseUnitEvent(int eventCode, RuntimeUnit unit, int value = 0, int reason = 0)
    {
        var gameEvent = new GameEvent
        {
            Code = eventCode,
            Card = unit?.SourceCard,
            Unit = unit,
            Value = value,
            Reason = reason,
            Player = 0
        };
        
        _eventQueue.Enqueue(gameEvent);
        
        Debug.Log($"[EventSystem] Unit Event Raised: {eventCode}, Unit: {unit?.Name}");
    }
    
    /// <summary>
    /// 触发装备事件 (Special case: Unit is target, Card is equipment)
    /// </summary>
    public void RaiseEquipEvent(RuntimeUnit target, RuntimeCard equipment)
    {
        var gameEvent = new GameEvent
        {
            Code = EventCode.EQUIP,
            Card = equipment,
            Unit = target,
            Value = 0,
            Reason = 0,
            Player = equipment?.Owner ?? 0
        };
        
        _eventQueue.Enqueue(gameEvent);
        
        Debug.Log($"[EventSystem] Equip Event Raised: {target?.Name} equipped with {equipment?.Data?.cardName}");
    }

    /// <summary>
    /// 处理事件队列
    /// </summary>
    public void ProcessEvents()
    {
        while (_eventQueue.Count > 0)
        {
            var evt = _eventQueue.Dequeue();
            ProcessSingleEvent(evt);
        }
    }
    
    /// <summary>
    /// 处理单个事件
    /// </summary>
    private void ProcessSingleEvent(GameEvent evt)
    {
        _currentEvent = evt;
        
        // 收集满足条件的触发效果
        CollectTriggerEffects(evt);
        
        // 执行触发效果
        ExecuteTriggerEffects(evt);
        
        _currentEvent = null;
    }
    
    /// <summary>
    /// 收集触发效果
    /// </summary>
    private void CollectTriggerEffects(GameEvent evt)
    {
        _triggerEffects.Clear();
        
        // Debug.Log($"[EventSystem] Collecting effects for event {evt.Code}");
        
        // 遍历所有玩家单位的效果
        if (_battleManager?.UnitManager != null)
        {
            foreach (var unit in _battleManager.UnitManager.PlayerUnits)
            {
                if (unit?.SourceCard == null) continue;
                
                // Debug.Log($"[EventSystem] Unit {unit.Name} has {unit.SourceCard.Effects.Count} effects");
                foreach (var effect in unit.SourceCard.Effects)
                {
                    if (CheckEffectTrigger(effect, evt))
                    {
                        _triggerEffects.Add(effect);
                        // Debug.Log($"[EventSystem] ✓ Added effect Code={effect.EffectCode} from {unit.Name}");
                    }
                }
            }
        }
        
        // 遍历手牌效果
        if (_battleManager?.DeckManager != null)
        {
            foreach (var card in _battleManager.DeckManager.Hand)
            {
                if (card == null) continue;
                
                foreach (var effect in card.Effects)
                {
                    if (CheckEffectTrigger(effect, evt))
                    {
                        _triggerEffects.Add(effect);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 检查效果是否满足触发条件
    /// </summary>
    private bool CheckEffectTrigger(Effect effect, GameEvent evt)
    {
        // 检查效果类型是否为触发效果
        if (!effect.IsHasType(EffectType.TRIGGER)) return false;
        
        // 检查效果代码是否匹配事件
        if (effect.EffectCode != evt.Code && effect.EffectCode != EventCode.FREE_CHAIN) return false;
        
        // 检查效果是否可用
        if (!effect.IsAvailable()) return false;
        
        // 检查条件
        // KEY CHANGE: Passing evt.Card as 'eg' (Event Group / Event Graph)
        // If it's an EQUIP event, evt.Card is the Equipment.
        // If it's a SUMMON event, evt.Card is the Summoned Unit's Card.
        return effect.CheckCondition(evt.Player, evt.Card, evt.Player, evt.Value, null, evt.Reason, evt.Player);
    }
    
    /// <summary>
    /// 执行触发效果
    /// </summary>
    private void ExecuteTriggerEffects(GameEvent evt)
    {
        foreach (var effect in _triggerEffects)
        {
            // 检查代价
            if (!effect.CheckCost(evt.Player, evt.Card, evt.Player, evt.Value, null, evt.Reason, evt.Player))
            {
                continue;
            }
            
            // 检查目标
            if (!effect.CheckTarget(evt.Player, evt.Card, evt.Player, evt.Value, null, evt.Reason, evt.Player))
            {
                continue;
            }
            
            // 执行操作
            effect.ExecuteOperation(evt.Player, evt.Card, evt.Player, evt.Value, null, evt.Reason, evt.Player);
            
            Debug.Log($"[EventSystem] Effect Executed: {effect.Description ?? effect.EffectCode.ToString()}");
        }
    }
    
    /// <summary>
    /// 获取当前事件
    /// </summary>
    public GameEvent GetCurrentEvent()
    {
        return _currentEvent;
    }
}

/// <summary>
/// 游戏事件
/// </summary>
[LuaCallCSharp]
public class GameEvent
{
    public int Code;            // 事件代码
    public RuntimeCard Card;    // 相关卡牌
    public RuntimeUnit Unit;    // 相关单位
    public int Value;           // 事件值（如伤害量）
    public int Reason;          // 原因
    public int Player;          // 玩家
}