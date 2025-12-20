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
    
    [Header("可选项")]
    public List<EventOptionData> Options = new List<EventOptionData>();  // 可选择的选项列表
}