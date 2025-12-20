using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责在战斗中维护敌人的状态
/// </summary>
public class EnemyState : MonoBehaviour
{
    public int MaxHP = 50;
    public int CurrentHP = 50;

    [Header("战斗中的单位")]
    public List<Unit> ActiveUnits = new List<Unit>();

    // 可以在这里扩展敌人特有的逻辑，比如意图系统
}
