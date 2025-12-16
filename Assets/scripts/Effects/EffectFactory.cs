public static class EffectFactory
{
    public static EffectBase GetEffect(CardEffectType type)
    {
        switch (type)
        {
            case CardEffectType.DamageEnemy: return new DamageEnemyEffect();
            case CardEffectType.HealUnit: return new HealUnitEffect();
            case CardEffectType.DrawCards: return new DrawCardsEffect();
            case CardEffectType.UnitBuff: return new UnitBuffEffect();
            case CardEffectType.FieldEvolve: return new FieldEvolveEffect();
            case CardEffectType.DamageAllEnemyUnits: return new DamageAllEnemiesEffect();
            case CardEffectType.ReviveUnit: return new ReviveUnitEffect();
            // ... 其他类型请在这里补充
            default: return null;
        }
    }
}