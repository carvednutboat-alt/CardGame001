using UnityEngine;
using System.Collections.Generic;

public static class DevCardLoader
{
    private static bool _loaded = false;

    public enum DevDeckType
    {
        ThousandWeapons,
        Robot
    }

    public static void InjectDeck(DevDeckType type)
    {
        if (GameManager.Instance == null) return;
        
        // Clear existing just in case (though GameManager.Instance.MasterDeck should be cleared outside)
        // GameManager.Instance.MasterDeck.Clear(); // Assuming New Game clears it.

        switch (type)
        {
            case DevDeckType.ThousandWeapons:
                InjectThousandWeaponsDeck();
                break;
            case DevDeckType.Robot:
                InjectRobotDeck();
                break;
        }
    }

    private static void InjectThousandWeaponsDeck()
    {
        Debug.Log("=== Injecting Deck (千具武 Series) ===");

        // 1. Create Dummy Equipment (to ensure search works)
        CardData sword = CreateEquipment("试炼之剑", 1, 1);
        GameManager.Instance.AddCardToDeck(sword);
        GameManager.Instance.AddCardToDeck(sword); 

        // 2. Red Unit 4-4 (Commander)
        CardData unit44 = CreateUnit("千具武·宗师", 4, 4, 4);
        unit44.cardTag = CardTag.MartialArtist;
        unit44.isCommander = true; 
        GameManager.Instance.AddCardToDeck(unit44);

        // 3. Red Unit 2-3 (Soldier)
        CardData unit23 = CreateUnit("千具武·侍卫", 2, 2, 3);
        unit23.cardTag = CardTag.MartialArtist;
        unit23.deathEffect = CardEffectType.SearchEquipmentOnDeath;
        unit23.startsInDeck = true;       
        unit23.dieWithoutCommander = true;
        unit23.description = "亡语：检索装备。\n<color=red>若无指挥官则自毁。</color>";
        GameManager.Instance.AddCardToDeck(unit23);
        GameManager.Instance.AddCardToDeck(unit23);

        // 4. Red Unit 0-1 (Soldier)
        CardData unit01 = CreateUnit("千具武·新兵", 0, 0, 1);
        unit01.cardTag = CardTag.MartialArtist;
        unit01.deathEffect = CardEffectType.SearchEquipmentOnDeath;
        unit01.startsInDeck = true;
        unit01.dieWithoutCommander = true;
        unit01.description = "亡语：检索装备。\n<color=red>若无指挥官则自毁。</color>";
        GameManager.Instance.AddCardToDeck(unit01);
        GameManager.Instance.AddCardToDeck(unit01);

        // 5. Red Unit 2-1 (Soldier)
        CardData unit21 = CreateUnit("千具武·突击者", 2, 2, 1);
        unit21.cardTag = CardTag.MartialArtist;
        unit21.onReceiveEquipEffect = CardEffectType.SearchFamilyOnEquip;
        unit21.startsInDeck = true;
        unit21.dieWithoutCommander = true;
        unit21.description = "被装备：检索本家。\n<color=red>若无指挥官则自毁。</color>";
        GameManager.Instance.AddCardToDeck(unit21);
        GameManager.Instance.AddCardToDeck(unit21);
    }

    private static void InjectRobotDeck()
    {
        Debug.Log("=== Injecting Deck (Robot Series) ===");
        
        // 1. Commander: Steam Robot (3/5)
        CardData cmd = CreateUnit("蒸汽机器人·指挥官", 3, 3, 5);
        cmd.cardTag = CardTag.Robot;
        cmd.isCommander = true;
        cmd.description = "过载模式：获得 +2/+0。若被极限运转，则攻击力 = 过载 x 2。";
        GameManager.Instance.AddCardToDeck(cmd);

        // 2. Unit 0/2 (Taunt, Aura) x3
        CardData u02 = CreateUnit("蒸汽哨兵", 0, 0, 2);
        u02.cardTag = CardTag.Robot;
        u02.startsInDeck = true;
        u02.dieWithoutCommander = true;
        u02.unitHasTaunt = true; // Need to ensure Helper sets this
        u02.description = "嘲讽。光环：相邻友方单位 +1 生命。\n<color=red>若无指挥官则自毁。</color>";
        AddCopies(u02, 3);

        // 3. Unit 2/1 (On Kill -> Overload 1) x3
        CardData u21 = CreateUnit("蒸汽收割者", 2, 2, 1);
        u21.cardTag = CardTag.Robot;
        u21.startsInDeck = true;
        u21.dieWithoutCommander = true;
        u21.description = "当此单位破坏敌人时，获得过载 1。\n<color=red>若无指挥官则自毁。</color>";
        AddCopies(u21, 3);

        // 4. Unit 1/1 (Aura: Overload Gain +1) x3
        CardData u11 = CreateUnit("过载增幅器", 1, 1, 1);
        u11.cardTag = CardTag.Robot;
        u11.startsInDeck = true;
        u11.dieWithoutCommander = true;
        u11.description = "光环：当友方获得过载时，使其数值+1。\n<color=red>若无指挥官则自毁。</color>";
        AddCopies(u11, 3);

        // 5. Spells
        // Overload Mode (Overload 2)
        CardData sOverload = CreateSpell("过载模式", CardEffectType.GrantOverload, CardTargetType.Ally, 2);
        sOverload.cardTag = CardTag.Robot;
        sOverload.description = "使一个单位获得 过载 2。";
        AddCopies(sOverload, 3);

        // Double Overload
        CardData sDouble = CreateSpell("二重过载", CardEffectType.DoubleOverload, CardTargetType.Ally, 0);
        sDouble.cardTag = CardTag.Robot;
        sDouble.description = "使一个单位过载翻倍，但回合结束时受到等量的伤害。";
        AddCopies(sDouble, 3);

        // Limit Operation
        CardData sLimit = CreateSpell("极限运转", CardEffectType.LimitOperationEvolve, CardTargetType.Ally, 0);
        sLimit.cardTag = CardTag.Robot;
        sLimit.description = "进化指挥官：攻击加成改为 (过载 x 2)。";
        AddCopies(sLimit, 3);
    }

    private static void AddCopies(CardData data, int count)
    {
        for(int i=0; i<count; i++) GameManager.Instance.AddCardToDeck(data);
    }

    private static CardData CreateSpell(string name, CardEffectType effect, CardTargetType target, int value)
    {
        CardData c = ScriptableObject.CreateInstance<CardData>();
        c.name = name;
        c.cardName = name;
        c.cost = 1;
        c.color = CardColor.Red; // Robot is Red? Assuming Red.
        c.kind = CardKind.Spell;
        c.effectType = effect;
        c.targetType = target;
        c.value = value;
        return c;
    }

    private static CardData CreateUnit(string name, int cost, int atk, int hp)
    {
        CardData c = ScriptableObject.CreateInstance<CardData>();
        c.name = name; // Asset name
        c.cardName = name;
        c.cost = cost; // Kept for compatibility, though we use Color
        c.color = CardColor.Red;
        c.kind = CardKind.Unit;
        c.unitAttack = atk;
        c.unitHealth = hp;
        
        // Default target type for units is usually None (place in slot), 
        // but if they have a targeted battlecry it might differ.
        c.targetType = CardTargetType.None; 
        
        return c;
    }

    private static CardData CreateEquipment(string name, int atkBonus, int hpBonus)
    {
        CardData c = ScriptableObject.CreateInstance<CardData>();
        c.name = name;
        c.cardName = name;
        c.cost = 1;
        c.color = CardColor.Colorless; // General equipment
        c.kind = CardKind.Spell; // Equipment is technically a Spell kind with isEquipment flag
        c.isEquipment = true;
        c.equipAttackBonus = atkBonus;
        c.equipHealthBonus = hpBonus;
        c.targetType = CardTargetType.Ally; // Must target ally
        c.description = $"装备：+{atkBonus}/+{hpBonus}";
        return c;
    }
}
