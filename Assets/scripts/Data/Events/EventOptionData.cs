using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件选项数据 - 定义事件中的一个选择项
/// </summary>
[Serializable]
public class EventOptionData
{
    [Header("选项显示")]
    public string OptionText = "选择此项";     // 按钮上显示的文本
    
    [Header("结果反馈")]
    public string ResultText = "结果描述";     // 选择后显示的结果文本
    
    [Header("旧版效果系统")]
    public EventEffectType Effect = EventEffectType.None;  // 效果类型
    public int Value = 0;                                   // 效果数值
    public CardData TargetCard;                             // 目标卡牌 (如果效果是获得卡牌)
    
    [Header("新版效果系统")]
    public List<EffectBase> Effects = new List<EffectBase>();  // 使用新的效果系统
}