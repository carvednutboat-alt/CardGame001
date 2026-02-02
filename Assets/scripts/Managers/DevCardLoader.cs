using UnityEngine;
using System.Collections.Generic;

public static class DevCardLoader
{
    private static bool _loaded = false;

    public enum DevDeckType
    {
        ThousandWeapons,
        Robot,
        LuaTest // New
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
            case DevDeckType.LuaTest:
                InjectLuaTestDeck();
                break;
        }
    }

    public static List<CardData> GetLuaTestCards()
    {
        List<CardData> list = new List<CardData>();
        
        // 1. Damage Spell - Fireball (1004)
        var fireAsset = Resources.Load<CardData>("Cards/火球");
        if (fireAsset != null) list.Add(fireAsset);

        // 2. Pot of Greed (1002) - Fix for user
        var greedAsset = Resources.Load<CardData>("Cards/强欲之壶");
        if (greedAsset != null) list.Add(greedAsset);

        // 3. SaintSword Caliburn (3101) - Request from user
        var caliburnAsset = Resources.Load<CardData>("Cards/圣剑卡利班");
        if (caliburnAsset != null) list.Add(caliburnAsset);
        
        // 4. Martial Artist (Red Unit for Caliburn)
        var unitAsset = Resources.Load<CardData>("Cards/千具武·宗师");
        if (unitAsset != null) list.Add(unitAsset);

        return list;
    }

    private static void InjectLuaTestDeck()
    {
        Debug.Log("=== Injecting Deck (Lua Test) ===");
        var cards = GetLuaTestCards();
        foreach(var c in cards)
        {
             // Add to GameManager MasterDeck (3 copies)
             for(int i=0; i<3; i++) GameManager.Instance.RegisterCardToDeck(c);
        }
    }
    
    private static CardData CreateEphemeralCard(int id, string name, string desc, CardKind kind, CardColor color, CardTargetType targetType, int atk=0, int hp=0)
    {
        CardData card = ScriptableObject.CreateInstance<CardData>();
        card.name = name; // Asset name
        card.id = id;
        card.cardName = name;
        card.description = desc;
        card.kind = kind;
        card.color = color;
        card.targetType = targetType;
        
        if (kind == CardKind.Unit)
        {
            card.unitAttack = atk;
            card.unitHealth = hp;
        }
        return card;
        
        // Removed GameManager dependency here to make it pure
    }

    private static void InjectThousandWeaponsDeck()
    {
        Debug.Log("=== Injecting Deck (千具武 Series) ===");

        // 1. Create Dummy Equipment (to ensure search works)
        AddCard("试炼之剑", 2, 3002);

        // 2. Red Unit 4-4 (Commander)
        AddCard("千具武·宗师", 1, 3001);

        // 3. Red Unit 2-3 (Soldier)
        AddCard("千具武·侍卫", 2, 3003);

        // 4. Red Unit 0-1 (Soldier)
        AddCard("千具武·新兵", 2, 3004);

        // 5. Red Unit 2-1 (Soldier)
        AddCard("千具武·突击者", 2, 3005);
        
        // 6. Field Magic: Prismatic Battleground (Colorless, ID 7001)
        CreateEphemeralCard(7001, "Prismatic Battleground", CardKind.Field, CardColor.Colorless, 3, 0, 0, null, 1);
    }

    private static void InjectRobotDeck()
    {
        Debug.Log("=== Injecting Deck (Robot Series) ===");
        
        // 1. Commander: Steam Robot (3/5)
        AddCard("蒸汽机器人·指挥官", 1, 4001);

        // 2. Unit 0/2 (Taunt, Aura) x3
        AddCard("蒸汽哨兵", 3, 4002);

        // 3. Unit 2/1 (On Kill -> Overload 1) x3
        AddCard("蒸汽收割者", 3, 4003);

        // 4. Unit 1/1 (Aura: Overload Gain +1) x3
        AddCard("过载增幅器", 3, 4004);

        // 5. Spells
        // Overload Mode (Overload 2)
        AddCard("过载模式", 3, 4005);

        // Double Overload
        AddCard("二重过载", 3, 4006);

        // Limit Operation
        AddCard("极限运转", 3, 4007);

        // New: Overload Release
        AddCard("过载释放", 3, 4008);
    }

    private static void AddCard(string assetName, int count, int luaId = 0)
    {
        CardData originalData = Resources.Load<CardData>("Cards/" + assetName);
        if (originalData == null)
        {
            Debug.LogError($"[DevCardLoader] Could not load card asset: Cards/{assetName}");
            return;
        }

        // Create a runtime clone to avoid modifying the Asset permanently in Editor,
        // but ensure we modify it for this session.
        CardData data = Object.Instantiate(originalData);
        data.name = originalData.name; // Keep name for lookups if needed
        
        // Patch ID for Lua
        if (luaId > 0)
        {
            data.id = luaId;
            // Clear legacy effect type to avoid double execution or conflicts
            // data.effectType = CardEffectType.None; 
            // Note: Some legacy logic (like damage value) might still use CardData fields.
            // But main logic is moved to Lua.
            // Safe to clear effectType if Lua script handles EVENT triggers.
            // But if EffectLogic.cs is triggered by specific Enums, clearing them stops C# logic.
            // Based on my Lua implementation, I WANT C# logic stopped for these IDs.
            data.effectType = CardEffectType.None;
            data.deathEffect = CardEffectType.None;
            data.onReceiveEquipEffect = CardEffectType.None;
            
            // Special case for Field: Ensure description is set if missing
            if (data.kind == CardKind.Field)
            {
                data.effectType = CardEffectType.None; 
            }
        }

        for(int i=0; i<count; i++) 
        {
            GameManager.Instance.RegisterCardToDeck(data);
        }
    }

    public static void InjectFieldDeck()
    {
        // 7001: Prismatic Battleground (Field magic, Colorless)
        CreateEphemeralCard(7001, "Prismatic Battleground", CardKind.Field, CardColor.Colorless, 3, 0, 0, null, 1);
    }

    private static void CreateEphemeralCard(int luaId, string name, CardKind kind, CardColor color, int cost, int atk, int hp, List<CardTag> tags, int count = 1)
    {
        // Create a fake asset structure or clone a base template
        // For simplicity, we clone an existing card and overwrite data
        // Assuming "Strike" exists as a template
        CardData template = Resources.Load<CardData>("Cards/Strike"); // Fallback
        if (template == null)
        {
             // Try load any card
             var all = Resources.LoadAll<CardData>("Cards");
             if (all.Length > 0) template = all[0];
        }

        if (template == null)
        {
            Debug.LogError("No template found!");
             return;
        }

        CardData data = Object.Instantiate(template);
        data.name = name; // Asset name
        data.cardName = name; // Display name
        data.id = luaId;
        data.kind = kind;
        data.color = color;
        data.cost = cost;
        data.unitAttack = atk;
        data.unitHealth = hp;
        data.description = "Field Magic: See Lua specific description.";
        if (tags != null && tags.Count > 0) data.cardTag = tags[0];
        else data.cardTag = CardTag.None;

        // Disable C# effects
        data.effectType = CardEffectType.None;
        data.deathEffect = CardEffectType.None;
        data.onReceiveEquipEffect = CardEffectType.None;

        for(int i=0; i<count; i++) 
        {
            GameManager.Instance.RegisterCardToDeck(data);
        }
        CacheCard(data);
    }

    // === Boss Data Injection ===
    public static void InjectBossData()
    {
        // 6000: Rainbow Boss (0/5/80)
        CreateEphemeralCard(6000, "Rainbow Boss", CardKind.Unit, CardColor.Colorless, 0, 5, 80, null, 0); 

        // === Boss Units (Minions) ===
        // 6001: Red Commander (Red, 5/5)
        CreateEphemeralCard(6001, "Red Commander", CardKind.Unit, CardColor.Red, 0, 5, 5, new List<CardTag>{CardTag.MartialArtist}, 0);
        
        // 6002: Green Commander (Green, 3/6)
        CreateEphemeralCard(6002, "Green Commander", CardKind.Unit, CardColor.Green, 0, 3, 6, null, 0);

        // 6003: Blue Commander (Blue, 4/4)
        CreateEphemeralCard(6003, "Blue Commander", CardKind.Unit, CardColor.Blue, 0, 4, 4, new List<CardTag>{CardTag.LinearAlgebra}, 0);

        // === Boss Action Cards ===
        // Red 1 (6011): +2 ATK (UnitBuff)
        CreateEphemeralCard(6011, "Red Strength", CardKind.Spell, CardColor.Red, 0, 0, 0, null, 0);
        var red1 = GetCardDataShim(6011);
        if(red1!=null) { red1.effectType = CardEffectType.UnitBuff; red1.value = 2; }

        // Red 2 (6012): Assault (Lua)
        CreateEphemeralCard(6012, "Assault", CardKind.Spell, CardColor.Red, 0, 0, 0, null, 0);

        // Green 1 (6021): Summon Wolves (Lua)
        CreateEphemeralCard(6021, "Summon Pack", CardKind.Spell, CardColor.Green, 0, 0, 0, null, 0);

        // Green 2 (6022): Pack Tactics (All +1/+1) (Lua)
        CreateEphemeralCard(6022, "Pack Tactics", CardKind.Spell, CardColor.Green, 0, 0, 0, null, 0);

        // Blue 1 (6031): Arcane Nova (Countdown 2, AOE 12) (Lua)
        CreateEphemeralCard(6031, "Arcane Nova", CardKind.Spell, CardColor.Blue, 0, 0, 0, null, 0);

        // Blue 2 (6032): Ice Shard (Countdown 1, Dmg 8) (Lua)
        CreateEphemeralCard(6032, "Ice Shard", CardKind.Spell, CardColor.Blue, 0, 0, 0, null, 0);

        // Tokens
        // 6050: Wolf (3/3)
        CreateEphemeralCard(6050, "Wolf", CardKind.Unit, CardColor.Green, 0, 3, 3, null, 0);
    }


    // Simplified Database Access for Prototype
    private static Dictionary<int, CardData> _cache = new Dictionary<int, CardData>();
    public static CardData GetCardDataShim(int id)
    {
        if (_cache.ContainsKey(id)) return _cache[id];
        return null;
    }

    private static void CacheCard(CardData data)
    {
        if (!_cache.ContainsKey(data.id)) _cache.Add(data.id, data);
    }
}

