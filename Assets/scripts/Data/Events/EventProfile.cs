using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件配置文件 - 定义一个完整的随机事件
/// </summary>
[CreateAssetMenu(fileName = "NewEvent", menuName = "Game/Event Profile")]
public class EventProfile : ScriptableObject
{
    [Header("事件信息")]
    public string Title = "事件标题";                 // 事件标题
    
    [TextArea(3, 10)]
    public string Description = "事件描述...";        // 事件描述文本
    
    public Sprite EventImage;                          // 事件配图
    
    [Header("解锁条件")]
    [Tooltip("需要拥有的遗物才能触发此事件 (留空表示无条件)")]
    public List<RelicData> RequiredRelics = new List<RelicData>();  // [NEW] 需要的遗物
    
    [Tooltip("需要拥有的遗物ID才能触发此事件 (留空表示无条件)")]
    public List<string> RequiredRelicIds = new List<string>();      // [NEW] 需要的遗物ID (备用方案)
    
    [Header("可选项")]
    public List<EventOptionData> Options = new List<EventOptionData>();  // 可选择的选项列表
}