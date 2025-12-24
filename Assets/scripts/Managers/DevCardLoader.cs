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
        AddCard("试炼之剑", 2);

        // 2. Red Unit 4-4 (Commander)
        AddCard("千具武·宗师", 1);

        // 3. Red Unit 2-3 (Soldier)
        AddCard("千具武·侍卫", 2);

        // 4. Red Unit 0-1 (Soldier)
        AddCard("千具武·新兵", 2);

        // 5. Red Unit 2-1 (Soldier)
        AddCard("千具武·突击者", 2);
    }

    private static void InjectRobotDeck()
    {
        Debug.Log("=== Injecting Deck (Robot Series) ===");
        
        // 1. Commander: Steam Robot (3/5)
        AddCard("蒸汽机器人·指挥官", 1);

        // 2. Unit 0/2 (Taunt, Aura) x3
        AddCard("蒸汽哨兵", 3);

        // 3. Unit 2/1 (On Kill -> Overload 1) x3
        AddCard("蒸汽收割者", 3);

        // 4. Unit 1/1 (Aura: Overload Gain +1) x3
        AddCard("过载增幅器", 3);

        // 5. Spells
        // Overload Mode (Overload 2)
        AddCard("过载模式", 3);

        // Double Overload
        AddCard("二重过载", 3);

        // Limit Operation
        AddCard("极限运转", 3);
    }

    private static void AddCard(string assetName, int count)
    {
        CardData data = Resources.Load<CardData>("Cards/" + assetName);
        if (data == null)
        {
            Debug.LogError($"[DevCardLoader] Could not load card asset: Cards/{assetName}");
            return;
        }

        for(int i=0; i<count; i++) 
        {
            GameManager.Instance.AddCardToDeck(data);
        }
    }
}
