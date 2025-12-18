using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Config/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    [Header("普通战斗池 (Minor Enemy)")]
    public List<EnemyEncounterProfile> MinorEncounters;

    [Header("精英战斗池 (Elite Enemy)")]
    public List<EnemyEncounterProfile> EliteEncounters;

    [Header("Boss 战斗池")]
    public List<EnemyEncounterProfile> BossEncounters;

    // 根据节点类型随机获取一场战斗
    public EnemyEncounterProfile GetRandomEncounter(NodeType type)
    {
        switch (type)
        {
            case NodeType.MinorEnemy:
                return GetRandom(MinorEncounters);
            case NodeType.EliteEnemy:
                return GetRandom(EliteEncounters);
            case NodeType.Boss:
                return GetRandom(BossEncounters);
            default:
                return null;
        }
    }

    private EnemyEncounterProfile GetRandom(List<EnemyEncounterProfile> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}