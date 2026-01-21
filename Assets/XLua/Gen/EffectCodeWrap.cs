#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class EffectCodeWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(EffectCode);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 40, 0, 0);
			
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "UPDATE_ATTACK", EffectCode.UPDATE_ATTACK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "UPDATE_DEFENSE", EffectCode.UPDATE_DEFENSE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SET_ATTACK", EffectCode.SET_ATTACK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SET_DEFENSE", EffectCode.SET_DEFENSE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SET_ATTACK_FINAL", EffectCode.SET_ATTACK_FINAL);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SET_DEFENSE_FINAL", EffectCode.SET_DEFENSE_FINAL);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CHANGE_LEVEL", EffectCode.CHANGE_LEVEL);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CHANGE_RANK", EffectCode.CHANGE_RANK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CHANGE_ATTRIBUTE", EffectCode.CHANGE_ATTRIBUTE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CHANGE_RACE", EffectCode.CHANGE_RACE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CHANGE_TYPE", EffectCode.CHANGE_TYPE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "INDESTRUCTABLE_BATTLE", EffectCode.INDESTRUCTABLE_BATTLE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "INDESTRUCTABLE_EFFECT", EffectCode.INDESTRUCTABLE_EFFECT);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CANNOT_DIRECT_ATTACK", EffectCode.CANNOT_DIRECT_ATTACK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DIRECT_ATTACK", EffectCode.DIRECT_ATTACK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "EXTRA_ATTACK", EffectCode.EXTRA_ATTACK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MUST_ATTACK", EffectCode.MUST_ATTACK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CANNOT_ATTACK", EffectCode.CANNOT_ATTACK);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "PIERCE", EffectCode.PIERCE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "IMMUNE_EFFECT", EffectCode.IMMUNE_EFFECT);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CANNOT_BE_EFFECT_TARGET", EffectCode.CANNOT_BE_EFFECT_TARGET);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CANNOT_DISABLE", EffectCode.CANNOT_DISABLE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DISABLE", EffectCode.DISABLE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CANNOT_TRIGGER", EffectCode.CANNOT_TRIGGER);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CANNOT_CHANGE_POSITION", EffectCode.CANNOT_CHANGE_POSITION);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CANNOT_FLIP_SUMMON", EffectCode.CANNOT_FLIP_SUMMON);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CANNOT_SPECIAL_SUMMON", EffectCode.CANNOT_SPECIAL_SUMMON);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DRAW_COUNT", EffectCode.DRAW_COUNT);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "HAND_LIMIT", EffectCode.HAND_LIMIT);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "CHANGE_DAMAGE", EffectCode.CHANGE_DAMAGE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "REFLECT_DAMAGE", EffectCode.REFLECT_DAMAGE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "NO_BATTLE_DAMAGE", EffectCode.NO_BATTLE_DAMAGE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "NO_EFFECT_DAMAGE", EffectCode.NO_EFFECT_DAMAGE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SET_PROC", EffectCode.SET_PROC);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "LIMIT_SUMMON_PROC", EffectCode.LIMIT_SUMMON_PROC);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "EXTRA_SUMMON_COUNT", EffectCode.EXTRA_SUMMON_COUNT);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "OVERLOAD", EffectCode.OVERLOAD);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "EVOLVE", EffectCode.EVOLVE);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "FATIGUE", EffectCode.FATIGUE);
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "EffectCode does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        
        
        
        
        
		
		
		
		
    }
}
