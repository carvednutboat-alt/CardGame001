public static class EffectFactory
{
    public static EffectBase GetEffect(CardEffectType type)
    {
        switch (type)
        {
            case CardEffectType.DamageEnemy: return new DamageEnemyEffect();
            case CardEffectType.HealUnit: return new HealUnitEffect();
            case CardEffectType.DrawCards: return new DrawCardsEffect();
            case CardEffectType.Fly: return new FlyEffect();
            case CardEffectType.FieldEvolve: return new FieldEvolveEffect();
            case CardEffectType.DamageAllEnemyUnits: return new DamageAllEnemiesEffect();
            case CardEffectType.ReviveUnit: return new ReviveUnitEffect();
            case CardEffectType.UnitBuff: return new UnitBuffEffect();
            // === NEW: Linear Algebra ===
            // case CardEffectType.LinearAlgebra_SwapColumns: return new SwapColumnsEffect();
            default: return null;
        }
    }
}
