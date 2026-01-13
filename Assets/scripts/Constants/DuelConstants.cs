using XLua;

/// <summary>
/// 统一的游戏常量定义系统
/// 参考YGOPro-core的常量结构
/// </summary>

// ============================================
// 效果类型 (EFFECT_TYPE)
// ============================================
[LuaCallCSharp]
public static class EffectType
{
    public const int SINGLE = 1;      // 单体效果（只影响自己）
    public const int FIELD = 2;       // 场地效果（影响全场）
    public const int IGNITION = 4;    // 起动效果（玩家主动激活）
    public const int TRIGGER = 8;     // 触发效果（满足条件自动触发）
    public const int QUICK = 16;      // 速攻效果（可在对方回合使用）
    public const int CONTINUOUS = 32; // 持续效果（一直生效）
    public const int EQUIP = 64;      // 装备效果
}

// ============================================
// 位置 (LOCATION)
// ============================================
[LuaCallCSharp]
public static class Location
{
    public const int HAND = 0x01;
    public const int MZONE = 0x02;    // Monster Zone (战场)
    public const int SZONE = 0x04;    // Spell/Trap Zone
    public const int GRAVE = 0x08;    // 墓地
    public const int REMOVED = 0x10;  // 除外区
    public const int DECK = 0x20;     // 牌库
    public const int EXTRA = 0x40;    // 额外卡组
    public const int OVERLAY = 0x80;  // 叠放（XYZ素材）
    
    public const int ONFIELD = MZONE | SZONE; // 场上
    public const int ALL = 0xFF;
}

// ============================================
// 效果标志 (EFFECT_FLAG)
// ============================================
[LuaCallCSharp]
public static class EffectFlag
{
    public const int SINGLE_RANGE = 0x01;          // 效果只影响自己
    public const int BOTH_SIDE = 0x02;             // 影响双方
    public const int CANNOT_DISABLE = 0x04;        // 不能被无效
    public const int IMMEDIATELY_APPLY = 0x08;     // 立即生效
    public const int COPY_INHERIT = 0x10;          // 可被复制继承
    public const int OWNER_RELATE = 0x20;          // 与持有者关联
    public const int PLAYER_TARGET = 0x40;         // 以玩家为目标
    public const int CARD_TARGET = 0x80;           // 以卡牌为目标
    public const int DELAY = 0x100;                // 延迟生效
    public const int INITIAL = 0x200;              // 初始效果（卡牌自带）
}

// ============================================
// 事件代码 (EVENT)
// ============================================
[LuaCallCSharp]
public static class EventCode
{
    // 召唤相关
    public const int SUMMON = 100;
    public const int FLIP_SUMMON = 101;
    public const int SPECIAL_SUMMON = 102;
    public const int SUMMON_SUCCESS = 103;
    
    // 破坏相关
    public const int DESTROYED = 200;
    public const int BATTLE_DESTROYED = 201;
    public const int DESTROYED_BY_EFFECT = 202;
    
    // 战斗相关
    public const int BATTLE_START = 300;
    public const int BATTLE_END = 301;
    public const int ATTACK_ANNOUNCE = 302;
    public const int BE_BATTLE_TARGET = 303;
    public const int DAMAGE_STEP_START = 304;
    public const int DAMAGE_CALCULATING = 305;
    public const int DAMAGE_STEP_END = 306;
    
    // 卡牌移动
    public const int TO_HAND = 400;
    public const int TO_DECK = 401;
    public const int TO_GRAVE = 402;
    public const int REMOVE = 403;
    
    // 抽卡
    public const int DRAW = 500;
    
    // 伤害/回复
    public const int DAMAGE = 600;
    public const int RECOVER = 601;
    
    // 回合/阶段
    public const int PHASE_START = 700;
    public const int PHASE_END = 701;
    public const int TURN_END = 702;
    
    // 效果相关
    public const int CHAIN_SOLVING = 800;
    public const int CHAIN_SOLVED = 801;
    public const int CHAIN_END = 802;
    
    // 装备
    public const int EQUIP = 900;
    public const int EQUIPPED = 901;
    public const int LEAVE_FIELD = 902;
    
    // 自由时点（任意时机都能发动）
    public const int FREE_CHAIN = 1000;
}

// ============================================
// 理由 (REASON) - 卡牌移动/破坏的原因
// ============================================
[LuaCallCSharp]
public static class Reason
{
    public const int DESTROY = 0x01;
    public const int RELEASE = 0x02;     // 解放
    public const int TEMPORARY = 0x04;   // 临时
    public const int MATERIAL = 0x08;    // 作为素材
    public const int SUMMON = 0x10;      // 召唤
    public const int BATTLE = 0x20;      // 战斗破坏
    public const int EFFECT = 0x40;      // 效果破坏
    public const int COST = 0x80;        // 代价
    public const int ADJUST = 0x100;     // 规则调整
    public const int LOST_TARGET = 0x200;// 失去目标
    public const int RULE = 0x400;       // 规则（如超过手牌上限）
    public const int SPSUMMON = 0x800;   // 特殊召唤
    public const int RETURN = 0x1000;    // 返回
    public const int FUSION = 0x2000;    // 融合
    public const int SYNCHRO = 0x4000;   // 同调
    public const int RITUAL = 0x8000;    // 仪式
    public const int XYZ = 0x10000;      // 超量
    public const int REPLACE = 0x20000;  // 替换
    public const int DRAW = 0x40000;     // 抽牌
    public const int REDIRECT = 0x80000; // 重定向
}

// ============================================
// 效果代码 (EFFECT_CODE) - 具体的效果类型
// ============================================
[LuaCallCSharp]
public static class EffectCode
{
    // 攻击力/防御力修正
    public const int UPDATE_ATTACK = 1;
    public const int UPDATE_DEFENSE = 2;
    public const int SET_ATTACK = 3;
    public const int SET_DEFENSE = 4;
    public const int SET_ATTACK_FINAL = 5;
    public const int SET_DEFENSE_FINAL = 6;
    
    // 属性修正
    public const int CHANGE_LEVEL = 10;
    public const int CHANGE_RANK = 11;
    public const int CHANGE_ATTRIBUTE = 12;
    public const int CHANGE_RACE = 13;
    public const int CHANGE_TYPE = 14;
    
    // 战斗相关
    public const int INDESTRUCTABLE_BATTLE = 20;
    public const int INDESTRUCTABLE_EFFECT = 21;
    public const int CANNOT_DIRECT_ATTACK = 22;
    public const int DIRECT_ATTACK = 23;
    public const int EXTRA_ATTACK = 24;
    public const int MUST_ATTACK = 25;
    public const int CANNOT_ATTACK = 26;
    public const int PIERCE = 27;          // 贯穿伤害
    
    // 效果免疫
    public const int IMMUNE_EFFECT = 30;
    public const int CANNOT_BE_EFFECT_TARGET = 31;
    public const int CANNOT_DISABLE = 32;
    
    // 特殊状态
    public const int DISABLE = 40;
    public const int CANNOT_TRIGGER = 41;
    public const int CANNOT_CHANGE_POSITION = 42;
    public const int CANNOT_FLIP_SUMMON = 43;
    public const int CANNOT_SPECIAL_SUMMON = 44;
    
    // 抽牌/手牌
    public const int DRAW_COUNT = 50;
    public const int HAND_LIMIT = 51;
    
    // 伤害相关
    public const int CHANGE_DAMAGE = 60;
    public const int REFLECT_DAMAGE = 61;
    public const int NO_BATTLE_DAMAGE = 62;
    public const int NO_EFFECT_DAMAGE = 63;
    
    // 覆盖/翻转
    public const int SET_PROC = 70;
    
    // 召唤相关
    public const int LIMIT_SUMMON_PROC = 80;
    public const int EXTRA_SUMMON_COUNT = 81;
    
    // 自定义效果（游戏特有）
    public const int OVERLOAD = 1000;      // 过载
    public const int EVOLVE = 1001;        // 进化
    public const int FATIGUE = 1002;       // 疲劳
}

// ============================================
// 位置状态 (POSITION)
// ============================================
[LuaCallCSharp]
public static class Position
{
    public const int FACEUP_ATTACK = 0x1;
    public const int FACEDOWN_ATTACK = 0x2;
    public const int FACEUP_DEFENSE = 0x4;
    public const int FACEDOWN_DEFENSE = 0x8;
    public const int FACEUP = FACEUP_ATTACK | FACEUP_DEFENSE;
    public const int FACEDOWN = FACEDOWN_ATTACK | FACEDOWN_DEFENSE;
    public const int ATTACK = FACEUP_ATTACK | FACEDOWN_ATTACK;
    public const int DEFENSE = FACEUP_DEFENSE | FACEDOWN_DEFENSE;
}

// ============================================
// 玩家 (PLAYER)
// ============================================
[LuaCallCSharp]
public static class Player
{
    public const int SELF = 0;
    public const int OPPONENT = 1;
    public const int ALL = 2;
}

// ============================================
// 阶段 (PHASE)
// ============================================
[LuaCallCSharp]
public static class Phase
{
    public const int DRAW = 0x01;
    public const int STANDBY = 0x02;
    public const int MAIN1 = 0x04;
    public const int BATTLE_START = 0x08;
    public const int BATTLE_STEP = 0x10;
    public const int DAMAGE = 0x20;
    public const int DAMAGE_CAL = 0x40;
    public const int BATTLE = 0x80;
    public const int MAIN2 = 0x100;
    public const int END = 0x200;
}