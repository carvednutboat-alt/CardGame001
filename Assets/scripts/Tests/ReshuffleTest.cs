using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReshuffleTest : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Debug/Test Reshuffle Logic")]
    public static void RunTest()
    {
        var go = new GameObject("ReshuffleTest_Runner");
        go.AddComponent<ReshuffleTest>();
    }
#endif

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        
        var bm = BattleManager.Instance;
        if (bm == null) { Debug.LogError("BattleManager not found"); yield break; }
        var dm = bm.DeckManager;

        Debug.Log("=== Starting Reshuffle Logic Test ===");

        // 1. Setup: Clear everything
        dm.DrawPile.Clear();
        dm.DiscardPile.Clear();
        dm.Hand.Clear(); // Data only cleanup for test

        Debug.Log("Step 1: Cleared all piles.");

        // 2. Add Dummy Cards to Discard
        for(int i=0; i<5; i++)
        {
            var cardData = ScriptableObject.CreateInstance<CardData>();
            cardData.cardName = "TestCard_" + i;
            cardData.id = 9991 + i; 
            cardData.kind = CardKind.Spell;
            var rc = new RuntimeCard(cardData);
            // Simulate discard state
            rc.UpdateLocation(0x10, i); // Location.GRAVE = 0x10 usually, checking assumption
            dm.DiscardPile.Add(rc);
        }
        Debug.Log($"Step 2: Added 5 cards to Discard Pile. DrawPile: {dm.DrawPile.Count}, Discard: {dm.DiscardPile.Count}");

        // 3. Trigger Draw (Should trigger reshuffle)
        Debug.Log("Step 3: Attempting to Draw 1 card...");
        
        dm.DrawCards(1);

        yield return null;

        // 4. Verification
        Debug.Log($"=== Results ===");
        Debug.Log($"DrawPile Count: {dm.DrawPile.Count}");
        Debug.Log($"Hand Count: {dm.Hand.Count}");
        Debug.Log($"DiscardPile Count: {dm.DiscardPile.Count}");

        if (dm.DrawPile.Count == 4 && dm.Hand.Count == 1)
        {
            Debug.Log("<color=green>TEST PASSED: Reshuffle worked correctly.</color>");
            if (dm.Hand.Count > 0) Debug.Log($"Drawn Card: {dm.Hand[0].Name} Loc={dm.Hand[0].CurrentLocation}");
        }
        else
        {
            Debug.LogError("<color=red>TEST FAILED: State mismatch.</color>");
        }

        Destroy(this.gameObject);
    }
}
