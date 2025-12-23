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
            case CardEffectType.SearchEquipmentOnDeath: return new SearchCardEffect();
            case CardEffectType.SearchFamilyOnEquip: return new SearchCardEffect();
            case CardEffectType.GrantOverload:
            case CardEffectType.DoubleOverload:
            case CardEffectType.LimitOperationEvolve:
                return new RobotEffect();
            default: return null;
        }
    }
}
