using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEncounter", menuName = "Data/Enemy Encounter")]
public class EnemyEncounterProfile : ScriptableObject
{
    [Header("遭遇战 ID")]
    public string EncounterID; // 例如 "Goblin_Trio"

    [Header("包含的敌人列表")]
    // 这里引用你的 CardData (假设你的敌人数据也是用 CardData 存的，如果是 UnitData 请自行替换)
    public List<CardData> Enemies;

    [Header("敌人站位偏移 (可选)")]
    // 如果你想微调每个怪的位置，可以加这个，否则默认按生成的槽位排
    public List<Vector3> SpawnOffsets;
}